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
using Inventec.Common.Adapter;
using Inventec.Common.Controls.EditorLoader;
using Inventec.Common.Logging;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using Inventec.UC.Paging;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Controls.Session;
using HIS.Desktop.LibraryMessage;
using HIS.Desktop.LocalStorage.ConfigApplication;
using HIS.Desktop.LocalStorage.LocalData;
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
using Inventec.Desktop.Common.LanguageManager;
using System.Resources;
using HIS.Desktop.Plugins.HisSubclinicalRsAdd.Validtion;
using HIS.Desktop.LocalStorage.BackendData;

namespace HIS.Desktop.Plugins.HisSubclinicalRsAdd.HisSubclinicalRsAdd
{
    public partial class frmHisSubclinicalRsAdd : HIS.Desktop.Utility.FormBase
    {
        #region Declare
        int rowCount = 0;
        int dataTotal = 0;
        int startPage = 0;
        PagingGrid pagingGrid;
        int ActionType = -1;
        int positionHandle = -1;
        MOS.EFMODEL.DataModels.V_HIS_SUBCLINICAL_RS_ADD currentData;
        List<string> arrControlEnableNotChange = new List<string>();
        Dictionary<string, int> dicOrderTabIndexControl = new Dictionary<string, int>();
        Inventec.Desktop.Common.Modules.Module moduleData;
        #endregion

