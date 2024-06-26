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
using DevExpress;
using System.Windows.Forms;
using System.Drawing;
using Inventec.Desktop.Common.Controls.ValidationRule;
using DevExpress.XtraEditors.DXErrorProvider;
using Inventec.Desktop.Common.LanguageManager;
using System.Resources;
using AutoMapper;
using HIS.Desktop.Plugins.CareTypeList;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;

namespace HIS.Desktop.Plugins.CareTypeList.CareTypeList
{
    public partial class frmCareTypeList : HIS.Desktop.Utility.FormBase
    {

        #region Declare
        int rowCount = 0;
        int dataTotal = 0;
        int startPage = 0;
        int ActionType = -1;

        MOS.EFMODEL.DataModels.HIS_CARE_TYPE currentData;
        MOS.EFMODEL.DataModels.HIS_CARE_TYPE focusData;
        List<string> arrControlEnableNotChange = new List<string>();
        Dictionary<string, int> dicOrderTabIndexControl = new Dictionary<string, int>();
        Inventec.Desktop.Common.Modules.Module moduleData;
        #endregion


        #region FormConstructor

        public frmCareTypeList(Inventec.Desktop.Common.Modules.Module moduleData)
		:base(moduleData)
        {
            try
            {
                InitializeComponent();

                this.moduleData = moduleData;
                gridControlFormList.ToolTipController = toolTipControllerGrid;

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
                LogSystem.Warn(ex);
            }
        }
        #endregion

