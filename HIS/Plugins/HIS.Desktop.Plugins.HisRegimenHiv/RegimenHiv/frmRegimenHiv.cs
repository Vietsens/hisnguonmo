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
using HIS.Desktop.Plugins.HisRegimenHiv;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;

namespace HIS.Desktop.Plugins.HisRegimenHiv.RegimenHiv
{
    public partial class frmRegimenHiv : HIS.Desktop.Utility.FormBase
    {

        #region Declare
        int rowCount = 0;
        int dataTotal = 0;
        int startPage = 0;
        int ActionType = -1;

        MOS.EFMODEL.DataModels.HIS_REGIMEN_HIV currentData;
        List<string> arrControlEnableNotChange = new List<string>();
        Dictionary<string, int> dicOrderTabIndexControl = new Dictionary<string, int>();
        Inventec.Desktop.Common.Modules.Module moduleData;
        #endregion


        #region FormConstructor

        public frmRegimenHiv(Inventec.Desktop.Common.Modules.Module moduleData)
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

        private void frmRegimenHiv_Load(object sender, EventArgs e)
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

        /// <summary>
        ///Hàm xét ngôn ngữ cho giao diện frmRegimenHiv
        /// </summary>
        private void SetCaptionByLanguageKey()
        {
            try
            {
                ////Khoi tao doi tuong resource
                Resources.ResourceLanguageManager.LanguageResource = new ResourceManager("HIS.Desktop.Plugins.HisRegimenHiv.Resources.Lang", typeof(frmRegimenHiv).Assembly);

                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.bar1.Text = Inventec.Common.Resource.Get.Value("frmRegimenHiv.bar1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnQuickSearch.Caption = Inventec.Common.Resource.Get.Value("frmRegimenHiv.btnQuickSearch.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnQuickAdd.Caption = Inventec.Common.Resource.Get.Value("frmRegimenHiv.btnQuickAdd.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnQuickEdit.Caption = Inventec.Common.Resource.Get.Value("frmRegimenHiv.btnQuickEdit.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnQuickReset.Caption = Inventec.Common.Resource.Get.Value("frmRegimenHiv.btnQuickReset.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnF2.Caption = Inventec.Common.Resource.Get.Value("frmRegimenHiv.btnF2.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl1.Text = Inventec.Common.Resource.Get.Value("frmRegimenHiv.layoutControl1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnReset.Text = Inventec.Common.Resource.Get.Value("frmRegimenHiv.btnReset.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnAdd.Text = Inventec.Common.Resource.Get.Value("frmRegimenHiv.btnAdd.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnEdit.Text = Inventec.Common.Resource.Get.Value("frmRegimenHiv.btnEdit.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem6.Text = Inventec.Common.Resource.Get.Value("frmRegimenHiv.layoutControlItem6.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem7.Text = Inventec.Common.Resource.Get.Value("frmRegimenHiv.layoutControlItem7.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl2.Text = Inventec.Common.Resource.Get.Value("frmRegimenHiv.layoutControl2.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.txtKeyWord.Properties.NullValuePrompt = Inventec.Common.Resource.Get.Value("frmRegimenHiv.txtKeyWord.Properties.NullValuePrompt", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnSearch.Text = Inventec.Common.Resource.Get.Value("frmRegimenHiv.btnSearch.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.STT.Caption = Inventec.Common.Resource.Get.Value("frmRegimenHiv.STT.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumnCode.Caption = Inventec.Common.Resource.Get.Value("frmRegimenHiv.gridColumnCode.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumnName.Caption = Inventec.Common.Resource.Get.Value("frmRegimenHiv.gridColumnName.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn9.Caption = Inventec.Common.Resource.Get.Value("frmRegimenHiv.gridColumn9.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.CREATE_TIME.Caption = Inventec.Common.Resource.Get.Value("frmRegimenHiv.CREATE_TIME.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.CREATOR.Caption = Inventec.Common.Resource.Get.Value("frmRegimenHiv.CREATOR.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.MODIFY_TIME.Caption = Inventec.Common.Resource.Get.Value("frmRegimenHiv.MODIFY_TIME.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumnModifier.Caption = Inventec.Common.Resource.Get.Value("frmRegimenHiv.gridColumnModifier.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.Text = Inventec.Common.Resource.Get.Value("frmRegimenHiv.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
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
                ValidationSingleControl(txtCode, 20);
                ValidationSingleControl(txtName, 1000);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ValidationSingleControl(BaseEdit control,int maxLength)
        {
            try
            {
                ValidMaxlength validRule = new ValidMaxlength();
                validRule.textEdit = control;
                validRule.maxLength = maxLength;
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

                if (currentData != null && currentData.IS_ACTIVE == 0)
                {
                    btnEdit.Enabled = false;
                    btnAdd.Enabled = false;
                }
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

                Inventec.Core.ApiResultObject<List<MOS.EFMODEL.DataModels.HIS_REGIMEN_HIV>> apiResult = null;

                HisRegimenHivFilter filter = new HisRegimenHivFilter();

                SetFilterNavbar(ref filter);
                filter.ORDER_FIELD = "MODIFY_TIME";
                filter.ORDER_DIRECTION = "DESC";
                gridView1.BeginUpdate();

                apiResult = new BackendAdapter(param).GetRO<List<MOS.EFMODEL.DataModels.HIS_REGIMEN_HIV>>("api/HisRegimenHiv/Get", ApiConsumers.MosConsumer, filter, param);
                if (apiResult != null)
                {
                    var data = (List<MOS.EFMODEL.DataModels.HIS_REGIMEN_HIV>)apiResult.Data;
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

        private void SetFilterNavbar(ref HisRegimenHivFilter filter)
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
                    MOS.EFMODEL.DataModels.HIS_REGIMEN_HIV pData = (MOS.EFMODEL.DataModels.HIS_REGIMEN_HIV)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    short status = Inventec.Common.TypeConvert.Parse.ToInt16((pData.IS_ACTIVE ?? -1).ToString());

                    if (e.Column.FieldName == "STT")
                    {
                        e.Value = e.ListSourceRowIndex + 1 + startPage; //+ ((pagingGrid.CurrentPage - 1) * pagingGrid.PageSize);
                    }
                    else if (e.Column.FieldName == "CREATE_TIME_STR" && pData.CREATE_TIME != null)
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
                    else if (e.Column.FieldName == "MODIFY_TIME_STR" && pData.MODIFY_TIME != null)
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
                    else if (e.Column.FieldName == "IS_ACTIVE_str")
                    {
                        e.Value = pData != null && pData.IS_ACTIVE == 1 ? "Hoạt động" : "Tạm khóa";
                    }
                }

                gridControlFormList.RefreshDataSource();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ChangeDataRow(HIS_REGIMEN_HIV rowData)
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

        private void FillDataToEditorControl(HIS_REGIMEN_HIV rowData)
        {
            try
            {
                if (rowData != null)
                {
                    this.txtCode.Text = rowData.REGIMEN_HIV_CODE;
                    this.txtName.Text = rowData.REGIMEN_HIV_NAME;
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

                    MOS.EFMODEL.DataModels.HIS_REGIMEN_HIV updateDTO = new MOS.EFMODEL.DataModels.HIS_REGIMEN_HIV();


                    //Lấy ID của đối tượng đang chọn, nếu là Update
                    if (this.currentData != null && this.currentData.ID > 0)
                    {
                        LoadCurrentData(this.currentData.ID, ref updateDTO);
                    }

                    else
                    {
                        UpdateDTOFromDataForm(ref updateDTO);
                    }

                    Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => updateDTO), updateDTO));
                    if (ActionType == GlobalVariables.ActionAdd)
                    {
                        var resultData = new BackendAdapter(param).Post<MOS.EFMODEL.DataModels.HIS_REGIMEN_HIV>("api/HisRegimenHiv/Create", ApiConsumers.MosConsumer, updateDTO, param);

                        if (resultData != null)
                        {
                            success = true;

                            LoadDataToGrid();
                        }
                    }
                    else
                    {
                        //Hàm Update
                        var resultData = new BackendAdapter(param).Post<MOS.EFMODEL.DataModels.HIS_REGIMEN_HIV>("api/HisRegimenHiv/Update", ApiConsumers.MosConsumer, updateDTO, param);

                        if (resultData != null)
                        {
                            success = true;

                            //Nếu cần Update tại vị trí đang Focus
                            LoadDataToGrid();
                        }
                    }
                    if (success)
                        SetDefaultValue();
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

        private void LoadCurrentData(long thisID, ref HIS_REGIMEN_HIV updateDTO)
        {
            try
            {
                updateDTO = currentData;
                //Gán lại giá trị cần thay đổi cho đối tượng Update để tiến hành Update
                updateDTO.REGIMEN_HIV_CODE = this.txtCode.Text.Trim();
                updateDTO.REGIMEN_HIV_NAME = this.txtName.Text.Trim();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void UpdateRowAfterEdit(HIS_REGIMEN_HIV resultData)
        {
            try
            {
                var rowData = (MOS.EFMODEL.DataModels.HIS_REGIMEN_HIV)gridView1.GetFocusedRow();

                if (resultData == null)
                {
                    throw new ArgumentNullException("data(MOS.EFMODEL.DataModels.HIS_REGIMEN_HIV) is null");
                }
                else
                {
                    Inventec.Common.Mapper.DataObjectMapper.Map<MOS.EFMODEL.DataModels.HIS_REGIMEN_HIV>(rowData, resultData);
                    gridView1.RefreshRow(gridView1.FocusedRowHandle);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void UpdateDTOFromDataForm(ref HIS_REGIMEN_HIV updateDTO)
        {
            try
            {
                updateDTO.REGIMEN_HIV_CODE = txtCode.Text.Trim();
                updateDTO.REGIMEN_HIV_NAME = txtName.Text.Trim();
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
                ResetFormData();
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
                HIS_REGIMEN_HIV HisRegimenHivFocus = (HIS_REGIMEN_HIV)gridView1.GetFocusedRow();
                if (HisRegimenHivFocus != null)
                {
                    bool success = false;
                    success = new BackendAdapter(param).Post<bool>
                        ("api/HisRegimenHiv/Delete", ApiConsumers.MosConsumer, HisRegimenHivFocus.ID, param);
                    if (success)
                    {
                        ResetFormData();
                        LoadDataToGrid();
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
                    HIS_REGIMEN_HIV data = (HIS_REGIMEN_HIV)((IList)((BaseView)sender).DataSource)[e.RowHandle];
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
                if (currentData != null)
                {
                    bool success = false;
                    success = new BackendAdapter(param).Post<bool>
                        ("api/HisRegimenHiv/Delete", ApiConsumers.MosConsumer, currentData, param);
                    if (success)
                    {
                        ResetFormData();
                        LoadDataToGrid();
                    }
                    MessageManager.Show(this, param, success);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSession.Warn(ex);
            }
        }

        private void gridButtonUnlock_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            CommonParam param = new CommonParam();
            HIS_REGIMEN_HIV success = new HIS_REGIMEN_HIV();

            try
            {
                HIS_REGIMEN_HIV HisRegimenHivFocus = (HIS_REGIMEN_HIV)gridView1.GetFocusedRow();
                DialogResult dialog = new DialogResult();


                dialog = MessageBox.Show
                    (LibraryMessage.MessageUtil.GetMessage
                    (LibraryMessage.Message.Enum.HeThongTBCuaSoThongBaoBanCoMuonBoKhoaDuLieuKhong),
                    "", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (dialog == DialogResult.Yes)
                {
                    WaitingManager.Show();

                    success = new BackendAdapter(param).Post<HIS_REGIMEN_HIV>
                        ("api/HisRegimenHiv/ChangeLock", ApiConsumers.MosConsumer, HisRegimenHivFocus.ID, param);

                    WaitingManager.Hide();

                    if (success != null)
                    {
                        ResetFormData();
                        LoadDataToGrid();
                    }

                    MessageManager.Show(this, param, success != null);
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
            HIS_REGIMEN_HIV success = new HIS_REGIMEN_HIV();

            try
            {
                HIS_REGIMEN_HIV HisRegimenHivFocus = (HIS_REGIMEN_HIV)gridView1.GetFocusedRow();
                DialogResult dialog = new DialogResult();


                dialog = MessageBox.Show
                    (LibraryMessage.MessageUtil.GetMessage
                    (LibraryMessage.Message.Enum.HeThongTBCuaSoThongBaoBanCoMuonKhoaDuLieuKhong),
                    "", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (dialog == DialogResult.Yes)
                {
                    WaitingManager.Show();

                    success = new BackendAdapter(param).Post<HIS_REGIMEN_HIV>
                        ("api/HisRegimenHiv/ChangeLock", ApiConsumers.MosConsumer, HisRegimenHivFocus.ID, param);

                    WaitingManager.Hide();

                    if (success != null)
                    {
                        ResetFormData();
                        LoadDataToGrid();
                    }

                    MessageManager.Show(this, param, success != null);
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

                var rowData = (MOS.EFMODEL.DataModels.HIS_REGIMEN_HIV)gridView1.GetFocusedRow();
                if (rowData != null)
                {
                    currentData = rowData;
                    ChangeDataRow(rowData);
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

        private void gridView1_RowCellStyle(object sender, RowCellStyleEventArgs e)
        {
            try
            {
                DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                if (e.RowHandle >= 0)
                {
                    short isActive = Inventec.Common.TypeConvert.Parse.ToInt16((gridView1.GetRowCellValue(e.RowHandle, "IS_ACTIVE") ?? "").ToString());

                    HIS_REGIMEN_HIV data = (HIS_REGIMEN_HIV)((IList)((BaseView)sender).DataSource)[e.RowHandle];
                    if (e.Column.FieldName == "IS_ACTIVE_str")
                    {
                        if (isActive == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__FALSE)
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
    }
}