        #region Construct
        public frmHisSubclinicalRsAdd(Inventec.Desktop.Common.Modules.Module moduleData)
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
                MeShow();
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
                Resources.ResourceLanguageManager.LanguageResource = new ResourceManager("HIS.Desktop.Plugins.HisSubclinicalRsAdd.Resources.Lang", typeof(HIS.Desktop.Plugins.HisSubclinicalRsAdd.HisSubclinicalRsAdd.frmHisSubclinicalRsAdd).Assembly);

                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.layoutControl4.Text = Inventec.Common.Resource.Get.Value("frmHisSupclinicalRsAdo.layoutControl4.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl7.Text = Inventec.Common.Resource.Get.Value("frmHisSupclinicalRsAdo.layoutControl7.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnSearch.Text = Inventec.Common.Resource.Get.Value("frmHisSupclinicalRsAdo.btnSearch.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.STT.Caption = Inventec.Common.Resource.Get.Value("frmHisSupclinicalRsAdo.STT.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumnEdit.ToolTip = Inventec.Common.Resource.Get.Value("frmHisSupclinicalRsAdo.gridColumnEdit.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColCashierRoomCode.Caption = Inventec.Common.Resource.Get.Value("frmHisSupclinicalRsAdo.grdColCashierRoomCode.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColCashierRoomCode.ToolTip = Inventec.Common.Resource.Get.Value("frmHisSupclinicalRsAdo.grdColCashierRoomCode.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColCashierRoomName.Caption = Inventec.Common.Resource.Get.Value("frmHisSupclinicalRsAdo.grdColCashierRoomName.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColCashierRoomName.ToolTip = Inventec.Common.Resource.Get.Value("frmHisSupclinicalRsAdo.grdColCashierRoomName.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn6.Caption = Inventec.Common.Resource.Get.Value("frmHisSupclinicalRsAdo.gridColumn6.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn1.Caption = Inventec.Common.Resource.Get.Value("frmHisSupclinicalRsAdo.gridColumn1.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn1.ToolTip = Inventec.Common.Resource.Get.Value("frmHisSupclinicalRsAdo.gridColumn1.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumnRoomName.Caption = Inventec.Common.Resource.Get.Value("frmHisSupclinicalRsAdo.gridColumnRoomName.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn2.Caption = Inventec.Common.Resource.Get.Value("frmHisSupclinicalRsAdo.gridColumn2.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn7.Caption = Inventec.Common.Resource.Get.Value("frmHisSupclinicalRsAdo.gridColumn7.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn8.Caption = Inventec.Common.Resource.Get.Value("frmHisSupclinicalRsAdo.gridColumn8.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn3.Caption = Inventec.Common.Resource.Get.Value("frmHisSupclinicalRsAdo.gridColumn3.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn3.ToolTip = Inventec.Common.Resource.Get.Value("frmHisSupclinicalRsAdo.gridColumn3.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn5.Caption = Inventec.Common.Resource.Get.Value("frmHisSupclinicalRsAdo.gridColumn5.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn5.ToolTip = Inventec.Common.Resource.Get.Value("frmHisSupclinicalRsAdo.gridColumn5.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColCreateTime.Caption = Inventec.Common.Resource.Get.Value("frmHisSupclinicalRsAdo.grdColCreateTime.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColCreateTime.ToolTip = Inventec.Common.Resource.Get.Value("frmHisSupclinicalRsAdo.grdColCreateTime.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColCreator.Caption = Inventec.Common.Resource.Get.Value("frmHisSupclinicalRsAdo.grdColCreator.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColCreator.ToolTip = Inventec.Common.Resource.Get.Value("frmHisSupclinicalRsAdo.grdColCreator.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColModifyTime.Caption = Inventec.Common.Resource.Get.Value("frmHisSupclinicalRsAdo.grdColModifyTime.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColModifyTime.ToolTip = Inventec.Common.Resource.Get.Value("frmHisSupclinicalRsAdo.grdColModifyTime.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColModifier.Caption = Inventec.Common.Resource.Get.Value("frmHisSupclinicalRsAdo.grdColModifier.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColModifier.ToolTip = Inventec.Common.Resource.Get.Value("frmHisSupclinicalRsAdo.grdColModifier.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.txtKeyword.Properties.NullValuePrompt = Inventec.Common.Resource.Get.Value("frmHisSupclinicalRsAdo.txtKeyword.Properties.NullValuePrompt", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lcEditorInfo.Text = Inventec.Common.Resource.Get.Value("frmHisSupclinicalRsAdo.lcEditorInfo.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bar1.Text = Inventec.Common.Resource.Get.Value("frmHisSupclinicalRsAdo.bar1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bbtnSearch.Caption = Inventec.Common.Resource.Get.Value("frmHisSupclinicalRsAdo.bbtnSearch.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bbtnEdit.Caption = Inventec.Common.Resource.Get.Value("frmHisSupclinicalRsAdo.bbtnEdit.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bbtnAdd.Caption = Inventec.Common.Resource.Get.Value("frmHisSupclinicalRsAdo.bbtnAdd.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bbtnReset.Caption = Inventec.Common.Resource.Get.Value("frmHisSupclinicalRsAdo.bbtnReset.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bbtnFocusDefault.Caption = Inventec.Common.Resource.Get.Value("frmHisSupclinicalRsAdo.bbtnFocusDefault.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboResultDesk.Properties.NullText = Inventec.Common.Resource.Get.Value("frmHisSupclinicalRsAdo.cboResultDesk.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboResultRoom.Properties.NullText = Inventec.Common.Resource.Get.Value("frmHisSupclinicalRsAdo.cboResultRoom.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboExecuteRoom.Properties.NullText = Inventec.Common.Resource.Get.Value("frmHisSupclinicalRsAdo.cboExecuteRoom.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboRequestRoom.Properties.NullText = Inventec.Common.Resource.Get.Value("frmHisSupclinicalRsAdo.cboRequestRoom.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnRefresh.Text = Inventec.Common.Resource.Get.Value("frmHisSupclinicalRsAdo.btnRefresh.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnAdd.Text = Inventec.Common.Resource.Get.Value("frmHisSupclinicalRsAdo.btnAdd.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnEdit.Text = Inventec.Common.Resource.Get.Value("frmHisSupclinicalRsAdo.btnEdit.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.dnNavigation.Text = Inventec.Common.Resource.Get.Value("frmHisSupclinicalRsAdo.dnNavigation.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem10.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("frmHisSupclinicalRsAdo.layoutControlItem10.OptionsToolTip.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem10.Text = Inventec.Common.Resource.Get.Value("frmHisSupclinicalRsAdo.layoutControlItem10.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem12.Text = Inventec.Common.Resource.Get.Value("frmHisSupclinicalRsAdo.layoutControlItem12.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem16.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("frmHisSupclinicalRsAdo.layoutControlItem16.OptionsToolTip.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem16.Text = Inventec.Common.Resource.Get.Value("frmHisSupclinicalRsAdo.layoutControlItem16.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem17.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("frmHisSupclinicalRsAdo.layoutControlItem17.OptionsToolTip.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem17.Text = Inventec.Common.Resource.Get.Value("frmHisSupclinicalRsAdo.layoutControlItem17.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem22.Text = Inventec.Common.Resource.Get.Value("frmHisSupclinicalRsAdo.layoutControlItem22.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem23.Text = Inventec.Common.Resource.Get.Value("frmHisSupclinicalRsAdo.layoutControlItem23.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem24.Text = Inventec.Common.Resource.Get.Value("frmHisSupclinicalRsAdo.layoutControlItem24.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem14.Text = Inventec.Common.Resource.Get.Value("frmHisSupclinicalRsAdo.layoutControlItem14.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
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


        private void SetDefaultValue()
        {
            try
            {

                this.ActionType = GlobalVariables.ActionAdd;
                txtKeyword.Text = "";
                ResetFormData();
                EnableControlChanged(this.ActionType);
                dxValidationProviderEditorInfo.RemoveControlError(cboRequestRoom);
                dxValidationProviderEditorInfo.RemoveControlError(cboInstructionDateFrom);
                dxValidationProviderEditorInfo.RemoveControlError(tmIntructionTimeFrom);

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
                txtKeyword.Focus();
                txtKeyword.SelectAll();
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


        void ValidMaxlengthTextBox(TextEdit txtEdit, int? maxLength)
        {
            try
            {
                ValidateMaxLength validateMaxLength = new ValidateMaxLength();
                validateMaxLength.textEdit = txtEdit;
                validateMaxLength.maxLength = maxLength;
                dxValidationProviderEditorInfo.SetValidationRule(txtEdit, validateMaxLength);
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

        private void FillDataToControlsForm()
        {
            try
            {
                InitComboRequestRoom();
                InitComboExecuteRoom();
                InitComboResultRoom();
                InitComboResultDesk();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        #region Init combo
        private void InitComboRequestRoom()
        {
            try
            {
                CommonParam param = new CommonParam();
                var data = BackendDataWorker.Get<V_HIS_ROOM>().Where(o => o.IS_ACTIVE == 1).ToList();
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("ROOM_CODE", "", 100, 1));
                columnInfos.Add(new ColumnInfo("ROOM_NAME", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("ROOM_NAME", "ID", columnInfos, false, 350);
                ControlEditorLoader.Load(cboRequestRoom, data, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitComboExecuteRoom()
        {
            try
            {
                CommonParam param = new CommonParam();
                var data = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<V_HIS_ROOM>().Where(o => o.IS_ACTIVE == 1).ToList();
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("ROOM_CODE", "", 100, 1));
                columnInfos.Add(new ColumnInfo("ROOM_NAME", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("ROOM_NAME", "ID", columnInfos, false, 350);
                ControlEditorLoader.Load(cboExecuteRoom, data, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitComboResultRoom()
        {
            try
            {
                CommonParam param = new CommonParam();
                var data = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<V_HIS_ROOM>().Where(o => o.IS_ACTIVE == 1).ToList();
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("ROOM_CODE", "", 100, 1));
                columnInfos.Add(new ColumnInfo("ROOM_NAME", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("ROOM_NAME", "ID", columnInfos, false, 350);
                ControlEditorLoader.Load(cboResultRoom, data, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitComboResultDesk()
        {
            try
            {
                CommonParam param = new CommonParam();
                var data = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_DESK>().Where(o => o.IS_ACTIVE == 1).ToList();
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("DESK_CODE", "", 100, 1));
                columnInfos.Add(new ColumnInfo("DESK_NAME", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("DESK_NAME", "ID", columnInfos, false, 350);
                ControlEditorLoader.Load(cboResultDesk, data, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
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
                Inventec.Core.ApiResultObject<List<MOS.EFMODEL.DataModels.V_HIS_SUBCLINICAL_RS_ADD>> apiResult = null;
                HisSubclinicalRsAddViewFilter filter = new HisSubclinicalRsAddViewFilter();
                SetFilterNavBar(ref filter);

                filter.ORDER_DIRECTION = "DESC";
                filter.ORDER_FIELD = "MODIFY_TIME";
                dnNavigation.DataSource = null;
                gridviewFormList.BeginUpdate();
                apiResult = new BackendAdapter(paramCommon).GetRO<List<MOS.EFMODEL.DataModels.V_HIS_SUBCLINICAL_RS_ADD>>(HisRequestUriStore.MOSHIS_CASHIER_ROOM_GETVIEW, ApiConsumers.MosConsumer, filter, paramCommon);
                if (apiResult != null)
                {
                    var data = (List<MOS.EFMODEL.DataModels.V_HIS_SUBCLINICAL_RS_ADD>)apiResult.Data;
                    if (data != null)
                    {
                        dnNavigation.DataSource = data;
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

        private void SetFilterNavBar(ref HisSubclinicalRsAddViewFilter filter)
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
                    var rowData = (MOS.EFMODEL.DataModels.V_HIS_SUBCLINICAL_RS_ADD)gridviewFormList.GetFocusedRow();
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
                    MOS.EFMODEL.DataModels.V_HIS_SUBCLINICAL_RS_ADD pData = (MOS.EFMODEL.DataModels.V_HIS_SUBCLINICAL_RS_ADD)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
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
                    else if (e.Column.FieldName == "INSTR_TIME_FROM_DISPLAY")
                    {
                        e.Value = !String.IsNullOrWhiteSpace(pData.INSTR_TIME_FROM) && pData.INSTR_TIME_FROM.Length == 4 ? String.Format("{0}:{1}", pData.INSTR_TIME_FROM.Substring(0, 2), pData.INSTR_TIME_FROM.Substring(2, 2)) : "";
                    }

                    else if (e.Column.FieldName == "INSTR_TIME_TO_DISPLAY")
                    {
                        e.Value = !String.IsNullOrWhiteSpace(pData.INSTR_TIME_TO) && pData.INSTR_TIME_TO.Length == 4 ? String.Format("{0}:{1}", pData.INSTR_TIME_TO.Substring(0, 2), pData.INSTR_TIME_TO.Substring(2, 2)) : "";
                    }
                    else if (e.Column.FieldName == "INSTR_DAY_FROM_DISPLAY")
                    {
                        e.Value = ConvertTimeNumberToTimeString(pData.INSTR_DAY_FROM ?? 0);
                    }
                    else if (e.Column.FieldName == "INSTR_DAY_TO_DISPLAY")
                    {
                        e.Value = ConvertTimeNumberToTimeString(pData.INSTR_DAY_TO ?? 0);
                    }
                    gridControlFormList.RefreshDataSource();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private string ConvertTimeNumberToTimeString(long number)
        {
            string result = "";
            try
            {
                switch (number)
                {
                    case 1:
                        result = "Chủ nhật";
                        break;
                    case 2:
                        result = "Thứ 2";
                        break;
                    case 3:
                        result = "Thứ 3";
                        break;
                    case 4:
                        result = "Thứ 4";
                        break;
                    case 5:
                        result = "Thứ 5";
                        break;
                    case 6:
                        result = "Thứ 6";
                        break;
                    case 7:
                        result = "Thứ 7";
                        break;
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        private void gridControlFormList_DoubleClick(object sender, EventArgs e)
        {
            try
            {
                var rowData = (MOS.EFMODEL.DataModels.V_HIS_SUBCLINICAL_RS_ADD)gridviewFormList.GetFocusedRow();
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
                    var rowData = (MOS.EFMODEL.DataModels.V_HIS_SUBCLINICAL_RS_ADD)gridviewFormList.GetFocusedRow();
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

        private void dnNavigation_PositionChanged(object sender, EventArgs e)
        {
            try
            {
                this.currentData = (MOS.EFMODEL.DataModels.V_HIS_SUBCLINICAL_RS_ADD)(gridControlFormList.DataSource as List<MOS.EFMODEL.DataModels.V_HIS_SUBCLINICAL_RS_ADD>)[dnNavigation.Position];
                if (this.currentData != null)
                {
                    ChangedDataRow(this.currentData);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ChangedDataRow(MOS.EFMODEL.DataModels.V_HIS_SUBCLINICAL_RS_ADD data)
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
                    Inventec.Desktop.Controls.ControlWorker.ValidationProviderRemoveControlError(dxValidationProviderEditorInfo, dxErrorProvider);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private long ConvertTimeStringToLong(string time)
        {
            long result = 0;
            try
            {
                switch (time)
                {
                    case "Chủ nhật":
                        result = 1;
                        break;
                    case "Thứ 2":
                        result = 2;
                        break;
                    case "Thứ 3":
                        result = 3;
                        break;
                    case "Thứ 4":
                        result = 4;
                        break;
                    case "Thứ 5":
                        result = 5;
                        break;
                    case "Thứ 6":
                        result = 6;
                        break;
                    case "Thứ 7":
                        result = 7;
                        break;
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        private void FillDataToEditorControl(MOS.EFMODEL.DataModels.V_HIS_SUBCLINICAL_RS_ADD data)
        {
            try
            {
                if (data != null)
                {
                    cboRequestRoom.EditValue = data.REQUEST_ROOM_ID;
                    txtRequestRoomCode.Text = data.REQUEST_ROOM_CODE;
                    txtExecuteRoomCode.Text = data.EXECUTE_ROOM_CODE;
                    cboExecuteRoom.EditValue = data.EXECUTE_ROOM_ID;
                    cboInstructionDateFrom.EditValue = ConvertTimeNumberToTimeString(data.INSTR_DAY_FROM ?? 0);
                    cboInstructionDateTo.EditValue = ConvertTimeNumberToTimeString(data.INSTR_DAY_TO ?? 0);
                    if (!String.IsNullOrWhiteSpace(data.INSTR_TIME_FROM) && data.INSTR_TIME_FROM.Length == 4)
                        tmIntructionTimeFrom.EditValue = String.Format("{0:00}:{1:00}", data.INSTR_TIME_FROM.Substring(0, 2), data.INSTR_TIME_FROM.Substring(2, 2));
                    else
                        tmIntructionTimeFrom.EditValue = null;

                    if (!String.IsNullOrWhiteSpace(data.INSTR_TIME_TO) && data.INSTR_TIME_TO.Length == 4)
                        tmIntructionTimeTo.EditValue = String.Format("{0:00}:{1:00}", data.INSTR_TIME_TO.Substring(0, 2), data.INSTR_TIME_TO.Substring(2, 2));
                    else
                        tmIntructionTimeTo.EditValue = null;

                    txtResultRoomCode.Text = data.RESULT_ROOM_CODE;
                    cboResultRoom.EditValue = data.RESULT_ROOM_ID;
                    txtDeskCode.Text = data.RESULT_DESK_CODE;
                    cboResultDesk.EditValue = data.RESULT_DESK_ID;
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
                //TODO

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Debug(ex);
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
                        if (lci != null && lci.Control != null && lci.Control is BaseEdit)
                        {
                            DevExpress.XtraEditors.BaseEdit fomatFrm = lci.Control as DevExpress.XtraEditors.BaseEdit;
                            cboRequestRoom.Focus();
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
                    lcEditorInfo.EndUpdate();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadCurrent(long currentId, ref MOS.EFMODEL.DataModels.HIS_SUBCLINICAL_RS_ADD currentDTO)
        {
            try
            {
                CommonParam param = new CommonParam();
                HisSubclinicalRsAddFilter filter = new HisSubclinicalRsAddFilter();
                filter.ID = currentId;
                currentDTO = new BackendAdapter(param).Get<List<MOS.EFMODEL.DataModels.HIS_SUBCLINICAL_RS_ADD>>(HisRequestUriStore.MOSHIS_CASHIER_ROOM_GET, ApiConsumers.MosConsumer, filter, param).FirstOrDefault();
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

        private void btnRefesh_Click(object sender, EventArgs e)
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

        private void btnGDelete_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                var rowData = (MOS.EFMODEL.DataModels.V_HIS_SUBCLINICAL_RS_ADD)gridviewFormList.GetFocusedRow();
                CommonParam param = new CommonParam();
                if (rowData != null)
                {
                    if (MessageBox.Show(LibraryMessage.MessageUtil.GetMessage(LibraryMessage.Message.Enum.HeThongTBCuaSoThongBaoBanCoMuonXoaDuLieuKhong), "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        bool success = false;
                        success = new BackendAdapter(param).Post<bool>(HisRequestUriStore.MOSHIS_CASHIER_ROOM_DELETE, ApiConsumers.MosConsumer, rowData.ID, param);
                        if (success)
                        {
                            FillDataToGridControl();
                            currentData = ((List<V_HIS_SUBCLINICAL_RS_ADD>)gridControlFormList.DataSource).FirstOrDefault();
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

        private void btnCancel_Click(object sender, EventArgs e)
        {

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

        private void SaveProcess()
        {
            CommonParam param = new CommonParam();
            try
            {
                bool success = false;
                if (!btnEdit.Enabled && !btnAdd.Enabled)
                    return;

                if (cboRequestRoom.EditValue == null && cboExecuteRoom.EditValue == null)
                {
                    MessageBox.Show("Bắt buộc chọn phòng yêu cầu hoặc phòng xử lý", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtRequestRoomCode.Focus();
                    return;
                }

                if (cboResultDesk.EditValue == null && cboResultRoom.EditValue == null)
                {
                    MessageBox.Show("Bắt buộc chọn phòng trả kết quả hoặc bàn trả kết qủa", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtResultRoomCode.Focus();
                    return;
                }

                positionHandle = -1;
                if (!dxValidationProviderEditorInfo.Validate())
                    return;

                WaitingManager.Show();
                MOS.EFMODEL.DataModels.HIS_SUBCLINICAL_RS_ADD updateDTO = new MOS.EFMODEL.DataModels.HIS_SUBCLINICAL_RS_ADD();

                if (this.currentData != null && this.currentData.ID > 0)
                {
                    LoadCurrent(this.currentData.ID, ref updateDTO);
                }

                UpdateDTOFromDataForm(ref updateDTO);

                if (ActionType == GlobalVariables.ActionAdd)
                {
                    updateDTO.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                    var resultData = new BackendAdapter(param).Post<MOS.EFMODEL.DataModels.HIS_SUBCLINICAL_RS_ADD>(HisRequestUriStore.MOSHIS_CASHIER_ROOM_CREATE, ApiConsumers.MosConsumer, updateDTO, param);
                    if (resultData != null)
                    {
                        success = true;
                        FillDataToGridControl();
                        ResetFormData();
                    }
                }
                else
                {
                    var resultData = new BackendAdapter(param).Post<MOS.EFMODEL.DataModels.HIS_SUBCLINICAL_RS_ADD>(HisRequestUriStore.MOSHIS_CASHIER_ROOM_UPDATE, ApiConsumers.MosConsumer, updateDTO, param);
                    if (resultData != null)
                    {
                        success = true;
                        //UpdateRowDataAfterEdit(resultData);
                        FillDataToGridControl();
                        //ResetFormData();
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

        private void UpdateRowDataAfterEdit(MOS.EFMODEL.DataModels.HIS_SUBCLINICAL_RS_ADD data)
        {
            try
            {
                if (data == null)
                    throw new ArgumentNullException("data(MOS.EFMODEL.DataModels.V_HIS_SUBCLINICAL_RS_ADD) is null");
                var rowData = (MOS.EFMODEL.DataModels.V_HIS_SUBCLINICAL_RS_ADD)gridviewFormList.GetFocusedRow();
                if (rowData != null)
                {
                    Inventec.Common.Mapper.DataObjectMapper.Map<MOS.EFMODEL.DataModels.V_HIS_SUBCLINICAL_RS_ADD>(rowData, data);
                    gridviewFormList.RefreshRow(gridviewFormList.FocusedRowHandle);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void UpdateDTOFromDataForm(ref MOS.EFMODEL.DataModels.HIS_SUBCLINICAL_RS_ADD currentDTO)
        {
            try
            {
                if (cboRequestRoom.EditValue != null)
                    currentDTO.REQUEST_ROOM_ID = Inventec.Common.TypeConvert.Parse.ToInt64((cboRequestRoom.EditValue ?? "0").ToString());
                else
                    currentDTO.REQUEST_ROOM_ID = null;

                if (cboExecuteRoom.EditValue != null)
                    currentDTO.EXECUTE_ROOM_ID = Inventec.Common.TypeConvert.Parse.ToInt64((cboExecuteRoom.EditValue ?? "0").ToString());
                else
                    currentDTO.EXECUTE_ROOM_ID = null;

                if (cboResultDesk.EditValue != null)
                    currentDTO.RESULT_DESK_ID = Inventec.Common.TypeConvert.Parse.ToInt64((cboResultDesk.EditValue ?? "0").ToString());
                else
                    currentDTO.RESULT_DESK_ID = null;

                if (cboResultRoom.EditValue != null)
                    currentDTO.RESULT_ROOM_ID = Inventec.Common.TypeConvert.Parse.ToInt64((cboResultRoom.EditValue ?? "0").ToString());
                else
                    currentDTO.RESULT_ROOM_ID = null;

                if (cboInstructionDateFrom.SelectedIndex >= 0)
                {
                    currentDTO.INSTR_DAY_FROM = ConvertTimeStringToLong((cboInstructionDateFrom.EditValue ?? "0").ToString());
                }
                else
                    currentDTO.INSTR_DAY_FROM = null;

                if (cboInstructionDateTo.SelectedIndex >= 0)
                {
                    currentDTO.INSTR_DAY_TO = ConvertTimeStringToLong((cboInstructionDateTo.EditValue ?? "0").ToString());
                }
                else
                {
                    currentDTO.INSTR_DAY_TO = null;
                }

                tmIntructionTimeFrom.DeselectAll();// lấy giá trị mới nhất khi dùng phím tắt
                if (tmIntructionTimeFrom.EditValue != null)
                    currentDTO.INSTR_TIME_FROM = String.Format("{0:00}{1:00}", tmIntructionTimeFrom.TimeSpan.Hours, tmIntructionTimeFrom.TimeSpan.Minutes);
                else
                    currentDTO.INSTR_TIME_FROM = null;

                tmIntructionTimeTo.DeselectAll();
                if (tmIntructionTimeTo.EditValue != null)
                    currentDTO.INSTR_TIME_TO = String.Format("{0:00}{1:00}", tmIntructionTimeTo.TimeSpan.Hours, tmIntructionTimeTo.TimeSpan.Minutes);
                else
                    currentDTO.INSTR_TIME_TO = null;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        #endregion

        #region Validate
        private void ValidateForm()
        {
            try
            {
                //ValidationSingleControl(cboRequestRoom);
                //ValidMaxlengthTextBox(txtDeskName, 100);
                ValidFromDateAndToDate();
                ValidFromTimeAndToTime();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        void ValidFromDateAndToDate()
        {
            try
            {
                ValidateDateFromAndDateTo validateTimeFromAndTimeTo = new ValidateDateFromAndDateTo();
                validateTimeFromAndTimeTo.cboFrom = cboInstructionDateFrom;
                validateTimeFromAndTimeTo.cboTo = cboInstructionDateTo;
                dxValidationProviderEditorInfo.SetValidationRule(cboInstructionDateFrom, validateTimeFromAndTimeTo);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        void ValidFromTimeAndToTime()
        {
            try
            {
                ValidateTimeFromAndTimeTo validateTimeFromAndTimeTo = new ValidateTimeFromAndTimeTo();
                validateTimeFromAndTimeTo.spanFrom = tmIntructionTimeFrom;
                validateTimeFromAndTimeTo.spanTo = tmIntructionTimeTo;
                dxValidationProviderEditorInfo.SetValidationRule(tmIntructionTimeFrom, validateTimeFromAndTimeTo);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

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

        #endregion

        #region Public method
        public void MeShow()
        {
            try
            {
                //Gan gia tri mac dinh
                SetDefaultValue();

                //Set enable control default
                EnableControlChanged(this.ActionType);

                //Fill data into datasource combo
                FillDataToControlsForm();

                //Load du lieu
                FillDataToGridControl();

                //Load ngon ngu label control
                SetCaptionByLanguageKey();

                //Set tabindex control
                InitTabIndex();

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
        #endregion

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

        private void bbtnEdit_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                if (this.ActionType == GlobalVariables.ActionEdit && btnEdit.Enabled)
                {
                    btnEdit_Click(null, null);
                }
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

        private void bbtnReset_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                btnRefesh_Click(null, null);
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
            HIS_SUBCLINICAL_RS_ADD result = new HIS_SUBCLINICAL_RS_ADD();
            bool success = false;
            try
            {

                V_HIS_SUBCLINICAL_RS_ADD data = (V_HIS_SUBCLINICAL_RS_ADD)gridviewFormList.GetFocusedRow();
                WaitingManager.Show();
                result = new Inventec.Common.Adapter.BackendAdapter(param).Post<HIS_SUBCLINICAL_RS_ADD>(HisRequestUriStore.MOSHIS_CASHIER_ROOM_CHANGELOCK, ApiConsumers.MosConsumer, data.ID, param);
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

        private void unLock_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            CommonParam param = new CommonParam();
            HIS_SUBCLINICAL_RS_ADD result = new HIS_SUBCLINICAL_RS_ADD();
            bool success = false;
            //bool notHandler = false;
            try
            {

                V_HIS_SUBCLINICAL_RS_ADD data = (V_HIS_SUBCLINICAL_RS_ADD)gridviewFormList.GetFocusedRow();
                WaitingManager.Show();
                result = new Inventec.Common.Adapter.BackendAdapter(param).Post<HIS_SUBCLINICAL_RS_ADD>(HisRequestUriStore.MOSHIS_CASHIER_ROOM_CHANGELOCK, ApiConsumers.MosConsumer, data.ID, param);
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

        private void gridviewFormList_RowCellStyle(object sender, DevExpress.XtraGrid.Views.Grid.RowCellStyleEventArgs e)
        {
            try
            {
                DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                if (e.RowHandle >= 0)
                {
                    V_HIS_SUBCLINICAL_RS_ADD data = (V_HIS_SUBCLINICAL_RS_ADD)((IList)((BaseView)sender).DataSource)[e.RowHandle];
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

                    V_HIS_SUBCLINICAL_RS_ADD data = (V_HIS_SUBCLINICAL_RS_ADD)((IList)((BaseView)sender).DataSource)[e.RowHandle];
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


        #region ---Combo Area---
        private void cboHisArea_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {

        }

        private void cboHisArea_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == DevExpress.XtraEditors.Controls.ButtonPredefines.Delete)
                {
                }
            }
            catch (Exception ex)
            {

                LogSystem.Warn(ex);
            }
        }

        private void lkDepartmentId_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (cboRequestRoom.EditValue != null)
                {
                }
                else
                {
                }
            }
            catch (Exception ex)
            {

                LogSystem.Warn(ex);
            }
        }

        #endregion


        private void gridviewFormList_CellValueChanging(object sender, CellValueChangedEventArgs e)
        {

        }

        private void txtRequestRoomCode_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (string.IsNullOrEmpty(txtRequestRoomCode.Text))
                    {
                        cboRequestRoom.EditValue = null;
                        cboRequestRoom.Focus();
                        cboRequestRoom.ShowPopup();
                    }
                    else
                    {
                        List<V_HIS_ROOM> searchs = null;
                        var listData1 = BackendDataWorker.Get<V_HIS_ROOM>().Where(o => o.IS_ACTIVE == 1).Where(o => o.ROOM_CODE.ToUpper().Contains(txtRequestRoomCode.Text.ToUpper())).ToList();
                        if (listData1 != null && listData1.Count > 0)
                        {
                            searchs = (listData1.Count == 1) ? listData1 : (listData1.Where(o => o.ROOM_CODE.ToUpper() == txtRequestRoomCode.Text.ToUpper()).ToList());
                        }
                        if (searchs != null && searchs.Count == 1)
                        {
                            txtRequestRoomCode.Text = searchs[0].ROOM_CODE;
                            cboRequestRoom.EditValue = searchs[0].ID;
                            txtExecuteRoomCode.Focus();
                            txtExecuteRoomCode.SelectAll();
                        }
                        else
                        {
                            cboRequestRoom.Focus();
                            cboRequestRoom.ShowPopup();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboRequestRoom_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                try
                {
                    if (e.CloseMode == PopupCloseMode.Normal)
                    {
                        if (cboRequestRoom.EditValue != null)
                        {
                            var data = BackendDataWorker.Get<V_HIS_ROOM>().SingleOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64((cboRequestRoom.EditValue ?? "").ToString()));
                            if (data != null)
                            {
                                txtRequestRoomCode.Text = data.ROOM_CODE;
                                txtExecuteRoomCode.Focus();
                                txtExecuteRoomCode.SelectAll();
                            }
                        }
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

        private void txtExecuteRoomCode_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (string.IsNullOrEmpty(txtExecuteRoomCode.Text))
                    {
                        cboExecuteRoom.EditValue = null;
                        cboExecuteRoom.Focus();
                        cboExecuteRoom.ShowPopup();
                    }
                    else
                    {
                        List<V_HIS_ROOM> searchs = null;
                        var listData1 = BackendDataWorker.Get<V_HIS_ROOM>().Where(o => o.IS_ACTIVE == 1).Where(o => o.ROOM_CODE.ToUpper().Contains(txtExecuteRoomCode.Text.ToUpper())).ToList();
                        if (listData1 != null && listData1.Count > 0)
                        {
                            searchs = (listData1.Count == 1) ? listData1 : (listData1.Where(o => o.ROOM_CODE.ToUpper() == txtExecuteRoomCode.Text.ToUpper()).ToList());
                        }
                        if (searchs != null && searchs.Count == 1)
                        {
                            txtExecuteRoomCode.Text = searchs[0].ROOM_CODE;
                            cboExecuteRoom.EditValue = searchs[0].ID;
                            txtResultRoomCode.Focus();
                        }
                        else
                        {
                            cboExecuteRoom.Focus();
                            cboExecuteRoom.ShowPopup();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboExecuteRoom_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                try
                {
                    if (e.CloseMode == PopupCloseMode.Normal)
                    {
                        if (cboExecuteRoom.EditValue != null)
                        {
                            var data = BackendDataWorker.Get<V_HIS_ROOM>().SingleOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64((cboExecuteRoom.EditValue ?? "").ToString()));
                            if (data != null)
                            {
                                txtExecuteRoomCode.Text = data.ROOM_CODE;
                                txtResultRoomCode.Focus();
                            }
                        }
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

        private void txtResultRoomCode_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (string.IsNullOrEmpty(txtExecuteRoomCode.Text))
                    {
                        cboResultRoom.EditValue = null;
                        cboResultRoom.Focus();
                        cboResultRoom.ShowPopup();
                    }
                    else
                    {
                        List<V_HIS_ROOM> searchs = null;
                        var listData1 = BackendDataWorker.Get<V_HIS_ROOM>().Where(o => o.IS_ACTIVE == 1).Where(o => o.ROOM_CODE.ToUpper().Contains(txtResultRoomCode.Text.ToUpper())).ToList();
                        if (listData1 != null && listData1.Count > 0)
                        {
                            searchs = (listData1.Count == 1) ? listData1 : (listData1.Where(o => o.ROOM_CODE.ToUpper() == txtResultRoomCode.Text.ToUpper()).ToList());
                        }
                        if (searchs != null && searchs.Count == 1)
                        {
                            txtResultRoomCode.Text = searchs[0].ROOM_CODE;
                            cboResultRoom.EditValue = searchs[0].ID;
                            txtDeskCode.Focus();
                            txtDeskCode.SelectAll();
                        }
                        else
                        {
                            cboResultRoom.Focus();
                            cboResultRoom.ShowPopup();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboResultRoom_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {

            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    if (cboResultRoom.EditValue != null)
                    {
                        var data = BackendDataWorker.Get<V_HIS_ROOM>().SingleOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64((cboResultRoom.EditValue ?? "").ToString()));
                        if (data != null)
                        {
                            txtResultRoomCode.Text = data.ROOM_CODE;
                            txtDeskCode.Focus();
                            txtDeskCode.SelectAll();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtDeskCode_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {

        }

        private void cboResultDesk_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    if (cboResultDesk.EditValue != null)
                    {
                        var data = BackendDataWorker.Get<HIS_DESK>().SingleOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64((cboResultDesk.EditValue ?? "").ToString()));
                        if (data != null)
                        {
                            txtDeskCode.Text = data.DESK_CODE;
                            cboInstructionDateFrom.Focus();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboInstructionDateFrom_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    cboInstructionDateTo.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboInstructionDateTo_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    tmIntructionTimeFrom.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void tmIntructionTimeFrom_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {

        }

        private void tmIntructionTimeTo_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {

        }

        private void cboRequestRoom_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == DevExpress.XtraEditors.Controls.ButtonPredefines.Delete)
                {
                    cboRequestRoom.EditValue = null;
                    txtRequestRoomCode.Text = "";
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboExecuteRoom_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == DevExpress.XtraEditors.Controls.ButtonPredefines.Delete)
                {
                    cboExecuteRoom.EditValue = null;
                    txtExecuteRoomCode.Text = "";
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboResultRoom_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == DevExpress.XtraEditors.Controls.ButtonPredefines.Delete)
                {
                    cboResultRoom.EditValue = null;
                    txtResultRoomCode.Text = "";
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboResultDesk_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == DevExpress.XtraEditors.Controls.ButtonPredefines.Delete)
                {
                    cboResultDesk.EditValue = null;
                    txtDeskCode.Text = "";
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboRequestRoom_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtExecuteRoomCode.Focus();
                    txtExecuteRoomCode.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboExecuteRoom_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtResultRoomCode.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboResultRoom_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtDeskCode.Focus();
                    txtDeskCode.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboResultDesk_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    cboInstructionDateFrom.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void tmIntructionTimeTo_KeyUp(object sender, KeyEventArgs e)
        {

        }

        private void tmIntructionTimeFrom_KeyUp(object sender, KeyEventArgs e)
        {

        }

        private void tmIntructionTimeFrom_Enter(object sender, EventArgs e)
        {
            tmIntructionTimeTo.Focus();
        }

        private void tmIntructionTimeTo_Enter(object sender, EventArgs e)
        {
            txtResultRoomCode.Focus();
        }

        private void tmIntructionTimeTo_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                if (this.ActionType == GlobalVariables.ActionAdd)
                {
                    btnAdd.Focus();
                }
                else
                {
                    btnEdit.Focus();
                }
                e.Handled = true;
            }
        }

        private void tmIntructionTimeFrom_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                tmIntructionTimeTo.Focus();
                e.Handled = true;
            }
        }

        private void txtDeskCode_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (string.IsNullOrEmpty(txtRequestRoomCode.Text))
                    {
                        cboResultDesk.EditValue = null;
                        cboResultDesk.Focus();
                        cboResultDesk.ShowPopup();
                    }
                    else
                    {
                        List<HIS_DESK> searchs = null;
                        var listData1 = BackendDataWorker.Get<HIS_DESK>().Where(o => o.IS_ACTIVE == 1).Where(o => o.DESK_CODE.ToUpper().Contains(txtDeskCode.Text.ToUpper())).ToList();
                        if (listData1 != null && listData1.Count > 0)
                        {
                            searchs = (listData1.Count == 1) ? listData1 : (listData1.Where(o => o.DESK_CODE.ToUpper() == txtDeskCode.Text.ToUpper()).ToList());
                        }
                        if (searchs != null && searchs.Count == 1)
                        {
                            txtDeskCode.Text = searchs[0].DESK_CODE;
                            cboResultDesk.EditValue = searchs[0].ID;
                            cboInstructionDateFrom.Focus();

                        }
                        else
                        {
                            cboResultDesk.Focus();
                            cboResultDesk.ShowPopup();
                        }
                    }
                    e.Handled = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