        private void frmCareTypeList_Load(object sender, EventArgs e)
        {
            try
            {
                SetCaptionByLanguageKey();
                WaitingManager.Show();

                //Gọi hàm khởi tạo, truyền tham số vào GridControl
                Init();


                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SetCaptionByLanguageKey()
        {
            try
            {
                ////Khoi tao doi tuong resource
                Resources.ResourceLanguageManager.LanguageResource = new ResourceManager("HIS.Desktop.Plugins.CareTypeList.Resources.Lang", typeof(HIS.Desktop.Plugins.CareTypeList.CareTypeList.frmCareTypeList).Assembly);

                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.bar1.Text = Inventec.Common.Resource.Get.Value("frmCareTypeList.bar1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnQuickSearch.Caption = Inventec.Common.Resource.Get.Value("frmCareTypeList.btnQuickSearch.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnQuickAdd.Caption = Inventec.Common.Resource.Get.Value("frmCareTypeList.btnQuickAdd.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnQuickEdit.Caption = Inventec.Common.Resource.Get.Value("frmCareTypeList.btnQuickEdit.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnQuickReset.Caption = Inventec.Common.Resource.Get.Value("frmCareTypeList.btnQuickReset.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnF2.Caption = Inventec.Common.Resource.Get.Value("frmCareTypeList.btnF2.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl1.Text = Inventec.Common.Resource.Get.Value("frmCareTypeList.layoutControl1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnReset.Text = Inventec.Common.Resource.Get.Value("frmCareTypeList.btnReset.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnAdd.Text = Inventec.Common.Resource.Get.Value("frmCareTypeList.btnAdd.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnEdit.Text = Inventec.Common.Resource.Get.Value("frmCareTypeList.btnEdit.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem6.Text = Inventec.Common.Resource.Get.Value("frmCareTypeList.layoutControlItem6.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem7.Text = Inventec.Common.Resource.Get.Value("frmCareTypeList.layoutControlItem7.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl2.Text = Inventec.Common.Resource.Get.Value("frmCareTypeList.layoutControl2.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.txtKeyWord.Properties.NullValuePrompt = Inventec.Common.Resource.Get.Value("frmCareTypeList.txtKeyWord.Properties.NullValuePrompt", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnSearch.Text = Inventec.Common.Resource.Get.Value("frmCareTypeList.btnSearch.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.STT.Caption = Inventec.Common.Resource.Get.Value("frmCareTypeList.STT.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumnCode.Caption = Inventec.Common.Resource.Get.Value("frmCareTypeList.gridColumnCode.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumnName.Caption = Inventec.Common.Resource.Get.Value("frmCareTypeList.gridColumnName.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.CREATE_TIME.Caption = Inventec.Common.Resource.Get.Value("frmCareTypeList.CREATE_TIME.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.CREATOR.Caption = Inventec.Common.Resource.Get.Value("frmCareTypeList.CREATOR.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.MODIFY_TIME.Caption = Inventec.Common.Resource.Get.Value("frmCareTypeList.MODIFY_TIME.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumnModifier.Caption = Inventec.Common.Resource.Get.Value("frmCareTypeList.gridColumnModifier.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.Text = Inventec.Common.Resource.Get.Value("frmCareTypeList.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());

                if (moduleData != null)
                {
                    this.Text = moduleData.text;
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }


        public void Init()
        {
            CommonParam param = new CommonParam();
            try
            {
                //Gán giá trị mặc định cho Form
                //Nếu Load mới thì keyword =""
                SetDefaultValue();

                //Lấy dữ liệu truyền vào Form
                LoadDataToGrid();

                //Gán Focus mặc định cho Form
                //KHÔNG CẦN THIẾT VÌ TRONG hàm SetDefaultValue đã có rồi
                //SetDefaultFocus();

                //Check đầu vào của Text
                ValidateForm();

                //Không cần, trong SetDefautlValue đã có rồi.
                //EnableControlChange(this.ActionType);

                //Đổi ngôn ngữ
                ChangeLanguage();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ChangeLanguage()
        {
            try
            {
                //Khởi tạo đối tượng Resource


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
                //Kiểm tra đàu vào của từng textbox
                ValidationSingleControl(txtCode);
                ValidationSingleControl(txtName);
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
                dxValidationProviderInfo.SetValidationRule(control, validRule);
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
                this.txtKeyWord.Focus();
                this.txtKeyWord.SelectAll();
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
                ResetFormData();

                //Sự kiện EnableControlChange này CHỈ thay đổi thuộc tính Enable của Button
                EnableControlChange(this.ActionType);

                this.txtKeyWord.ResetText();
                this.txtCode.Focus();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void EnableControlChange(int action)
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

        private void ResetFormData()
        {
            try
            {
                //Kiểm tra xem Layout Control có được gọi lên không
                if (!lcEditorInfo.IsInitialized)
                    return;

                //Nếu đã được gọi lên thì tiến hành Update
                lcEditorInfo.BeginUpdate();
                try
                {
                    //foreach (DevExpress.XtraLayout.LayoutControlItem item in lcEditorInfo.Items)
                    //{
                    //    DevExpress.XtraLayout.LayoutControlItem lci = item as DevExpress.XtraLayout.LayoutControlItem;
                    //    if (lci != null && lci.Control != null && lci.Control is BaseEdit)
                    //    {
                    //        DevExpress.XtraEditors.BaseEdit formatfrm = lci.Control as DevExpress.XtraEditors.BaseEdit;

                    //        formatfrm.ResetText();
                    //        formatfrm.EditValue = null;
                    //        this.txtName.Focus();
                    //    }
                    //}
                    currentData = null;
                    this.txtCode.ResetText();
                    this.txtName.ResetText();
                    this.txtCode.Focus();
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
            finally
            {
                lcEditorInfo.EndUpdate();
            }
        }

        private void LoadDataToGrid()
        {
            try
            {
                ////Để hiển thị được dữ liệu thì Grid cần có DataSource, nó sẽ tự đổ dữ liệu trong DS vào Grid.
                //gridControlFormList.DataSource = null;

                ////Khai báo biến - Biến này sẽ thông báo lỗi của Server nếu lỗi.
                //CommonParam param = new CommonParam();

                ////Khai báo bộ lọc, Tên khai báo của bộ lọc này sẽ là tên Project+Filter, đã có sẵn trong chương trình.
                //HisCareFilter filter = new HisCareFilter();

                ////Khai báo biến lưu trữ dữ liệu được get từ DB.
                //var cares = new BackendAdapter(param).Get<List<HIS_CARE_TYPE>>(ApiConsumer.HisRequestUriStore.HIS_CARE_TYPE_GET, ApiConsumers.MosConsumer, filter, param);

                ////Gán DataSource cho Grid
                //if (cares != null && cares.Count > 0)
                //{
                //    gridControlFormList.BeginUpdate();
                //    gridControlFormList.DataSource = cares;
                //    gridControlFormList.EndUpdate();
                //}

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
                ucPaging.Init(LoadPaging, param, numPageSize,this.gridControlFormList);
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadPaging(object commonParam)
        {
            try
            {
                startPage = ((CommonParam)commonParam).Start ?? 0;
                int limit = ((CommonParam)commonParam).Limit ?? 0;

                CommonParam param = new CommonParam(startPage, limit);

                Inventec.Core.ApiResultObject<List<MOS.EFMODEL.DataModels.HIS_CARE_TYPE>> apiResult = null;

                HisCareTypeFilter filter = new HisCareTypeFilter();

                SetFilterNavbar(ref filter);
                filter.ORDER_FIELD = "MODIFY_TIME";
                filter.ORDER_DIRECTION = "DESC";
                gridView1.BeginUpdate();

                apiResult = new BackendAdapter(param).GetRO<List<MOS.EFMODEL.DataModels.HIS_CARE_TYPE>>("api/HisCareType/Get", ApiConsumers.MosConsumer, filter, param);
                if (apiResult != null)
                {
                    var data = (List<MOS.EFMODEL.DataModels.HIS_CARE_TYPE>)apiResult.Data;
                    if (data != null)
                    {
                        gridControlFormList.DataSource = data;
                        rowCount = (data == null ? 0 : data.Count);
                        dataTotal = (apiResult.Param == null ? 0 : apiResult.Param.Count ?? 0);
                    }
                }

                gridView1.EndUpdate();

                #region Process has exception
                SessionManager.ProcessTokenLost(param);
                #endregion
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SetFilterNavbar(ref HisCareTypeFilter filter)
        {
            try
            {
                filter.KEY_WORD = this.txtKeyWord.Text.Trim();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridView1_CustomUnboundColumnData(object sender, CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)
                {
                    MOS.EFMODEL.DataModels.HIS_CARE_TYPE pData = (MOS.EFMODEL.DataModels.HIS_CARE_TYPE)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    short status = Inventec.Common.TypeConvert.Parse.ToInt16((pData.IS_ACTIVE ?? -1).ToString());

                    if (e.Column.FieldName == "STT")
                    {
                        e.Value = e.ListSourceRowIndex + 1 + startPage; //+ ((pagingGrid.CurrentPage - 1) * pagingGrid.PageSize);
                    }
                    else if (e.Column.FieldName == "CREATE_TIME_STR")
                    {
                        try
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString((long)pData.CREATE_TIME);
                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Warn(ex);
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
                            Inventec.Common.Logging.LogSystem.Warn(ex);
                        }
                    }
                }

                gridControlFormList.RefreshDataSource();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ChangeDataRow(HIS_CARE_TYPE rowData)
        {
            try
            {
                if (rowData != null)
                {
                    FillDataToEditorControl(rowData);
                    this.ActionType = GlobalVariables.ActionEdit;
                    EnableControlChange(this.ActionType);

                    //Chưa dùng
                    //positionHandle = -1;

                    Inventec.Desktop.Controls.ControlWorker.ValidationProviderRemoveControlError(dxValidationProviderInfo, dxErrorProvider);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void FillDataToEditorControl(HIS_CARE_TYPE rowData)
        {
            try
            {
                if (rowData != null)
                {
                    this.txtCode.Text = rowData.CARE_TYPE_CODE;
                    this.txtName.Text = rowData.CARE_TYPE_NAME;
                }

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
                Inventec.Desktop.Controls.ControlWorker.ValidationProviderRemoveControlError(dxValidationProviderInfo, dxErrorProvider);
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

        private void SaveProcess()
        {
            try
            {
                CommonParam param = new CommonParam();

                try
                {
                    bool success = false;

                    if (!btnEdit.Enabled && !btnAdd.Enabled)
                    {
                        return;
                    }

                    if (!dxValidationProviderInfo.Validate())
                    {
                        return;
                    }

                    WaitingManager.Show();

                    MOS.EFMODEL.DataModels.HIS_CARE_TYPE updateDTO = new MOS.EFMODEL.DataModels.HIS_CARE_TYPE();


                    //Lấy ID của đối tượng đang chọn, nếu là Update
                    if (this.currentData != null && this.currentData.ID > 0)
                    {
                        LoadCurrentData(this.currentData.ID, ref updateDTO);
                    }

                    else
                    {
                        UpdateDTOFromDataForm(ref updateDTO);
                    }

                    if (ActionType == GlobalVariables.ActionAdd)
                    {
                        var resultData = new BackendAdapter(param).Post<MOS.EFMODEL.DataModels.HIS_CARE_TYPE>("api/HisCareType/Create", ApiConsumers.MosConsumer, updateDTO, param);

                        if (resultData != null)
                        {
                            success = true;

                            LoadDataToGrid();

                            ResetFormData();
                        }
                    }
                    else
                    {
                        //Hàm Update
                        var resultData = new BackendAdapter(param).Post<MOS.EFMODEL.DataModels.HIS_CARE_TYPE>("api/HisCareType/Update", ApiConsumers.MosConsumer, updateDTO, param);

                        if (resultData != null)
                        {
                            success = true;

                            //Nếu cần Update tại vị trí đang Focus
                            LoadDataToGrid();
                        }
                    }

                    MessageManager.Show(this, param, success);

                    WaitingManager.Hide();
                }
                catch (Exception ex)
                {
                    WaitingManager.Hide();
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadCurrentData(long thisID, ref HIS_CARE_TYPE updateDTO)
        {
            try
            {
                CommonParam param = new CommonParam();
                HisCareTypeFilter filter = new HisCareTypeFilter();
                filter.ID = thisID;

                updateDTO = new BackendAdapter(param).Get<List<MOS.EFMODEL.DataModels.HIS_CARE_TYPE>>("api/HisCareType/Get",
                    ApiConsumers.MosConsumer, filter, param).FirstOrDefault();

                //Gán lại giá trị cần thay đổi cho đối tượng Update để tiến hành Update
                updateDTO.CARE_TYPE_CODE = this.txtCode.Text;
                updateDTO.CARE_TYPE_NAME = this.txtName.Text;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void UpdateRowAfterEdit(HIS_CARE_TYPE resultData)
        {
            try
            {
                var rowData = (MOS.EFMODEL.DataModels.HIS_CARE_TYPE)gridView1.GetFocusedRow();

                if (resultData == null)
                {
                    throw new ArgumentNullException("data(MOS.EFMODEL.DataModels.HIS_CARE_TYPE) is null");
                }
                else
                {
                    Inventec.Common.Mapper.DataObjectMapper.Map<MOS.EFMODEL.DataModels.HIS_CARE_TYPE>(rowData, resultData);
                    gridView1.RefreshRow(gridView1.FocusedRowHandle);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void UpdateDTOFromDataForm(ref HIS_CARE_TYPE updateDTO)
        {
            try
            {
                updateDTO.CARE_TYPE_CODE = txtCode.Text.Trim();
                updateDTO.CARE_TYPE_NAME = txtName.Text.Trim();
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

        private void btnSearch_Click(object sender, EventArgs e)
        {
            try
            {
                LoadDataToGrid();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtKeyWord_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    btnSearch.PerformClick();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtName_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (btnAdd.Enabled)
                    {
                        btnAdd.Focus();
                    }
                    if (btnEdit.Enabled)
                    {
                        btnEdit.Focus();
                    }
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtCode_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    this.txtName.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }



        private void gridButtonDelete_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {

        }

        private void gridButtonDelete_Click(object sender, EventArgs e)
        {
            try
            {
                CommonParam param = new CommonParam();
                HisCareTypeFilter filter = new HisCareTypeFilter();
                var rowdata = (MOS.EFMODEL.DataModels.HIS_CARE_TYPE)gridView1.GetFocusedRow();
                filter.ID = rowdata.ID;

                var result = new BackendAdapter(param).Get<List<MOS.EFMODEL.DataModels.HIS_CARE_TYPE>>
                    ("api/HisCareType/Get", ApiConsumers.MosConsumer, filter, param).FirstOrDefault();

                if (rowdata != null)
                {
                    bool success = false;
                    success = new BackendAdapter(param).Post<bool>
                        ("api/HisCareType/Delete", ApiConsumers.MosConsumer, result, param);
                    if (success)
                    {
                        LoadDataToGrid();
                        currentData = ((List<HIS_CARE_TYPE>)gridControlFormList.DataSource).FirstOrDefault();
                    }
                    MessageManager.Show(this, param, success);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridView1_CustomRowCellEdit(object sender, DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs e)
        {
            try
            {
                DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                if (e.RowHandle >= 0)
                {
                    HIS_CARE_TYPE data = (HIS_CARE_TYPE)((IList)((BaseView)sender).DataSource)[e.RowHandle];
                    if (e.Column.FieldName == "isLock")
                    {
                        e.RepositoryItem = (data.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__FALSE ? gridButtonUnlock : gridButtonLock);

                    }
                    if (e.Column.FieldName == "Delete")
                    {
                        e.RepositoryItem = (data.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE ? gridButtonDelete : deleteD);

                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSession.Warn(ex);
            }
        }

        private void repositoryItemButtonEdit1_Click(object sender, EventArgs e)
        {
            try
            {
                CommonParam param = new CommonParam();
                HisCareTypeFilter filter = new HisCareTypeFilter();
                var rowdata = (MOS.EFMODEL.DataModels.HIS_CARE_TYPE)gridView1.GetFocusedRow();
                filter.ID = rowdata.ID;

                var result = new BackendAdapter(param).Get<List<MOS.EFMODEL.DataModels.HIS_CARE_TYPE>>
                    ("api/HisCareType/Get", ApiConsumers.MosConsumer, filter, param).FirstOrDefault();

                if (rowdata != null)
                {
                    bool success = false;
                    success = new BackendAdapter(param).Post<bool>
                        ("api/HisCareType/Delete", ApiConsumers.MosConsumer, result, param);
                    if (success)
                    {
                        LoadDataToGrid();
                        currentData = ((List<HIS_CARE_TYPE>)gridControlFormList.DataSource).FirstOrDefault();
                    }
                    MessageManager.Show(this, param, success);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSession.Warn(ex);
            }
        }

        //private void gridView1_MouseDown(object sender, MouseEventArgs e)
        //{
        //    try
        //    {
        //        if ((Control.ModifierKeys & Keys.Control) != Keys.Control)
        //        {
        //            GridView view = sender as GridView;
        //            GridViewInfo viewInfo = view.GetViewInfo() as GridViewInfo;
        //            GridHitInfo hi = view.CalcHitInfo(e.Location);

        //            if (hi.HitTest == GridHitTest.RowCell)
        //            {
        //                if (hi.Column.FieldName == "Delete")
        //                {

        //                    gridButtonDelete_Click(null, null);
        //                    view.CloseEditor();
        //                }

        //                if (hi.Column.FieldName == "isLock")
        //                {
        //                    gridButtonLock_ButtonClick(null, null);
        //                    view.CloseEditor();
        //                }
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Inventec.Common.Logging.LogSystem.Warn(ex);
        //    }
        //}

        private void gridButtonDelete_ButtonClick_1(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {

        }

        private void gridButtonUnlock_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            CommonParam param = new CommonParam();
            HIS_CARE_TYPE success = new HIS_CARE_TYPE();

            try
            {
                HIS_CARE_TYPE hisCareTypeFocus = (HIS_CARE_TYPE)gridView1.GetFocusedRow();
                DialogResult dialog = new DialogResult();


                dialog = MessageBox.Show
                    (LibraryMessage.MessageUtil.GetMessage
                    (LibraryMessage.Message.Enum.HeThongTBCuaSoThongBaoBanCoMuonBoKhoaDuLieuKhong),
                    "", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (dialog == DialogResult.Yes)
                {
                    WaitingManager.Show();

                    success = new BackendAdapter(param).Post<HIS_CARE_TYPE>
                        ("api/HisCareType/ChangeLock", ApiConsumers.MosConsumer, hisCareTypeFocus, param);

                    WaitingManager.Hide();

                    if (success != null)
                    {
                        if (hisCareTypeFocus.IS_ACTIVE == 1)
                        {
                            btnEdit.Enabled = false;
                        }
                        else
                        {
                            FillDataToEditorControl(hisCareTypeFocus);
                            ActionType = GlobalVariables.ActionEdit;
                            EnableControlChange(ActionType);
                        }

                        LoadDataToGrid();

                        MessageManager.Show(this, param, true);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridButtonLock_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            CommonParam param = new CommonParam();
            HIS_CARE_TYPE success = new HIS_CARE_TYPE();

            try
            {
                HIS_CARE_TYPE hisCareTypeFocus = (HIS_CARE_TYPE)gridView1.GetFocusedRow();
                DialogResult dialog = new DialogResult();


                dialog = MessageBox.Show
                    (LibraryMessage.MessageUtil.GetMessage
                    (LibraryMessage.Message.Enum.HeThongTBCuaSoThongBaoBanCoMuonKhoaDuLieuKhong),
                    "", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (dialog == DialogResult.Yes)
                {
                    WaitingManager.Show();

                    success = new BackendAdapter(param).Post<HIS_CARE_TYPE>
                        ("api/HisCareType/ChangeLock", ApiConsumers.MosConsumer, hisCareTypeFocus, param);

                    WaitingManager.Hide();

                    if (success != null)
                    {
                        if (hisCareTypeFocus.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE)
                        {
                            btnEdit.Enabled = false;
                        }
                        else
                        {
                            FillDataToEditorControl(hisCareTypeFocus);
                            ActionType = GlobalVariables.ActionEdit;
                            EnableControlChange(ActionType);
                        }

                        LoadDataToGrid();

                        MessageManager.Show(this, param, true);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridView1_Click_1(object sender, EventArgs e)
        {
            try
            {

                var rowData = (MOS.EFMODEL.DataModels.HIS_CARE_TYPE)gridView1.GetFocusedRow();
                if (rowData != null)
                {
                    currentData = rowData;

                    if (rowData.IS_ACTIVE == 0)
                    {
                        btnEdit.Enabled = false;
                        btnAdd.Enabled = false;
                    }
                    else
                    {
                        ChangeDataRow(rowData);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                throw;
            }
        }

        private void btnQuickSearch_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                btnSearch.PerformClick();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnQuickAdd_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                btnAdd.PerformClick();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnQuickEdit_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                btnEdit.PerformClick();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnQuickReset_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                btnReset.PerformClick();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnAdd_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    btnAdd.PerformClick();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnEdit_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    btnEdit.PerformClick();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        //private void LoadCurrentData(long p, ref HIS_CARE_TYPE updateDTO)
        //{
        //    try
        //    {
        //        CommonParam param = new CommonParam();
        //        HisCareTypeFilter filter = new HisCareTypeFilter();
        //        filter.ID = p;

        //        updateDTO = new BackendAdapter(param).Get<List<MOS.EFMODEL.DataModels.HIS_CARE_TYPE>>(HisRequestUriStore.);
        //    }
        //    catch (Exception ex)
        //    {
        //        Inventec.Common.Logging.LogSystem.Warn(ex);
        //    }
        //}
    }
}
