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
using System.Windows.Forms;
using System.Drawing;
using Inventec.Desktop.Common.Controls.ValidationRule;
using DevExpress.XtraEditors.DXErrorProvider;
using MOS.SDO;
using Inventec.Desktop.Common.LanguageManager;
using System.Resources;

namespace HIS.Desktop.Plugins.HisPayForm
{
    public partial class frmHisPayForm : HIS.Desktop.Utility.FormBase
    {
        #region Declare
        int rowCount = 0;
        int dataTotal = 0;
        int startPage = 0;
        PagingGrid pagingGrid;
        int ActionType = -1;
        int positionHandle = -1;
        MOS.EFMODEL.DataModels.HIS_PAY_FORM currentData;
        List<string> arrControlEnableNotChange = new List<string>();
        Dictionary<string, int> dicOrderTabIndexControl = new Dictionary<string, int>();
        Inventec.Desktop.Common.Modules.Module moduleData;
        HIS_PAY_FORM currrentData;
        #endregion

        #region Construct
        public frmHisPayForm(Inventec.Desktop.Common.Modules.Module moduleData)
            : base(moduleData)
        {
            try
            {
                InitializeComponent();

                pagingGrid = new PagingGrid();
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
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        #endregion

        #region Private method
        private void frmHisCashierRoom_Load(object sender, EventArgs e)
        {
            try
            {
                timer1.Interval = 100;
                RegisterTimer(GetModuleLink(), "timer1", timer1.Interval, timer1_Tick);
                MeShow();
                StartTimer(GetModuleLink(), "timer1");
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
                Resources.ResourceLanguageManager.LanguageResource = new ResourceManager("HIS.Desktop.Plugins.HisPayForm.Resources.Lang", typeof(HIS.Desktop.Plugins.HisPayForm.frmHisPayForm).Assembly);
                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.layoutControl4.Text = Inventec.Common.Resource.Get.Value("frmHisPayForm.layoutControl4.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl7.Text = Inventec.Common.Resource.Get.Value("frmHisPayForm.layoutControl7.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnSearch.Text = Inventec.Common.Resource.Get.Value("frmHisPayForm.btnSearch.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.STT.Caption = Inventec.Common.Resource.Get.Value("frmHisPayForm.STT.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());

                this.grdColCashierRoomCode.Caption = Inventec.Common.Resource.Get.Value("frmHisPayForm.grdColCashierRoomCode.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColCashierRoomCode.ToolTip = Inventec.Common.Resource.Get.Value("frmHisPayForm.grdColCashierRoomCode.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColCashierRoomName.Caption = Inventec.Common.Resource.Get.Value("frmHisPayForm.grdColCashierRoomName.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColCashierRoomName.ToolTip = Inventec.Common.Resource.Get.Value("frmHisPayForm.grdColCashierRoomName.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColCreateTime.Caption = Inventec.Common.Resource.Get.Value("frmHisPayForm.grdColCreateTime.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColCreateTime.ToolTip = Inventec.Common.Resource.Get.Value("frmHisPayForm.grdColCreateTime.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColCreator.Caption = Inventec.Common.Resource.Get.Value("frmHisPayForm.grdColCreator.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColCreator.ToolTip = Inventec.Common.Resource.Get.Value("frmHisPayForm.grdColCreator.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColModifyTime.Caption = Inventec.Common.Resource.Get.Value("frmHisPayForm.grdColModifyTime.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColModifyTime.ToolTip = Inventec.Common.Resource.Get.Value("frmHisPayForm.grdColModifyTime.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColModifier.Caption = Inventec.Common.Resource.Get.Value("frmHisPayForm.grdColModifier.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColModifier.ToolTip = Inventec.Common.Resource.Get.Value("frmHisPayForm.grdColModifier.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.txtKeyword.Properties.NullValuePrompt = Inventec.Common.Resource.Get.Value("frmHisPayForm.txtKeyword.Properties.NullValuePrompt", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());

               

                //this.lkRoomTypeId.Properties.NullText = Inventec.Common.Resource.Get.Value("frmHisCashierRoom.lkRoomTypeId.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());

                //this.lciRoomId.Text = Inventec.Common.Resource.Get.Value("frmHisCashierRoom.lciRoomId.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());

                this.Text = Inventec.Common.Resource.Get.Value("frmHisPayForm.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                if (this.moduleData != null && !String.IsNullOrEmpty(this.moduleData.text))
                {
                    this.Text = this.moduleData.text;
                }
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
                txtName.Focus();
                txtName.SelectAll();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Debug(ex);
            }
        }

        private void InitTabIndex()
        {
            try
            {
                //dicOrderTabIndexControl.Add("txtCashierRoomCode", 0);
                //dicOrderTabIndexControl.Add("txtCashierRoomName", 1);
                //dicOrderTabIndexControl.Add("lkRoomTypeId", 2);


                //if (dicOrderTabIndexControl != null)
                //{
                //    foreach (KeyValuePair<string, int> itemOrderTab in dicOrderTabIndexControl)
                //    {
                //        SetTabIndexToControl(itemOrderTab, lcEditorInfo);
                //    }
                //}
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





        #endregion

        /// <summary>
        /// Ham lay du lieu theo dieu kien tim kiem va gan du lieu vao danh sach
        /// </summary>
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
                startPage = ((CommonParam)param).Start ?? 0;
                int limit = ((CommonParam)param).Limit ?? 0;
                CommonParam paramCommon = new CommonParam(startPage, limit);
                Inventec.Core.ApiResultObject<List<MOS.EFMODEL.DataModels.HIS_PAY_FORM>> apiResult = null;
                HisPayFormFilter filter = new HisPayFormFilter();
               // SetFilterNavBar(ref filter);
                if (!string.IsNullOrEmpty(txtKeyword.Text))
                {
                    filter.KEY_WORD = txtKeyword.Text.Trim();
                }
                filter.ORDER_DIRECTION = "DESC";
                filter.ORDER_FIELD = "MODIFY_TIME";
                gridviewFormList.BeginUpdate();
                apiResult = new BackendAdapter(paramCommon).GetRO<List<MOS.EFMODEL.DataModels.HIS_PAY_FORM>>("/api/HisPayForm/Get", ApiConsumers.MosConsumer, filter, paramCommon);
                if (apiResult != null)
                {
                    Inventec.Common.Logging.LogSystem.Debug("HIS_PAY_FORM___apiResult" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => apiResult), apiResult));
                    var data = (List<MOS.EFMODEL.DataModels.HIS_PAY_FORM>)apiResult.Data;
                    if (data != null)
                    {
                        gridviewFormList.GridControl.DataSource = data;
                        rowCount = (data == null ? 0 : data.Count);
                        dataTotal = (apiResult.Param == null ? 0 : apiResult.Param.Count ?? 0);
                    }
                }
                gridviewFormList.EndUpdate();

                #region Process has exception
                SessionManager.ProcessTokenLost(paramCommon);
                #endregion
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        private void SetFilterNavBar(ref HisPayFormFilter filter)
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
                    gridviewFormList.Focus();
                    gridviewFormList.FocusedRowHandle = 0;
                    var rowData = (MOS.EFMODEL.DataModels.HIS_PAY_FORM)gridviewFormList.GetFocusedRow();
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

        private void gridviewFormList_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)
                {
                    MOS.EFMODEL.DataModels.HIS_PAY_FORM pData = (MOS.EFMODEL.DataModels.HIS_PAY_FORM)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                    short status = Inventec.Common.TypeConvert.Parse.ToInt16((pData.IS_ACTIVE ?? -1).ToString());
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
                    else if (e.Column.FieldName == "IS_PAUSE_CHECK")
                    {
                        try
                        {
                            //  e.Value = pData != null && pData.IS_PAUSE == 1 ? true : false;
                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Warn("Loi set gia tri cho cot la ngoai dinh suat IS_HEIN_NDS_CHECK", ex);
                        }
                    }

                    else if (e.Column.FieldName == "IS_ACTIVE_STR")
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
                    else if (e.Column.FieldName == "PAY_FORM_CODE")
                    {
                        e.Value = pData.PAY_FORM_CODE;
                    }
                    else if (e.Column.FieldName == "PAY_FORM_NAME")
                    {
                        e.Value = pData.PAY_FORM_NAME;
                    }
                   

                    gridControlFormList.RefreshDataSource();
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
                var rowData = (MOS.EFMODEL.DataModels.HIS_PAY_FORM)gridviewFormList.GetFocusedRow();
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
                    var rowData = (MOS.EFMODEL.DataModels.HIS_PAY_FORM)gridviewFormList.GetFocusedRow();
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
        private void ChangedDataRow(MOS.EFMODEL.DataModels.HIS_PAY_FORM data)
        {
            try
            {
                if (data != null)
                {

                    this.ActionType = GlobalVariables.ActionEdit;

                    positionHandle = -1;
                    Inventec.Desktop.Controls.ControlWorker.ValidationProviderRemoveControlError(dxValidationProviderEditorInfo, dxErrorProvider);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// Gan focus vao control mac dinh
        /// </summary>
        private void SetFocusEditor()
        {
            try
            {

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Debug(ex);
            }
        }

        private void LoadCurrent(long currentId, ref MOS.EFMODEL.DataModels.HIS_CASHIER_ROOM currentDTO)
        {
            try
            {
                CommonParam param = new CommonParam();
                HisCashierRoomFilter filter = new HisCashierRoomFilter();
                filter.ID = currentId;
                currentDTO = new BackendAdapter(param).Get<List<MOS.EFMODEL.DataModels.HIS_CASHIER_ROOM>>(HisRequestUriStore.MOSHIS_CASHIER_ROOM_GET, ApiConsumers.MosConsumer, filter, param).FirstOrDefault();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void dxValidationProvider_ValidationFailed(object sender, DevExpress.XtraEditors.DXErrorProvider.ValidationFailedEventArgs e)
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

        #region Button handler
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

        //private void SaveProcess()
        //{
        //    CommonParam param = new CommonParam();
        //    try
        //    {
        //        bool success = false;

        //        positionHandle = -1;
        //        if (!dxValidationProviderEditorInfo.Validate())
        //            return;

        //        WaitingManager.Show();
        //        HisCashierRoomSDO sdo = new HisCashierRoomSDO();
        //        MOS.EFMODEL.DataModels.HIS_CASHIER_ROOM updateDTO = new MOS.EFMODEL.DataModels.HIS_CASHIER_ROOM();
        //        HIS_ROOM room = new HIS_ROOM();
        //        //Inventec.Common.Mapper.DataObjectMapper.Map<HIS_CASHIER_ROOM>(updateDTO,currentData);

        //        if (this.currentData != null && this.currentData.ID > 0)
        //        {
        //            LoadCurrent(this.currentData.ID, ref updateDTO);
        //        }


        //        sdo.HisCashierRoom = updateDTO;
        //        //updateDTO.ROOM_ID = currentData.ROOM_ID;
        //        room.ROOM_TYPE_ID = IMSys.DbConfig.HIS_RS.HIS_ROOM_TYPE.ID__TN;
        //        sdo.HisRoom = room;

        //        if (ActionType == GlobalVariables.ActionAdd)
        //        {
        //            updateDTO.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
        //            var resultData = new BackendAdapter(param).Post<MOS.EFMODEL.DataModels.HIS_CASHIER_ROOM>(HisRequestUriStore.MOSHIS_CASHIER_ROOM_CREATE, ApiConsumers.MosConsumer, sdo, param);
        //            if (resultData != null)
        //            {
        //                success = true;
        //                FillDataToGridControl();

        //            }
        //        }
        //        else
        //        {

        //            var resultData = new BackendAdapter(param).Post<MOS.EFMODEL.DataModels.HIS_CASHIER_ROOM>(HisRequestUriStore.MOSHIS_CASHIER_ROOM_UPDATE, ApiConsumers.MosConsumer, sdo, param);
        //            if (resultData != null)
        //            {
        //                success = true;
        //                //UpdateRowDataAfterEdit(resultData);
        //                FillDataToGridControl();
        //                //ResetFormData();
        //            }
        //        }

        //        if (success)
        //        {
        //            SetFocusEditor();
        //        }

        //        WaitingManager.Hide();

        //        #region Hien thi message thong bao
        //        MessageManager.Show(this, param, success);
        //        #endregion

        //        #region Neu phien lam viec bi mat, phan mem tu dong logout va tro ve trang login
        //        SessionManager.ProcessTokenLost(param);
        //        #endregion
        //    }
        //    catch (Exception ex)
        //    {
        //        WaitingManager.Hide();
        //        Inventec.Common.Logging.LogSystem.Warn(ex);
        //    }
        //}
        private void UpdateRowDataAfterEdit(MOS.EFMODEL.DataModels.HIS_CASHIER_ROOM data)
        {
            try
            {
                if (data == null)
                    throw new ArgumentNullException("data(MOS.EFMODEL.DataModels.V_HIS_CASHIER_ROOM) is null");
                var rowData = (MOS.EFMODEL.DataModels.V_HIS_CASHIER_ROOM)gridviewFormList.GetFocusedRow();
                if (rowData != null)
                {
                    Inventec.Common.Mapper.DataObjectMapper.Map<MOS.EFMODEL.DataModels.V_HIS_CASHIER_ROOM>(rowData, data);
                    gridviewFormList.RefreshRow(gridviewFormList.FocusedRowHandle);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        #endregion

        #region Validate


        private void ValidateLookupWithTextEdit(LookUpEdit cbo, TextEdit textEdit)
        {
            try
            {
                LookupEditWithTextEditValidationRule validRule = new LookupEditWithTextEditValidationRule();
                validRule.txtTextEdit = textEdit;
                validRule.cbo = cbo;
                validRule.ErrorText = MessageUtil.GetMessage(LibraryMessage.Message.Enum.TruongDuLieuBatBuoc);
                validRule.ErrorType = ErrorType.Warning;
                dxValidationProviderEditorInfo.SetValidationRule(textEdit, validRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ValidateGridLookupWithTextEdit(GridLookUpEdit cbo, TextEdit textEdit)
        {
            try
            {
                GridLookupEditWithTextEditValidationRule validRule = new GridLookupEditWithTextEditValidationRule();
                validRule.txtTextEdit = textEdit;
                validRule.cbo = cbo;
                validRule.ErrorText = MessageUtil.GetMessage(LibraryMessage.Message.Enum.TruongDuLieuBatBuoc);
                validRule.ErrorType = ErrorType.Warning;
                dxValidationProviderEditorInfo.SetValidationRule(textEdit, validRule);
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
                dxValidationProviderEditorInfo.SetValidationRule(control, validRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        #endregion

        #region Tooltip
        private void toolTipControllerGrid_GetActiveObjectInfo(object sender, ToolTipControllerGetActiveObjectInfoEventArgs e)
        {
            try
            {
                //TODO

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        #endregion

        #region Public method
        public void MeShow()
        {
            try
            {

                //Load du lieu
                FillDataToGridControl();

                //Load ngon ngu label control
                SetCaptionByLanguageKey();

                //Set tabindex control
             //   InitTabIndex();

                //Focus default
                SetDefaultFocus();
                ValidForm();
                btnSave.Enabled = false;

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        #endregion

        private void ValidationMaxLength(Control control, int? maxLength, bool required = false)
        {
            try
            {
                ControlMaxLengthValidationRule valid = new ControlMaxLengthValidationRule();
                valid.editor = control;
                valid.maxLength = maxLength;
                valid.IsRequired = required;
                valid.ErrorType = ErrorType.Warning;
                dxValidationProviderEditorInfo.SetValidationRule(control, valid);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ValidForm()
		{
			try
			{
                ValidationMaxLength(this.txtName, 100, true);
                ValidationMaxLength(this.txtPayFormName, 100, false);
            }
			catch (Exception ex)
			{
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
		}

        #region Shortcut
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






        private void bbtnFocusDefault_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                txtKeyword.Focus();
                txtKeyword.SelectAll();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        #endregion

        private void Lock_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            CommonParam param = new CommonParam();
            HIS_PAY_FORM success = new HIS_PAY_FORM();
            //bool notHandler = false;
            try
            {

                HIS_PAY_FORM data = (HIS_PAY_FORM)gridviewFormList.GetFocusedRow();
                if (MessageBox.Show(LibraryMessage.MessageUtil.GetMessage(LibraryMessage.Message.Enum.HeThongTBCuaSoThongBaoBanCoMuonBoKhoaDuLieuKhong), "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    HIS_CASHIER_ROOM data1 = new HIS_CASHIER_ROOM();
                    data1.ID = data.ID;
                    WaitingManager.Show();
                    success = new Inventec.Common.Adapter.BackendAdapter(param).Post<HIS_PAY_FORM>("/api/HisPayForm/ChangeLock", ApiConsumers.MosConsumer, data1, param);
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

        private void unLock_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            CommonParam param = new CommonParam();
            HIS_PAY_FORM success = new HIS_PAY_FORM();
            //bool notHandler = false;
            try
            {

                HIS_PAY_FORM data = (HIS_PAY_FORM)gridviewFormList.GetFocusedRow();
                if (MessageBox.Show(LibraryMessage.MessageUtil.GetMessage(LibraryMessage.Message.Enum.HeThongTBCuaSoThongBaoBanCoMuonKhoaDuLieuKhong), "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    HIS_PAY_FORM data1 = new HIS_PAY_FORM();
                    data1.ID = data.ID;
                    WaitingManager.Show();
                    success = new Inventec.Common.Adapter.BackendAdapter(param).Post<HIS_PAY_FORM>("/api/HisPayForm/ChangeLock", ApiConsumers.MosConsumer, data1, param);
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

        private void gridviewFormList_RowCellStyle(object sender, DevExpress.XtraGrid.Views.Grid.RowCellStyleEventArgs e)
        {
            try
            {
                DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                if (e.RowHandle >= 0)
                {
                    HIS_PAY_FORM data = (HIS_PAY_FORM)((IList)((BaseView)sender).DataSource)[e.RowHandle];
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

        private void gridviewFormList_CustomRowCellEdit(object sender, DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs e)
        {
            try
            {
                DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                if (e.RowHandle >= 0)
                {

                    HIS_PAY_FORM data = (HIS_PAY_FORM)((IList)((BaseView)sender).DataSource)[e.RowHandle];
                    if (e.Column.FieldName == "IS_LOCK")
                    {
                        e.RepositoryItem = (data.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE ? unLock : Lock);

                    }

                    if (e.Column.FieldName == "Delete")
                    {
                        e.RepositoryItem = (data.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE ? btnGEdit : DeleteDisable);

                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void lkRoomId_KeyUp(object sender, KeyEventArgs e)
        {

        }

        private void chkPause_KeyUp(object sender, KeyEventArgs e)
        {
            /*try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (this.ActionType == GlobalVariables.ActionAdd)
                    {
                        btnAdd.Focus();
                    }
                    else
                        btnEdit.Focus();
                }
                if (e.KeyCode == Keys.Space)
                {
                    chkPause.Checked = !chkPause.Checked;
                }
                e.Handled = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSession.Warn(ex);
            }*/
        }


        #region ---Combo Area---
        private void cboHisArea_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {

        }

        private void cboHisArea_EditValueChanged(object sender, EventArgs e)
        {

        }

		#endregion
    
		private void gridviewFormList_RowCellClick(object sender, DevExpress.XtraGrid.Views.Grid.RowCellClickEventArgs e)
		{
			try
			{
                currrentData = (MOS.EFMODEL.DataModels.HIS_PAY_FORM)gridviewFormList.GetFocusedRow();
                if(currrentData != null)
				{
                    txtCode.Text = currrentData.PAY_FORM_CODE;
                    txtName.Text = currrentData.PAY_FORM_NAME;
                    txtPayFormName.Text = currrentData.ELECTRONIC_PAY_FORM_NAME;
                    btnSave.Enabled = true;

                }                    
			}
			catch (Exception ex)
			{
                Inventec.Common.Logging.LogSession.Warn(ex);
            }
		}

		private void btnSave_Click(object sender, EventArgs e)
		{
            try
            {
                if (!dxValidationProviderEditorInfo.Validate())
                    return;
                SaveProcess();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSession.Warn(ex);
            }
        }

        private void SaveProcess()
        {
            CommonParam param = new CommonParam();

            try
            {
                bool success = false;


                WaitingManager.Show();
                currrentData.PAY_FORM_NAME = txtName.Text;
                currrentData.ELECTRONIC_PAY_FORM_NAME = txtPayFormName.Text;

                     var resultData = new BackendAdapter(param).Post<MOS.EFMODEL.DataModels.HIS_PAY_FORM>("api/HisPayForm/Update", ApiConsumers.MosConsumer, currrentData, param);
                    if (resultData != null)
                    {
                        success = true;
                    FillDataToGridControl();
                    btnReset_Click(null,null);
                    SetDefaultFocus();
                    }
               
                    SetFocusEditor();
               
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

        private void btnReset_Click(object sender, EventArgs e)
		{
			try
			{
                txtCode.Text = txtName.Text = txtPayFormName.Text = String.Empty;
                btnSave.Enabled = false;
                Inventec.Desktop.Controls.ControlWorker.ValidationProviderRemoveControlError(this.dxValidationProviderEditorInfo, this.dxErrorProvider);
            }
			catch (Exception ex)
			{
                Inventec.Common.Logging.LogSession.Warn(ex);
            }
		}


		private void txtName_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
		{
			try
			{
                if (e.KeyCode == Keys.Enter)
                    txtPayFormName.Focus();
			}
			catch (Exception ex)
			{
                Inventec.Common.Logging.LogSession.Warn(ex);
            }
		}

		private void txtPayFormName_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
		{
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (btnSave.Enabled)
                        btnSave.Focus();
                    else
                        btnReset.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSession.Warn(ex);
            }
        }

		private void timer1_Tick()
		{
            try
            {
                SetDefaultFocus();
                StopTimer(GetModuleLink(), "timer1");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSession.Warn(ex);
            }
        }

		private void barButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
		{
            if (btnSave.Enabled)
                btnSave_Click(null, null);
        }

		private void barButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
		{
            btnReset_Click(null, null);
        }
	}
}
