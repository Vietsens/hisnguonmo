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
using DevExpress.XtraEditors.DXErrorProvider;
using DevExpress.XtraEditors.ViewInfo;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Controls.Session;
using HIS.Desktop.LocalStorage.ConfigApplication;
using HIS.Desktop.LocalStorage.LocalData;
using Inventec.Common.Adapter;
using Inventec.Common.Logging;
using Inventec.Core;
using Inventec.Desktop.Common.Controls.ValidationRule;
using Inventec.Desktop.Common.Message;
using Inventec.UC.Paging;
using SDA.Desktop.Plugins.SdaField.Resources;
using SDA.Desktop.Plugins.SdaField.SdaField;
using SDA.EFMODEL.DataModels;
using SDA.Filter;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace SDA.Desktop.Plugins.SdaField
{
    public partial class frmField : HIS.Desktop.Utility.FormBase
    {
        #region Reclare
        Inventec.Desktop.Common.Modules.Module moduleData;
        int start = 0;
        int limit = 0;
        int rowCount = 0;
        int dataTotal = 0;
        int ActionType = -1;
        int startPage = -1;
        int positionHandle = -1;

        SdaModuleFieldADO currentEdit;
        PagingGrid pagingGrid;
        SdaModuleFieldADO currentData;
        List<SdaModuleFieldADO> listSSdaModuleFieldADO { get; set; }
        private Inventec.UC.Paging.UcPaging ucPaging;
        #endregion

        #region Construct
        public frmField()
        {
            InitializeComponent();
            // TODO: Complete member initialization
        }

        public frmField(Inventec.Desktop.Common.Modules.Module moduleData)
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

        private void frmField_Load(object sender, EventArgs e)
        {
            try
            {
                // btnEdit1.Enabled = false;
                FillDataToGridControl();


                ValidateForm();
                this.ActionType = GlobalVariables.ActionAdd;//actionedit
                EnabledControlChanged(this.ActionType);
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }

        private void FillDataToGridControl()
        {
            try
            {
                int numPageSize;
                if (ucpaging1.pagingGrid != null)
                {
                    numPageSize = ucpaging1.pagingGrid.PageSize;
                }
                else
                {
                    numPageSize = ConfigApplicationWorker.Get<int>("CONFIG_KEY__NUM_PAGESIZE");
                    //mPageSize = (int)ConfigApplications.NumPageSize;
                }
                FillDataToGridSdaModuleField(new CommonParam(0, numPageSize));
                CommonParam param = new CommonParam();
                param.Limit = rowCount;
                param.Count = dataTotal;
                ucpaging1.Init(FillDataToGridSdaModuleField, param, numPageSize);
                //LoadPaging(new CommonParam(0, numPageSize));
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>
        /// Hàm load dữ liệu lên grid
        /// Truyền vào đối tượng chứa dữ liệu phân trang
        /// </summary>
        /// <param name="data"></param>
        private void FillDataToGridSdaModuleField(object data)
        {
            try
            {
                WaitingManager.Show();
                this.start = ((CommonParam)data).Start ?? 0;
                this.limit = ((CommonParam)data).Limit ?? 0;
                CommonParam param = new CommonParam(this.start, this.limit);
                this.listSSdaModuleFieldADO = new List<SdaModuleFieldADO>();
                Inventec.Core.ApiResultObject<List<SDA_MODULE_FIELD>> apiResult = null;
                SdaModuleFieldFilter filter = new SdaModuleFieldFilter();

                filter.KEY_WORD = txtFind.Text;

                apiResult = new BackendAdapter(param).GetRO<List<SDA_MODULE_FIELD>>("api/SdaModuleField/Get", ApiConsumers.SdaConsumer, filter, param);
                if (apiResult != null)
                {
                    this.listSSdaModuleFieldADO = (from m in ((List<SDA_MODULE_FIELD>)apiResult.Data) select new SdaModuleFieldADO(m)).ToList();
                    gridControl1.DataSource = null;
                    gridControl1.DataSource = this.listSSdaModuleFieldADO;
                    this.rowCount = (this.listSSdaModuleFieldADO == null ? 0 : this.listSSdaModuleFieldADO.Count);
                    this.dataTotal = (apiResult.Param == null ? 0 : apiResult.Param.Count ?? 0);
                }
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void EnabledControlChanged(int p)
        {
            try
            {

                btnAdd.Enabled = (p == GlobalVariables.ActionAdd);
                btnEdit1.Enabled = (p == GlobalVariables.ActionEdit);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SaveProcess()
        {
            try
            {
                bool success = false;
                if (!btnEdit1.Enabled && !btnAdd.Enabled)
                {
                    return;
                }
                positionHandle = -1;
                if (!dxValidationProvider1.Validate())
                    return;
                WaitingManager.Show();
                CommonParam param = new CommonParam();
                SDA_MODULE_FIELD updateDTO = new SDA_MODULE_FIELD();

                if (this.ActionType == GlobalVariables.ActionEdit && this.currentData != null)
                {
                    Inventec.Common.Mapper.DataObjectMapper.Map<SDA_MODULE_FIELD>(updateDTO, currentData);
                }

                UpdateDTOFromDataForm(ref updateDTO);
                if (ActionType == GlobalVariables.ActionAdd)
                {
                    var resultData = new BackendAdapter(param).Post<SDA_MODULE_FIELD>(
                        "api/sdaModuleField/Create", ApiConsumers.SdaConsumer, updateDTO, param);
                    if (resultData != null)
                    {
                        success = true;
                        FillDataToGridControl();
                        ResetFormData();
                        HIS.Desktop.LocalStorage.ConfigApplication.ConfigApplicationWorker.Init();
                    }
                }
                else
                {
                    if (currentData.ID > 0 && currentData.IS_ACTIVE == IMSys.DbConfig.SDA_RS.COMMON.IS_ACTIVE__TRUE)
                    {
                        updateDTO.ID = currentData.ID;
                        var resultData = new BackendAdapter(param).Post<SDA_MODULE_FIELD>(
                        "api/sdaModuleField/Update", ApiConsumers.SdaConsumer, updateDTO, param);
                        if (resultData != null)
                        {
                            success = true;
                            UpdateRowDataAfterEdit(resultData);
                            HIS.Desktop.LocalStorage.ConfigApplication.ConfigApplicationWorker.Init();
                        }
                    }
                    else
                    {
                        DevExpress.XtraEditors.XtraMessageBox.Show("Dữ liệu đang bị khóa", "Thông báo");
                        return;
                    }
                }

                #region Hien thi message thong bao
                MessageManager.Show(this, param, success);
                #endregion

                #region Neu phien lam viec bi mat, phan mem tu dong logout va tro ve trang login
                SessionManager.ProcessTokenLost(param);
                #endregion
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        private void UpdateRowDataAfterEdit(SDA_MODULE_FIELD data)
        {
            try
            {
                if (data == null)
                    throw new ArgumentNullException("data(SDA.EFMODEL.DataModels.SDA_MODULE_FIELD) is null");
                var rowData = (SDA_MODULE_FIELD)gridviewFormList.GetFocusedRow();
                if (rowData != null)
                {
                    Inventec.Common.Mapper.DataObjectMapper.Map<SDA_MODULE_FIELD>(rowData, data);
                    gridviewFormList.RefreshRow(gridviewFormList.FocusedRowHandle);
                }
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="updateDTO"></param>
        private void UpdateDTOFromDataForm(ref SDA_MODULE_FIELD updateDTO)
        {
            try
            {
                // updateDTO.VALUE_ALLOW_IN = (string)txtValueAllowIn.EditValue;
                updateDTO.FIELD_CODE = txtField_code.Text;
                updateDTO.FIELD_NAME = txtField_name.Text;
                updateDTO.MODULE_LINK = txtModule_link.Text;
                //updateDTO.IS_VISIBLE = (int)chk_is_visible.CheckStateChanged;
                //updateDTO.NUM_ORDER = (string)txtedit.EditValue;
                updateDTO.IS_VISIBLE = (short)txtField_name.EditValue;

                // updateDTO.Keyword = (string)txtKey.EditValue;
                //updateDTO.DEFAULT_VALUE = (string)txtDefaultValue.EditValue;
                //updateDTO.DESCRIPTION = (string)txtDescription.EditValue;
                //updateDTO.VALUE_TYPE = (string)cboValueType.EditValue;
                //if (txtValueAllowMin.EditValue != null)
                //{
                //    updateDTO.VALUE_ALLOW_MIN = (txtValueAllowMin.EditValue).ToString();
                //}

            }

            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ResetFormData()
        {
            txtField_code.EditValue = null;
            txtField_name.EditValue = null;
            txtModule_link.EditValue = null;
            txtField_code.Focus();
        }

        private void ChangDataRow(SdaModuleFieldADO data)
        {
            try
            {
                if (data != null)
                {
                    FillDataToEditorControl(data);
                    this.ActionType = GlobalVariables.ActionEdit;
                    if (data.IS_ACTIVE == 0)
                    {
                        this.ActionType = -1;
                    }
                    EnabledControlChanged(this.ActionType);
                }
                else
                {
                    ResetFormData();
                }
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void FillDataToEditorControl(SdaModuleFieldADO data)
        {
            try
            {
                txtField_code.EditValue = data.FIELD_CODE;
                txtModule_link.Text = data.MODULE_LINK;
                txtField_name.EditValue = data.FIELD_NAME; ;

            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SaveProcessGrid()
        {
            try
            {
                gridviewFormList.PostEditor();
                var row = (SdaModuleFieldADO)gridviewFormList.GetFocusedRow();
                if (row != null)
                {
                    bool success = false;

                    // WaitingManager.Show();
                    CommonParam param = new CommonParam();
                    SDA_MODULE_FIELD updateDTO = new SDA_MODULE_FIELD();

                    if (this.ActionType == GlobalVariables.ActionEdit)
                    {
                        Inventec.Common.Mapper.DataObjectMapper.Map<SdaModuleFieldADO>(updateDTO, row);//SDA_MODULE_FIELD
                    }

                    if (row.IS_VISIBLE_STR)
                        updateDTO.IS_VISIBLE = 1;
                    else
                        updateDTO.IS_VISIBLE = null;
                    //if (row.IS_ACTIVE)
                    //{
                    //    updateDTO.IS_ACTIVE = 1;
                    //}
                    //else updateDTO.IS_ACTIVE = 0;

                    //UpdateDTOFromDataForm(ref updateDTO);

                    if (row.ID > 0 && row.IS_ACTIVE == IMSys.DbConfig.SDA_RS.COMMON.IS_ACTIVE__TRUE)
                    {
                        updateDTO.ID = row.ID;
                        var resultData = new BackendAdapter(param).Post<SDA_MODULE_FIELD>(//SDA_MODULE_FIELD
                        "api/SdaModuleField/Update", ApiConsumers.SdaConsumer, updateDTO, param);
                        if (resultData != null)
                        {
                            success = true;
                            UpdateRowDataAfterEdit(row);//
                            HIS.Desktop.LocalStorage.ConfigApplication.ConfigApplicationWorker.Init();

                        }

                    }

                    else
                    {
                        WaitingManager.Hide();
                        DevExpress.XtraEditors.XtraMessageBox.Show("Dữ Liệu đang bị khóa", "Thông báo");
                        FillDataToGridControl();

                    }

                    WaitingManager.Hide();
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
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetDefaultValue()
        {
            try
            {
                this.ActionType = GlobalVariables.ActionAdd;
                txtField_code.Text = "";
                txtField_name.Text = "";
                txtModule_link.Text = "";
                ResetFormData();
                EnabledControlChanged(this.ActionType);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtFind_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    btnFind_Click(null, null);
                }
                else if (e.KeyCode == Keys.Down)
                {
                    gridviewFormList.Focus();
                    gridviewFormList.FocusedRowHandle = 0;
                    var data = (SdaModuleFieldADO)gridviewFormList.GetFocusedRow();
                    if (data != null)
                    {
                        ChangDataRow(data);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtSTT_Click(object sender, EventArgs e)
        {
            try
            {
                //WaitingManager.Show();
                this.currentData = (SdaModuleFieldADO)gridviewFormList.GetFocusedRow();//SDA_MODULE_FIELD
                if (this.currentData != null)
                {
                    ChangDataRow(this.currentData);
                    WaitingManager.Hide();
                }

            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtField_code_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {

                    txtField_name.Focus();
                    txtField_name.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtField_name_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {

                    txtModule_link.Focus();
                    txtModule_link.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtModule_link_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                if (this.ActionType == GlobalVariables.ActionAdd)
                    btnAdd.Focus();

            }
        }

        private void chk_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                this.ActionType = GlobalVariables.ActionEdit;
                SaveProcessGrid();

            }
            catch (Exception ex)
            {
                FillDataToGridControl();

                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridviewFormList_CellValueChanged(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        {
            try
            {
                if (e.Column.FieldName == "NUM_ORDER")
                {
                    TextEdit control = ((GridView)sender).ActiveEditor as TextEdit;
                    if (control != null)
                    {
                        SaveProcessGrid();
                    }
                }

            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Warn(ex); ;
            }
        }

        private void gridviewFormList_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)
                {
                    var data = (SDA_MODULE_FIELD)gridviewFormList.GetRow(e.ListSourceRowIndex);


                    //SdaModuleFieldADO pData = (SdaModuleFieldADO)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                    short status = Inventec.Common.TypeConvert.Parse.ToInt16((data.IS_ACTIVE ?? -1).ToString());
                    if (e.Column.FieldName == "STT")
                    {
                        //e.Value = e.ListSourceRowIndex + 1 + startPage; //+ ((pagingGrid.CurrentPage - 1) * pagingGrid.PageSize);
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
                    //else if (e.Column.FieldName == "NUM_ORDER_STR")
                    //{
                    //    try
                    //    {
                    //        e.Value = data != null && data.IS_VISIBLE == 1 ? true : false;
                    //    }
                    //    catch (Exception ex)
                    //    {
                    //        Inventec.Common.Logging.LogSystem.Warn("Loi set gia tri cho cot la ngoai dinh suat IS_HEIN_NDS_CHECK", ex);
                    //    }
                    //}

                    else if (e.Column.FieldName == "IS_ACTIVE_STR")
                    {
                        try
                        {
                            if (status == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE)//if (data.IS_ACTIVE == 1)
                                e.Value = "Hoạt động";
                            e.Value = "Tạm khóa";
                            //gridColumn1.ColumnEdit. = false;

                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Error(ex);
                        }
                    }
                    //else if (e.Column.FieldName == "IS_ACTIVE")
                    //{
                    //    try
                    //    {
                    //        if (data.IS_ACTIVE == 1)
                    //        {
                    //            e.Column.ColumnEdit = Lock;
                    //        }
                    //        else e.Column.ColumnEdit = unlock;

                    //    }
                    //    catch (Exception ex)
                    //    {

                    //        Inventec.Common.Logging.LogSystem.Error(ex);
                    //    }
                    //}
                    //else if (e.Column.FieldName == "DLT")
                    //{
                    //    try
                    //    {
                    //        if (data.IS_ACTIVE == 1)
                    //        {
                    //            e.Column.ColumnEdit = btndelete;
                    //        }
                    //        else e.Column.ColumnEdit = hidedelete;
                    //    }
                    //    catch (Exception)
                    //    {

                    //        throw;
                    //    }

                    //}

                    gridControl1.RefreshDataSource();
                    //gridviewFormList.RefreshData();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridControl1_Click(object sender, EventArgs e)
        {
            try
            {

                WaitingManager.Show();
                this.currentData = (SdaModuleFieldADO)gridviewFormList.GetFocusedRow();//SDA_MODULE_FIELD
                if (this.currentData != null)
                {
                    ChangDataRow(this.currentData);
                    WaitingManager.Hide();
                }

            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridviewFormList_RowCellStyle(object sender, RowCellStyleEventArgs e)
        {
            try
            {
                DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                if (e.RowHandle >= 0)
                {
                    SDA_MODULE_FIELD data = (SDA_MODULE_FIELD)((IList)((BaseView)sender).DataSource)[e.RowHandle];
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

        private void gridviewFormList_CustomRowCellEdit(object sender, CustomRowCellEditEventArgs e)
        {
            try
            {
                DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                if (e.RowHandle >= 0)
                {

                    SDA_MODULE_FIELD data = (SDA_MODULE_FIELD)((IList)((BaseView)sender).DataSource)[e.RowHandle];
                    if (e.Column.FieldName == "IS_LOCK")
                    {
                        e.RepositoryItem = (data.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE ? Lock : unlock);

                    }

                    if (e.Column.FieldName == "DLT")
                    {
                        e.RepositoryItem = (data.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE ? btndelete : hidedelete);
                    }
                    if (e.Column.FieldName == "NUM_ORDER")
                    {
                        if (data.IS_ACTIVE == 0)
                        {
                            e.Column.ColumnEdit.ReadOnly = true;

                        }
                        else e.Column.ColumnEdit.ReadOnly = false;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void Lock_ButtonClick_1(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            CommonParam param = new CommonParam();
            SDA_MODULE_FIELD success = new SDA_MODULE_FIELD();
            //bool notHandler = false;
            try
            {

                SDA_MODULE_FIELD data = (SDA_MODULE_FIELD)gridviewFormList.GetFocusedRow();
                if (MessageBox.Show(HIS.Desktop.LibraryMessage.MessageUtil.GetMessage(HIS.Desktop.LibraryMessage.Message.Enum.HeThongTBCuaSoThongBaoBanCoMuonKhoaDuLieuKhong), "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    SDA_MODULE_FIELD data1 = new SDA_MODULE_FIELD();
                    data1.ID = data.ID;
                    WaitingManager.Show();
                    success = new Inventec.Common.Adapter.BackendAdapter(param).Post<SDA_MODULE_FIELD>(HisRequestUriStore.SDAHIS_FIELD_CHANGE_LOCK, ApiConsumers.SdaConsumer, data1, param);
                    WaitingManager.Hide();
                    if (success != null)
                    {
                        FillDataToGridControl();
                    }
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void unlock_ButtonClick_1(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            CommonParam param = new CommonParam();
            SDA_MODULE_FIELD success = new SDA_MODULE_FIELD();
            //bool notHandler = false;
            try
            {

                SDA_MODULE_FIELD data = (SDA_MODULE_FIELD)gridviewFormList.GetFocusedRow();
                if (MessageBox.Show(HIS.Desktop.LibraryMessage.MessageUtil.GetMessage(HIS.Desktop.LibraryMessage.Message.Enum.HeThongTBCuaSoThongBaoBanCoMuonBoKhoaDuLieuKhong), "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    SDA_MODULE_FIELD data1 = new SDA_MODULE_FIELD();
                    data1.ID = data.ID;
                    WaitingManager.Show();
                    success = new Inventec.Common.Adapter.BackendAdapter(param).Post<SDA_MODULE_FIELD>(HisRequestUriStore.SDAHIS_FIELD_CHANGE_LOCK, ApiConsumers.SdaConsumer, data1, param);
                    WaitingManager.Hide();
                    if (success != null)
                    {
                        FillDataToGridControl();
                    }
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        #region Button click

        private void btnFind_Click(object sender, EventArgs e)
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

        private void btnAdd_Click_1(object sender, EventArgs e)
        {
            try
            {
                SaveProcess();
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnEdit1_Click(object sender, EventArgs e)
        {
            try
            {
                SaveProcess();
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }

        private void btnRef_Click(object sender, EventArgs e)
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

        private void btndelete_ButtonClick_1(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                CommonParam param = new CommonParam();
                var rowData = (SDA.EFMODEL.DataModels.SDA_MODULE_FIELD)gridviewFormList.GetFocusedRow();
                if (MessageBox.Show(HIS.Desktop.LibraryMessage.MessageUtil.GetMessage(HIS.Desktop.LibraryMessage.Message.Enum.HeThongTBCuaSoThongBaoBanCoMuonXoaDuLieuKhong), "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    SdaModuleFieldFilter filter = new SdaModuleFieldFilter();
                    filter.ID = rowData.ID;
                    var data = new BackendAdapter(param).Get<List<SDA_MODULE_FIELD>>(HisRequestUriStore.SDAHIS_FIELD_GET, ApiConsumers.SdaConsumer, filter, param).FirstOrDefault();

                    if (rowData != null)
                    {
                        bool success = false;
                        success = new BackendAdapter(param).Post<bool>(HisRequestUriStore.SDAHIS_FIELD_DELETE, ApiConsumers.SdaConsumer, data.ID, param);
                        if (success)
                        {
                            FillDataToGridControl();
                            HIS.Desktop.LocalStorage.ConfigApplication.ConfigApplicationWorker.Init();
                            //currentData = ((List<SDA_MODULE_FIELD>)gridControl1.DataSource).FirstOrDefault();
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

        private void barButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            btnAdd_Click_1(null, null);
        }

        private void barButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            btnEdit1_Click(null, null);
        }

        private void barButtonItem3_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            btnRef_Click(null, null);
        }
        #endregion

        #region Validate

        private void ValidateForm()
        {
            try
            {
                ValidationSingleControl(txtField_code);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ValidationSingleControl(BaseEdit control)
        {
            try
            {
                ControlEditValidationRule validRule = new ControlEditValidationRule();
                validRule.editor = control;
                validRule.ErrorText = String.Format(ResourceMessage.ChuaNhapTruongDuLieuBatBuoc);
                validRule.ErrorType = ErrorType.Warning;
                dxValidationProvider1.SetValidationRule(control, validRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        
        private void dxValidationProvider1_ValidationFailed(object sender, ValidationFailedEventArgs e)
        {
            try
            {
                BaseEdit edit = e.InvalidControl as BaseEdit;
                if (edit == null)
                    return;

                BaseEditViewInfo viewInfo = edit.GetViewInfo() as BaseEditViewInfo;
                if (viewInfo == null)
                    return;

                if (positionHandle == -1)
                {
                    positionHandle = edit.TabIndex;
                    edit.SelectAll();
                    edit.Focus();
                }
                if (positionHandle > edit.TabIndex)
                {
                    positionHandle = edit.TabIndex;
                    edit.SelectAll();
                    edit.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        #endregion

        #endregion

    }
}
