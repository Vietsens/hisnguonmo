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
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Inventec.Core;
using HIS.Desktop.LocalStorage.ConfigApplication;
using Inventec.Desktop.Common.Message;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using DevExpress.Data;
using System.Collections;
using DevExpress.XtraGrid.Views.Base;
using HIS.Desktop.LibraryMessage;
using System.Threading;
using System.IO;
using Inventec.Common.RichEditor.Base;
using HIS.Desktop.Common;
using HIS.Desktop.IsAdmin;
using HIS.Desktop.LocalStorage.BackendData;
using MOS.EFMODEL.DataModels;
using MOS.SDO;
using HIS.Desktop.Utilities.Extensions;
using DevExpress.XtraEditors;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Utility;
using HIS.Desktop.Plugins.AggrImpMestList.Base;

namespace HIS.Desktop.Plugins.AggrImpMestList
{
    public partial class UCAggrImpMestList : HIS.Desktop.Utility.UserControlBase
    {
        #region Declare
        int rowCount = 0;
        int dataTotal = 0;
        int startPage = 0;
        private string LoggingName = "";
        System.Globalization.CultureInfo cultureLang;
        MOS.EFMODEL.DataModels.V_HIS_MEDI_STOCK medistock;
        int lastRowHandle = -1;
        DevExpress.XtraGrid.Columns.GridColumn lastColumn = null;
        DevExpress.Utils.ToolTipControlInfo lastInfo;
        long roomId = 0;
        long roomTypeId = 0;
        MOS.EFMODEL.DataModels.V_HIS_ROOM _Room = null;
        List<HIS_DEPARTMENT> listDepartment_Selected;
        V_HIS_IMP_MEST currentImpMest;

        internal Inventec.Desktop.Common.Modules.Module currentModule;
        List<ACS.EFMODEL.DataModels.ACS_CONTROL> controlAcs;

        bool isCancelImport_Enable = false;
        #endregion

        #region Construct
        public UCAggrImpMestList()
        {
            InitializeComponent();
            try
            {
                Resources.ResourceLanguageManager.InitResourceLanguageManager();
                cultureLang = Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture();
                LoggingName = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
                gridControl.ToolTipController = this.toolTipController;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public UCAggrImpMestList(Inventec.Desktop.Common.Modules.Module module)
            : base(module)
        {
            InitializeComponent();
            try
            {
                Resources.ResourceLanguageManager.InitResourceLanguageManager();
                cultureLang = Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture();
                LoggingName = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
                gridControl.ToolTipController = this.toolTipController;
                medistock = Base.GlobalStore.ListMediStock.FirstOrDefault(o => o.ROOM_ID == module.RoomId && o.ROOM_TYPE_ID == module.RoomTypeId);
                this.roomId = module.RoomId;
                this.roomTypeId = module.RoomTypeId;
                this.currentModule = module;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void UCAggrImpMestList_Load(object sender, EventArgs e)
        {
            try
            {
                //Gan ngon ngu
                LoadKeysFromlanguage();

                if (GlobalVariables.AcsAuthorizeSDO != null)
                {
                    controlAcs = GlobalVariables.AcsAuthorizeSDO.ControlInRoles;
                }
                GlobalStore.LoadConfig();
                LoadControlsState();

                FillDataNavStatus();

                InitCheck(cboReqDepartment, SelectionGrid__Department);
                InitCombo(cboReqDepartment, BackendDataWorker.Get<HIS_DEPARTMENT>().Where(o => o.IS_ACTIVE == 1).ToList(), "DEPARTMENT_NAME", "ID");
                //Gan gia tri mac dinh
                SetDefaultValueControl();

                //Load du lieu
                FillDataToGrid();

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadControlsState()
        {
            try
            {
                if (controlAcs != null && controlAcs.FirstOrDefault(o => o.CONTROL_CODE == ControlCode.Btn_CancelImport_Enable && o.IS_ACTIVE == IMSys.DbConfig.ACS_RS.COMMON.IS_ACTIVE__TRUE) != null)
                {
                    this.isCancelImport_Enable = true;
                }
                else
                {
                    this.isCancelImport_Enable = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void Event_MouseDown(object sender, MouseEventArgs e)
        {
            switch (e.Button)
            {
                case MouseButtons.Right:
                    {
                        MessageBox.Show("Chuot phai");
                    }
                    break;
                case MouseButtons.Left:
                    {
                        MessageBox.Show("Chuot trai");
                    }
                    break;
            }
        }
        #endregion

        #region Private method
        #region load
        private void LoadKeysFromlanguage()
        {
            try
            {
                this.btnRefresh.Text = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_AGGR_IMP_MEST_LIST__BTN_REFRESH",
                    Resources.ResourceLanguageManager.LanguageUCAggrImpMestList,
                    cultureLang);
                this.btnSearch.Text = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_AGGR_IMP_MEST_LIST__BTN_SEARCH",
                    Resources.ResourceLanguageManager.LanguageUCAggrImpMestList,
                    cultureLang);
                this.Gc_ApprovalName.Caption = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_AGGR_IMP_MEST_LIST__GC_APPROVAL_NAME",
                    Resources.ResourceLanguageManager.LanguageUCAggrImpMestList,
                    cultureLang);
                this.Gc_ApprovalTime.Caption = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_AGGR_IMP_MEST_LIST__GC_APPROVAL_TIME",
                    Resources.ResourceLanguageManager.LanguageUCAggrImpMestList,
                    cultureLang);
                this.Gc_CreateTime.Caption = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_AGGR_IMP_MEST_LIST__GC_CREATE_TIME",
                    Resources.ResourceLanguageManager.LanguageUCAggrImpMestList,
                    cultureLang);
                this.Gc_Creator.Caption = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_AGGR_IMP_MEST_LIST__GC_CREATOR",
                    Resources.ResourceLanguageManager.LanguageUCAggrImpMestList,
                    cultureLang);
                this.Gc_ImpLoginName.Caption = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_AGGR_IMP_MEST_LIST__GC_IMP_LOGIN_NAME",
                    Resources.ResourceLanguageManager.LanguageUCAggrImpMestList,
                    cultureLang);
                this.Gc_ImpMestCode.Caption = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_AGGR_IMP_MEST_LIST__GC_IMP_MEST_CODE",
                    Resources.ResourceLanguageManager.LanguageUCAggrImpMestList,
                    cultureLang);
                this.Gc_ImpMestTypeName.Caption = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_AGGR_IMP_MEST_LIST__GC_IMP_MEST_TYPE_NAME",
                    Resources.ResourceLanguageManager.LanguageUCAggrImpMestList,
                    cultureLang);
                this.Gc_ImpTime.Caption = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_AGGR_IMP_MEST_LIST__GC_IMP_TIME",
                    Resources.ResourceLanguageManager.LanguageUCAggrImpMestList,
                    cultureLang);
                this.Gc_MediStockName.Caption = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_AGGR_IMP_MEST_LIST__GC_MEDI_STOCK_NAME",
                    Resources.ResourceLanguageManager.LanguageUCAggrImpMestList,
                    cultureLang);
                this.Gc_Modifier.Caption = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_AGGR_IMP_MEST_LIST__GC_MODIFIER",
                    Resources.ResourceLanguageManager.LanguageUCAggrImpMestList,
                    cultureLang);
                this.Gc_ModifyTime.Caption = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_AGGR_IMP_MEST_LIST__GC_MODIFY_TIME",
                    Resources.ResourceLanguageManager.LanguageUCAggrImpMestList,
                    cultureLang);
                this.Gc_ReqName.Caption = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_AGGR_IMP_MEST_LIST__GC_REQ_NAME",
                    Resources.ResourceLanguageManager.LanguageUCAggrImpMestList,
                    cultureLang);
                this.Gc_STT.Caption = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_AGGR_IMP_MEST_LIST__GC_STT",
                    Resources.ResourceLanguageManager.LanguageUCAggrImpMestList,
                    cultureLang);
                this.lciCreateTimeFrom.Text = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_AGGR_IMP_MEST_LIST__LCI_CREATE_TIME_FROM",
                    Resources.ResourceLanguageManager.LanguageUCAggrImpMestList,
                    cultureLang);
                this.lciCreateTimeTo.Text = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_AGGR_IMP_MEST_LIST__LCI_CREATE_TIME_TO",
                    Resources.ResourceLanguageManager.LanguageUCAggrImpMestList,
                    cultureLang);
                this.layoutControlItem8.Text = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_AGGR_IMP_MEST_LIST__LCI_CREATE_TIME_FROM",
                    Resources.ResourceLanguageManager.LanguageUCAggrImpMestList,
                    cultureLang);
                this.layoutControlItem9.Text = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_AGGR_IMP_MEST_LIST__LCI_CREATE_TIME_TO",
                    Resources.ResourceLanguageManager.LanguageUCAggrImpMestList,
                    cultureLang);
                this.navBarGroupCreateTime.Caption = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_AGGR_IMP_MEST_LIST__NAV_BAR_CREATE_TIME",
                    Resources.ResourceLanguageManager.LanguageUCAggrImpMestList,
                    cultureLang);
                this.navBarGroupReqDepartment.Caption = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_AGGR_IMP_MEST_LIST__NAV_BAR_REQ_DEPARTMENT",
                    Resources.ResourceLanguageManager.LanguageUCAggrImpMestList,
                    cultureLang);
                this.navBarGroupImpTime.Caption = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_AGGR_IMP_MEST_LIST__NAV_BAR_IMP_TIME",
                    Resources.ResourceLanguageManager.LanguageUCAggrImpMestList,
                    cultureLang);
                this.txtKeyWord.Properties.NullValuePrompt = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_AGGR_IMP_MEST_LIST__TXT_KEYWORD__NULL_VALUE",
                    Resources.ResourceLanguageManager.LanguageUCAggrImpMestList,
                    cultureLang);
                this.navBarGroupStatus.Caption = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_AGGR_IMP_MEST_LIST__NAV_BAR_STATUS",
                    Resources.ResourceLanguageManager.LanguageUCAggrImpMestList,
                    cultureLang);
                this.Gc_ImpMestExp2.Caption = Inventec.Common.Resource.Get.Value(
            "IVT_LANGUAGE_KEY__UC_AGGR_IMP_MEST_LIST__GC_IMP_MEST_SUB_CODE_2",
            Resources.ResourceLanguageManager.LanguageUCAggrImpMestList,
            cultureLang);

                this.ButtonApproval.Buttons[0].ToolTip = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_AGGR_IMP_MEST_LIST__GR_BUTTON_APPROVAL",
                    Resources.ResourceLanguageManager.LanguageUCAggrImpMestList,
                    cultureLang);
                this.ButtonApprovalDisable.Buttons[0].ToolTip = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_AGGR_IMP_MEST_LIST__GR_BUTTON_APPROVAL",
                    Resources.ResourceLanguageManager.LanguageUCAggrImpMestList,
                    cultureLang);
                this.ButtonDisApproval.Buttons[0].ToolTip = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_AGGR_IMP_MEST_LIST__GR_BUTTON_DIS_APPROVAL",
                    Resources.ResourceLanguageManager.LanguageUCAggrImpMestList,
                    cultureLang);
                this.ButtonDisApprovalDisable.Buttons[0].ToolTip = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_AGGR_IMP_MEST_LIST__GR_BUTTON_DIS_APPROVAL",
                    Resources.ResourceLanguageManager.LanguageUCAggrImpMestList,
                    cultureLang);
                this.ButtonDiscard.Buttons[0].ToolTip = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_AGGR_IMP_MEST_LIST__GR_BUTTON_DISCARD",
                    Resources.ResourceLanguageManager.LanguageUCAggrImpMestList,
                    cultureLang);
                this.ButtonDiscardDisable.Buttons[0].ToolTip = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_AGGR_IMP_MEST_LIST__GR_BUTTON_DISCARD",
                    Resources.ResourceLanguageManager.LanguageUCAggrImpMestList,
                    cultureLang);
                this.ButtonEdit.Buttons[0].ToolTip = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_AGGR_IMP_MEST_LIST__GR_BUTTON_EDIT",
                    Resources.ResourceLanguageManager.LanguageUCAggrImpMestList,
                    cultureLang);
                this.ButtonEditDisable.Buttons[0].ToolTip = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_AGGR_IMP_MEST_LIST__GR_BUTTON_EDIT",
                    Resources.ResourceLanguageManager.LanguageUCAggrImpMestList,
                    cultureLang);
                this.ButtonImport.Buttons[0].ToolTip = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_AGGR_IMP_MEST_LIST__GR_BUTTON_IMPORT",
                    Resources.ResourceLanguageManager.LanguageUCAggrImpMestList,
                    cultureLang);
                this.ButtonImportDisable.Buttons[0].ToolTip = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_AGGR_IMP_MEST_LIST__GR_BUTTON_IMPORT",
                    Resources.ResourceLanguageManager.LanguageUCAggrImpMestList,
                    cultureLang);
                this.ButtonRequest.Buttons[0].ToolTip = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_AGGR_IMP_MEST_LIST__GR_BUTTON_RE_APPROVAL",
                    Resources.ResourceLanguageManager.LanguageUCAggrImpMestList,
                    cultureLang);
                this.ButtonRequestDisable.Buttons[0].ToolTip = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_AGGR_IMP_MEST_LIST__GR_BUTTON_RE_APPROVAL",
                    Resources.ResourceLanguageManager.LanguageUCAggrImpMestList,
                    cultureLang);
                this.ButtonViewDetail.Buttons[0].ToolTip = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_AGGR_IMP_MEST_LIST__GR_BUTTON_VIEW",
                    Resources.ResourceLanguageManager.LanguageUCAggrImpMestList,
                    cultureLang);
                this.btnExport.Text = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_AGGR_IMP_MEST_LIST__BTN_EXPORT",
                    Resources.ResourceLanguageManager.LanguageUCAggrImpMestList,
                    cultureLang);

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void FillDataNavStatus()
        {
            try
            {
                navBarControlFilter.BeginUpdate();
                int d = 0;
                foreach (var item in Base.GlobalStore.HisImpMestStts)
                {
                    navBarGroupStatus.GroupClientHeight += 25;
                    DevExpress.XtraEditors.CheckEdit checkEdit = new DevExpress.XtraEditors.CheckEdit();
                    layoutControlContainerStatus.Controls.Add(checkEdit);
                    checkEdit.Location = new System.Drawing.Point(50, 2 + (d * 23));
                    checkEdit.Name = item.ID.ToString();
                    checkEdit.Properties.Caption = item.IMP_MEST_STT_NAME;
                    checkEdit.Size = new System.Drawing.Size(150, 19);
                    checkEdit.StyleController = this.layoutControlContainerStatus;
                    checkEdit.TabIndex = 4 + d;
                    checkEdit.EnterMoveNextControl = false;
                    d++;
                }
                navBarControlFilter.EndUpdate();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SetDefaultValueControl()
        {
            try
            {
                _Room = BackendDataWorker.Get<V_HIS_ROOM>().FirstOrDefault(o => o.ID == this.roomId);

                txtKeyWord.Text = "";
                dtCreateTimeFrom.DateTime = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(Inventec.Common.DateTime.Get.StartDay() ?? 0) ?? DateTime.MinValue;
                dtCreateTimeTo.DateTime = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(Inventec.Common.DateTime.Get.EndDay() ?? 0) ?? DateTime.MinValue;
                dtImpTimeFrom.EditValue = null;
                dtImpTimeTo.EditValue = null;
                cboReqDepartment.Enabled = false;
                cboReqDepartment.Enabled = true;

                txtKeyWord.Focus();
                SetDefaultStatus();

                long showbtn = Inventec.Common.TypeConvert.Parse.ToInt64(HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>(Base.GlobalStore.showButton));

                if (showbtn == 1)
                {
                    lciBtnExport.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                    if (medistock == null)
                    {
                        lciBtnExport.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                    }
                }
                else
                {
                    lciBtnExport.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetDefaultStatus()
        {
            try
            {
                if (layoutControlContainerStatus.Controls.Count > 0)
                {
                    for (int i = 0; i < layoutControlContainerStatus.Controls.Count; i++)
                    {
                        if (layoutControlContainerStatus.Controls[i] is DevExpress.XtraEditors.CheckEdit)
                        {
                            var checkEdit = layoutControlContainerStatus.Controls[i] as DevExpress.XtraEditors.CheckEdit;
                            checkEdit.Checked = false;
                        }
                    }
                }
                navBarGroupStatus.Expanded = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void FillDataToGrid()
        {
            try
            {
                WaitingManager.Show();
                int pagingSize = ucPaging.pagingGrid != null ? ucPaging.pagingGrid.PageSize : (int)ConfigApplications.NumPageSize;
                GridPaging(new CommonParam(0, pagingSize));
                CommonParam param = new CommonParam();
                param.Limit = rowCount;
                param.Count = dataTotal;
                ucPaging.Init(GridPaging, param, pagingSize, this.gridControl);
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                WaitingManager.Hide();
            }
        }

        private void GridPaging(object param)
        {
            try
            {
                startPage = ((CommonParam)param).Start ?? 0;
                int limit = ((CommonParam)param).Limit ?? 0;
                CommonParam paramCommon = new CommonParam(startPage, limit);
                ApiResultObject<List<MOS.EFMODEL.DataModels.V_HIS_IMP_MEST>> apiResult = null;
                MOS.Filter.HisImpMestViewFilter filter = new MOS.Filter.HisImpMestViewFilter();
                filter.IMP_MEST_TYPE_ID = IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__THT;
                SetFilter(ref filter);
                gridView.BeginUpdate();
                apiResult = new Inventec.Common.Adapter.BackendAdapter
                    (paramCommon).GetRO<List<MOS.EFMODEL.DataModels.V_HIS_IMP_MEST>>
                    (ApiConsumer.HisRequestUriStore.HIS_IMP_MEST_GETVIEW, ApiConsumer.ApiConsumers.MosConsumer, filter, paramCommon);
                if (apiResult != null)
                {
                    var data = apiResult.Data;
                    if (data != null && data.Count > 0)
                    {
                        gridControl.DataSource = data;
                        rowCount = (data == null ? 0 : data.Count);
                        dataTotal = (apiResult.Param == null ? 0 : apiResult.Param.Count ?? 0);
                    }
                    else
                    {
                        gridControl.DataSource = null;
                        rowCount = (data == null ? 0 : data.Count);
                        dataTotal = (apiResult.Param == null ? 0 : apiResult.Param.Count ?? 0);
                    }
                }
                gridView.EndUpdate();

                #region Process has exception
                HIS.Desktop.Controls.Session.SessionManager.ProcessTokenLost(paramCommon);
                #endregion
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                gridView.EndUpdate();
            }
        }

        private void SetFilter(ref MOS.Filter.HisImpMestViewFilter filter)
        {
            try
            {
                filter.ORDER_FIELD = "EXP_MEST_MODIFY_TIME";
                filter.ORDER_DIRECTION = "DESC";
                filter.KEY_WORD = txtKeyWord.Text.Trim();
                if (roomId == 0)
                {
                    filter.CREATOR = LoggingName;
                }
                else
                {
                    filter.DATA_DOMAIN_FILTER = true;
                    filter.WORKING_ROOM_ID = roomId;
                }
                if (dtCreateTimeFrom.EditValue != null && dtCreateTimeFrom.DateTime != DateTime.MinValue)
                    filter.CREATE_TIME_FROM = Inventec.Common.TypeConvert.Parse.ToInt64(
                        Convert.ToDateTime(dtCreateTimeFrom.EditValue).ToString("yyyyMMddHHmm") + "00");

                if (dtCreateTimeTo.EditValue != null && dtCreateTimeTo.DateTime != DateTime.MinValue)
                    filter.CREATE_TIME_TO = Inventec.Common.TypeConvert.Parse.ToInt64(
                        Convert.ToDateTime(dtCreateTimeTo.EditValue).ToString("yyyyMMddHHmm") + "59");
                if (dtImpTimeFrom.EditValue != null && dtImpTimeFrom.DateTime != DateTime.MinValue)
                    filter.IMP_TIME_FROM = Inventec.Common.TypeConvert.Parse.ToInt64(
                        Convert.ToDateTime(dtImpTimeFrom.EditValue).ToString("yyyyMMddHHmm") + "00");
                if (dtImpTimeTo.EditValue != null && dtImpTimeTo.DateTime != DateTime.MinValue)
                    filter.IMP_TIME_TO = Inventec.Common.TypeConvert.Parse.ToInt64(
                        Convert.ToDateTime(dtImpTimeTo.EditValue).ToString("yyyyMMddHHmm") + "59");


                SetFilterStatus(ref filter);
                if (listDepartment_Selected != null && listDepartment_Selected.Count > 0)
                {
                    filter.REQ_DEPARTMENT_IDs = listDepartment_Selected.Select(o => o.ID).ToList();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetFilterStatus(ref MOS.Filter.HisImpMestViewFilter filter)
        {
            try
            {
                if (layoutControlContainerStatus.Controls.Count > 0)
                {
                    for (int i = 0; i < layoutControlContainerStatus.Controls.Count; i++)
                    {
                        if (layoutControlContainerStatus.Controls[i] is DevExpress.XtraEditors.CheckEdit)
                        {
                            var checkEdit = layoutControlContainerStatus.Controls[i] as DevExpress.XtraEditors.CheckEdit;
                            if (checkEdit.Checked)
                            {
                                if (filter.IMP_MEST_STT_IDs == null)
                                    filter.IMP_MEST_STT_IDs = new List<long>();
                                filter.IMP_MEST_STT_IDs.Add(Inventec.Common.TypeConvert.Parse.ToInt64(checkEdit.Name));
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        #endregion

        #region display
        private void toolTipController_GetActiveObjectInfo(object sender, DevExpress.Utils.ToolTipControllerGetActiveObjectInfoEventArgs e)
        {
            try
            {
                if (e.Info == null && e.SelectedControl == gridControl)
                {
                    DevExpress.XtraGrid.Views.Grid.GridView view = gridControl.FocusedView as DevExpress.XtraGrid.Views.Grid.GridView;
                    GridHitInfo info = view.CalcHitInfo(e.ControlMousePosition);
                    if (info.InRowCell)
                    {
                        if (lastRowHandle != info.RowHandle || lastColumn != info.Column)
                        {
                            lastColumn = info.Column;
                            lastRowHandle = info.RowHandle;

                            string text = "";
                            if (info.Column.FieldName == "IMP_MEST_STT_NAME")
                            {
                                text = (view.GetRowCellValue(lastRowHandle, "IMP_MEST_STT_NAME") ?? "").ToString();
                            }
                            else if (info.Column.FieldName == "IMP_MEST_STT_ICON")
                            {
                                text = (view.GetRowCellValue(lastRowHandle, "IMP_MEST_STT_NAME") ?? "").ToString();
                            }
                            lastInfo = new DevExpress.Utils.ToolTipControlInfo(new DevExpress.XtraGrid.GridToolTipInfo(view, new DevExpress.XtraGrid.Views.Base.CellToolTipInfo(info.RowHandle, info.Column, "Text")), text);
                        }
                        e.Info = lastInfo;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridView_CustomRowCellEdit(object sender, DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs e)
        {
            try
            {
                if (e.RowHandle >= 0)
                {
                    long statusIdCheckForButtonEdit = Inventec.Common.TypeConvert.Parse.ToInt64((gridView.GetRowCellValue(e.RowHandle, "IMP_MEST_STT_ID") ?? "").ToString());
                    long mediStockId = Inventec.Common.TypeConvert.Parse.ToInt64((gridView.GetRowCellValue(e.RowHandle, "MEDI_STOCK_ID") ?? "").ToString());
                    long typeIdCheckForButtonEdit = Inventec.Common.TypeConvert.Parse.ToInt64((gridView.GetRowCellValue(e.RowHandle, "IMP_MEST_TYPE_ID") ?? "").ToString());
                    string creator = (gridView.GetRowCellValue(e.RowHandle, "CREATOR") ?? "").ToString().Trim();
                    long departmentId = Inventec.Common.TypeConvert.Parse.ToInt64((gridView.GetRowCellValue(e.RowHandle, "REQ_DEPARTMENT_ID") ?? "0").ToString());
                    long reqRoomId = Inventec.Common.TypeConvert.Parse.ToInt64((gridView.GetRowCellValue(e.RowHandle, "REQ_ROOM_ID") ?? "0").ToString());
                    if (e.Column.FieldName == "EDIT_DISPLAY") // sửa
                    {
                        if ((_Room != null && _Room.DEPARTMENT_ID == departmentId)
                            && (statusIdCheckForButtonEdit == IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_STT.ID__DRAFT ||
                            ((statusIdCheckForButtonEdit == IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_STT.ID__REQUEST ||
                            statusIdCheckForButtonEdit == IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_STT.ID__REJECT) &&
                            (typeIdCheckForButtonEdit != IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__DNTTL &&
                            typeIdCheckForButtonEdit != IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__CK))))
                            e.RepositoryItem = ButtonEdit;
                        else
                            e.RepositoryItem = ButtonEditDisable;
                    }
                    else if (e.Column.FieldName == "DISCARD_DISPLAY") //hủy
                    {
                        if ((_Room != null && _Room.DEPARTMENT_ID == departmentId)
                            && (statusIdCheckForButtonEdit == IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_STT.ID__DRAFT ||
                            statusIdCheckForButtonEdit == IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_STT.ID__REJECT ||
                            statusIdCheckForButtonEdit == IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_STT.ID__REQUEST))
                        {
                            if (GlobalStore.isEnableDeleteAggregate)
                            {
                                if ((CheckLoginAdmin.IsAdmin(LoggingName) || creator.Equals(LoggingName)) && reqRoomId == _Room.ID)
                                    e.RepositoryItem = ButtonDiscard;
                                else
                                    e.RepositoryItem = ButtonDiscardDisable;
                            }
                            else
                                e.RepositoryItem = ButtonDiscard;
                        }
                        else
                            e.RepositoryItem = ButtonDiscardDisable;
                    }
                    else if (e.Column.FieldName == "APPROVAL_DISPLAY") //duyet
                    {
                        if (medistock != null && medistock.ID == mediStockId &&
                            ((statusIdCheckForButtonEdit == IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_STT.ID__REJECT ||
                            statusIdCheckForButtonEdit == IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_STT.ID__REQUEST)))

                            e.RepositoryItem = ButtonApproval;
                        else
                            e.RepositoryItem = ButtonApprovalDisable;
                    }
                    else if (e.Column.FieldName == "DIS_APPROVAL")// Từ chối duyệt
                    {
                        if (medistock != null && medistock.ID == mediStockId &&
                            (statusIdCheckForButtonEdit != IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_STT.ID__DRAFT &&
                            statusIdCheckForButtonEdit != IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_STT.ID__REJECT &&
                            statusIdCheckForButtonEdit != IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_STT.ID__IMPORT))
                            e.RepositoryItem = ButtonDisApproval;
                        else
                            e.RepositoryItem = ButtonDisApprovalDisable;
                    }
                    else if (e.Column.FieldName == "IMPORT_DISPLAY")// thực xuất
                    {
                        if (statusIdCheckForButtonEdit == IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_STT.ID__APPROVAL)
                        {
                            if (medistock != null && medistock.ID == mediStockId)
                                e.RepositoryItem = ButtonImport;
                            else
                                e.RepositoryItem = ButtonImportDisable;
                        }
                        else if (statusIdCheckForButtonEdit == IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_STT.ID__IMPORT)
                        {
                            if ((creator == LoggingName || CheckLoginAdmin.IsAdmin(LoggingName) || this.isCancelImport_Enable) && medistock != null && medistock.ID == mediStockId)
                                e.RepositoryItem = Btn_CancelImport_Enable;
                            else
                                e.RepositoryItem = Btn_CancelImport_Disable;
                        }
                        else
                        {
                            e.RepositoryItem = ButtonImportDisable;
                        }
                    }
                    else if (e.Column.FieldName == "REQUEST_DISPLAY")// Hủy duyệt
                    {
                        if (medistock != null && medistock.ID == mediStockId &&
                            (statusIdCheckForButtonEdit == IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_STT.ID__APPROVAL ||
                            statusIdCheckForButtonEdit == IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_STT.ID__REJECT))
                            e.RepositoryItem = ButtonRequest;
                        else
                            e.RepositoryItem = ButtonRequestDisable;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridView_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)
                {
                    MOS.EFMODEL.DataModels.V_HIS_IMP_MEST data = (MOS.EFMODEL.DataModels.V_HIS_IMP_MEST)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    if (data != null)
                    {
                        if (e.Column.FieldName == "STT")
                        {
                            e.Value = e.ListSourceRowIndex + 1 + startPage;
                        }
                        else if (e.Column.FieldName == "IMP_MEST_STT_ICON")// trạng thái
                        {
                            if (data.IMP_MEST_STT_ID == IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_STT.ID__DRAFT) //tam thoi
                            {
                                e.Value = imageListStatus.Images[0];
                            }
                            else if (data.IMP_MEST_STT_ID == IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_STT.ID__REQUEST) //huy duyet
                            {
                                e.Value = imageListStatus.Images[1];
                            }
                            else if (data.IMP_MEST_STT_ID == IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_STT.ID__REJECT) // tu choi duyet
                            {
                                e.Value = imageListStatus.Images[2];
                            }
                            else if (data.IMP_MEST_STT_ID == IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_STT.ID__APPROVAL) // duyet
                            {
                                e.Value = imageListStatus.Images[3];
                            }
                            else if (data.IMP_MEST_STT_ID == IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_STT.ID__IMPORT) // da xuat
                            {
                                e.Value = imageListStatus.Images[4];
                            }
                        }
                        else if (e.Column.FieldName == "CREATE_TIME_DISPLAY")
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(data.CREATE_TIME ?? 0);
                        }
                        else if (e.Column.FieldName == "IMP_TIME_DISPLAY")
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(data.IMP_TIME ?? 0);
                        }
                        else if (e.Column.FieldName == "MODIFY_TIME_DISPLAY")
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(data.MODIFY_TIME ?? 0);
                        }
                        else if (e.Column.FieldName == "APPROVAL_TIME_DISPLAY")
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(data.APPROVAL_TIME ?? 0);
                        }
                        else if (e.Column.FieldName == "APPROVAL_LOGINNAME_DISPLAY")
                        {
                            string APPROVAL_LOGINNAME = data.APPROVAL_LOGINNAME;
                            string APPROVAL_USERNAME = data.APPROVAL_USERNAME;
                            e.Value = DisplayName(APPROVAL_LOGINNAME, APPROVAL_USERNAME);
                        }
                        else if (e.Column.FieldName == "IMP_LOGINNAME_DISPLAY")
                        {
                            string IMP_LOGINNAME = data.IMP_LOGINNAME;
                            string IMP_USERNAME = data.IMP_USERNAME;
                            e.Value = DisplayName(IMP_LOGINNAME, IMP_USERNAME);
                        }
                        else if (e.Column.FieldName == "REQ_LOGINNAME_DISPLAY")
                        {
                            string Req_loginName = data.REQ_LOGINNAME;
                            string Req_UserName = data.REQ_USERNAME;
                            e.Value = DisplayName(Req_loginName, Req_UserName);
                        }
                        else if (e.Column.FieldName == "IMP_MEST_SUB_CODE_2")
                        {
                            if (data.IMP_MEST_SUB_CODE_2 != null)
                                e.Value = data.IMP_MEST_SUB_CODE_2;
                            else
                                e.Value = "";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private string DisplayName(string loginname, string username)
        {
            string value = "";
            try
            {
                if (String.IsNullOrEmpty(loginname) && String.IsNullOrEmpty(username))
                {
                    value = "";
                }
                else if (loginname != "" && username == "")
                {
                    value = loginname;
                }
                else if (loginname == "" && username != "")
                {
                    value = username;
                }
                else if (loginname != "" && username != "")
                {
                    value = string.Format("{0} - {1}", loginname, username);
                }
                return value;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                return value;
            }
        }
        #endregion

        #region Event
        private void btnSearch_Click(object sender, EventArgs e)
        {
            try
            {
                FillDataToGrid();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            try
            {
                ResetCombo(cboReqDepartment);
                SetDefaultValueControl();
                FillDataToGrid();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtKeyWord_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    btnSearch_Click(null, null);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void dtCreateTimeFrom_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == DevExpress.XtraEditors.PopupCloseMode.Normal)
                {
                    if (dtCreateTimeFrom.EditValue != null)
                    {
                        dtCreateTimeTo.Focus();
                        dtCreateTimeTo.ShowPopup();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void dtCreateTimeTo_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == DevExpress.XtraEditors.PopupCloseMode.Normal)
                {
                    SendKeys.Send("{TAB}");
                }
                if (dtCreateTimeTo.DateTime != null)
                {
                    string dateTime = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(dtCreateTimeTo.DateTime).ToString().Substring(0, 8) + "235959";
                    dtCreateTimeTo.DateTime = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(Inventec.Common.TypeConvert.Parse.ToInt64(dateTime)) ?? DateTime.MinValue;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ButtonViewDetail_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                var ExpMestData = (MOS.EFMODEL.DataModels.V_HIS_IMP_MEST)gridView.GetFocusedRow();
                Inventec.Desktop.Common.Modules.Module moduleData = LocalStorage.LocalData.GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.ApproveAggrImpMest").FirstOrDefault();
                if (moduleData == null)
                {
                    Inventec.Common.Logging.LogSystem.Error("khong tim thay moduleLink = HIS.Desktop.Plugins.ApproveAggrImpMest");
                    MessageManager.Show(Resources.ResourceMessage.TaiKhoanKhongCoQuyenThucHienChucNang);
                }

                if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                {
                    moduleData.RoomId = this.roomId;
                    moduleData.RoomTypeId = this.roomTypeId;
                    List<object> listArgs = new List<object>();
                    listArgs.Add(ExpMestData.ID);
                    listArgs.Add((DelegateRefreshData)FillDataToGrid);
                    var extenceInstance = HIS.Desktop.Utility.PluginInstance.GetPluginInstance(HIS.Desktop.Utility.PluginInstance.GetModuleWithWorkingRoom(moduleData, roomId, roomTypeId), listArgs);
                    if (extenceInstance == null) throw new ArgumentNullException("moduleData is null");

                    ((Form)extenceInstance).ShowDialog();
                }
                else
                {
                    MessageManager.Show(Resources.ResourceMessage.TaiKhoanKhongCoQuyenThucHienChucNang);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ButtonEdit_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                MessageManager.Show(Resources.ResourceMessage.ChucNangDangPhatTrienVuiLongThuLaiSau);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ButtonDiscard_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            CommonParam param = new CommonParam();
            try
            {
                bool success = false;
                if (DevExpress.XtraEditors.XtraMessageBox.Show(
                    Resources.ResourceMessage.HeThongTBCuaSoThongBaoBanCoMuonHuyDuLieuKhong,
                    Resources.ResourceMessage.ThongBao,
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    var row = (MOS.EFMODEL.DataModels.V_HIS_IMP_MEST)gridView.GetFocusedRow();
                    if (row != null)
                    {
                        WaitingManager.Show();
                        MOS.EFMODEL.DataModels.HIS_IMP_MEST data = new MOS.EFMODEL.DataModels.HIS_IMP_MEST();
                        Inventec.Common.Mapper.DataObjectMapper.Map<MOS.EFMODEL.DataModels.HIS_IMP_MEST>(data, row);
                        data.ID = row.ID;
                        var apiresul = new Inventec.Common.Adapter.BackendAdapter
                            (param).Post<bool>
                            (ApiConsumer.HisRequestUriStore.HIS_IMP_MEST_DELETE, ApiConsumer.ApiConsumers.MosConsumer, data, param);
                        if (apiresul)
                        {
                            success = true;
                            FillDataToGrid();
                        }
                        WaitingManager.Hide();

                        #region Show message
                        MessageManager.Show(this.ParentForm, param, success);
                        #endregion

                        #region Process has exception
                        HIS.Desktop.Controls.Session.SessionManager.ProcessTokenLost(param);
                        #endregion
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                WaitingManager.Hide();
            }
        }

        private void ButtonApproval_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                WaitingManager.Show();
                bool success = false;
                CommonParam param = new CommonParam();
                var row = (MOS.EFMODEL.DataModels.V_HIS_IMP_MEST)gridView.GetFocusedRow();
                var mobaChange = row.IS_MOBA_CHANGE_AMOUNT;
                if (mobaChange != null && mobaChange == 1)
                {
                    List<object> listArgs = new List<object>();
                    listArgs.Add(row);
                    listArgs.Add((HIS.Desktop.Common.DelegateSelectData)SelectDataResult);
                    HIS.Desktop.ModuleExt.PluginInstanceBehavior.ShowModule("HIS.Desktop.Plugins.ApproveMobaImpMest", this.roomId, this.roomTypeId, listArgs);

                }
                else
                {
                    MOS.EFMODEL.DataModels.HIS_IMP_MEST data = new MOS.EFMODEL.DataModels.HIS_IMP_MEST();
                    Inventec.Common.Mapper.DataObjectMapper.Map<MOS.EFMODEL.DataModels.HIS_IMP_MEST>(data, row);
                    data.ID = row.ID;
                    data.IMP_MEST_STT_ID = IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_STT.ID__APPROVAL;

                    var apiresul = new Inventec.Common.Adapter.BackendAdapter
                    (param).Post<MOS.EFMODEL.DataModels.HIS_IMP_MEST>
                    (ApiConsumer.HisRequestUriStore.HIS_IMP_MEST_UPDATE_STATUS, ApiConsumer.ApiConsumers.MosConsumer, data, param);
                    if (apiresul != null)
                    {
                        success = true;
                        FillDataToGrid();
                    }
                    #region Show message
                    Inventec.Desktop.Common.Message.MessageManager.Show(this.ParentForm, param, success);
                    #endregion

                    #region Process has exception
                    HIS.Desktop.Controls.Session.SessionManager.ProcessTokenLost(param);
                    #endregion
                }
                WaitingManager.Hide();


            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                WaitingManager.Hide();
            }
        }

        private void SelectDataResult(object data)
        {
            try
            {
                if (data != null)
                {
                    FillDataToGrid();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ButtonDisApproval_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                WaitingManager.Show();
                bool success = false;
                CommonParam param = new CommonParam();
                var row = (MOS.EFMODEL.DataModels.V_HIS_IMP_MEST)gridView.GetFocusedRow();
                MOS.EFMODEL.DataModels.HIS_IMP_MEST data = new MOS.EFMODEL.DataModels.HIS_IMP_MEST();
                Inventec.Common.Mapper.DataObjectMapper.Map<MOS.EFMODEL.DataModels.HIS_IMP_MEST>(data, row);
                data.ID = row.ID;
                data.IMP_MEST_STT_ID = IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_STT.ID__REJECT;
                var apiresul = new Inventec.Common.Adapter.BackendAdapter
                    (param).Post<MOS.EFMODEL.DataModels.HIS_IMP_MEST>
                    (ApiConsumer.HisRequestUriStore.HIS_IMP_MEST_UPDATE_STATUS, ApiConsumer.ApiConsumers.MosConsumer, data, param);
                if (apiresul != null)
                {
                    success = true;
                    FillDataToGrid();
                }
                WaitingManager.Hide();
                #region Show message
                Inventec.Desktop.Common.Message.MessageManager.Show(this.ParentForm, param, success);
                #endregion

                #region Process has exception
                HIS.Desktop.Controls.Session.SessionManager.ProcessTokenLost(param);
                #endregion
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                WaitingManager.Hide();
            }
        }

        private void ButtonImport_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                WaitingManager.Show();
                bool success = false;
                CommonParam param = new CommonParam();
                var row = (MOS.EFMODEL.DataModels.V_HIS_IMP_MEST)gridView.GetFocusedRow();
                MOS.EFMODEL.DataModels.HIS_IMP_MEST data = new MOS.EFMODEL.DataModels.HIS_IMP_MEST();
                Inventec.Common.Mapper.DataObjectMapper.Map<MOS.EFMODEL.DataModels.HIS_IMP_MEST>(data, row);
                data.ID = row.ID;
                var apiresul = new Inventec.Common.Adapter.BackendAdapter
                    (param).Post<MOS.EFMODEL.DataModels.HIS_IMP_MEST>
                    (ApiConsumer.HisRequestUriStore.HIS_IMP_MEST_IMPORT, ApiConsumer.ApiConsumers.MosConsumer, data, param);
                if (apiresul != null)
                {
                    success = true;
                    FillDataToGrid();
                }
                WaitingManager.Hide();
                #region Show message
                Inventec.Desktop.Common.Message.MessageManager.Show(this.ParentForm, param, success);
                #endregion

                #region Process has exception
                HIS.Desktop.Controls.Session.SessionManager.ProcessTokenLost(param);
                #endregion
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                WaitingManager.Hide();
            }
        }

        private void ButtonRequest_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                WaitingManager.Show();
                bool success = false;
                CommonParam param = new CommonParam();
                var row = (MOS.EFMODEL.DataModels.V_HIS_IMP_MEST)gridView.GetFocusedRow();
                if (row.IS_MOBA_CHANGE_AMOUNT != null && row.IS_MOBA_CHANGE_AMOUNT == 1)
                {
                    MOS.SDO.ImpMestAggrUnapprovalSDO data = new MOS.SDO.ImpMestAggrUnapprovalSDO();
                    data.ImpMestId = row.ID;
                    data.RequestRoomId = this.roomId;
                    var apiresul = new Inventec.Common.Adapter.BackendAdapter(param).Post<MOS.SDO.ImpMestAggrUnapprovalSDO>
                    ("api/HisImpMest/AggrUnapprove", ApiConsumer.ApiConsumers.MosConsumer, data, param);
                    if (apiresul != null)
                    {
                        success = true;
                        FillDataToGrid();
                    }
                }
                else
                {
                    MOS.EFMODEL.DataModels.HIS_IMP_MEST data = new MOS.EFMODEL.DataModels.HIS_IMP_MEST();
                    Inventec.Common.Mapper.DataObjectMapper.Map<MOS.EFMODEL.DataModels.HIS_IMP_MEST>(data, row);
                    data.ID = row.ID;
                    data.IMP_MEST_STT_ID = IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_STT.ID__REQUEST;
                    var apiresul = new Inventec.Common.Adapter.BackendAdapter
                        (param).Post<MOS.EFMODEL.DataModels.HIS_IMP_MEST>
                        (ApiConsumer.HisRequestUriStore.HIS_IMP_MEST_UPDATE_STATUS, ApiConsumer.ApiConsumers.MosConsumer, data, param);
                    if (apiresul != null)
                    {
                        success = true;
                        FillDataToGrid();
                    }
                }

                WaitingManager.Hide();
                #region Show message
                Inventec.Desktop.Common.Message.MessageManager.Show(this.ParentForm, param, success);
                #endregion

                #region Process has exception
                HIS.Desktop.Controls.Session.SessionManager.ProcessTokenLost(param);
                #endregion
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void Btn_CancelImport_Enable_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                WaitingManager.Show();
                bool success = false;
                CommonParam param = new CommonParam();
                var row = (MOS.EFMODEL.DataModels.V_HIS_IMP_MEST)gridView.GetFocusedRow();
                MOS.EFMODEL.DataModels.HIS_IMP_MEST data = new MOS.EFMODEL.DataModels.HIS_IMP_MEST();
                Inventec.Common.Mapper.DataObjectMapper.Map<MOS.EFMODEL.DataModels.HIS_IMP_MEST>(data, row);
                data.ID = row.ID;
                data.IMP_MEST_STT_ID = IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_STT.ID__APPROVAL;
                var apiresul = new Inventec.Common.Adapter.BackendAdapter
                    (param).Post<MOS.EFMODEL.DataModels.HIS_IMP_MEST>
                    ("api/HisImpMest/AggrUnimport", ApiConsumer.ApiConsumers.MosConsumer, data, param);
                if (apiresul != null)
                {
                    success = true;
                    FillDataToGrid();
                }
                WaitingManager.Hide();
                #region Show message
                Inventec.Desktop.Common.Message.MessageManager.Show(this.ParentForm, param, success);
                #endregion

                #region Process has exception
                HIS.Desktop.Controls.Session.SessionManager.ProcessTokenLost(param);
                #endregion
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                WaitingManager.Hide();
            }
        }

        private void InitCheck(GridLookUpEdit cbo, GridCheckMarksSelection.SelectionChangedEventHandler eventSelect)
        {
            try
            {
                GridCheckMarksSelection gridCheck = new GridCheckMarksSelection(cbo.Properties);
                gridCheck.SelectionChanged += new GridCheckMarksSelection.SelectionChangedEventHandler(eventSelect);
                cbo.Properties.Tag = gridCheck;
                cbo.Properties.View.OptionsSelection.MultiSelect = true;
                GridCheckMarksSelection gridCheckMark = cbo.Properties.Tag as GridCheckMarksSelection;
                if (gridCheckMark != null)
                {
                    gridCheckMark.ClearSelection(cbo.Properties.View);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ResetCombo(GridLookUpEdit cbo)
        {
            try
            {
                GridCheckMarksSelection gridCheckMark = cbo.Properties.Tag as GridCheckMarksSelection;
                if (gridCheckMark != null)
                {
                    gridCheckMark.SelectAll(cbo.Properties.DataSource);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void InitCombo(GridLookUpEdit cbo, object data, string DisplayValue, string ValueMember)
        {
            try
            {
                cbo.Properties.DataSource = data;
                cbo.Properties.DisplayMember = DisplayValue;
                cbo.Properties.ValueMember = ValueMember;

                DevExpress.XtraGrid.Columns.GridColumn col2 = cbo.Properties.View.Columns.AddField(DisplayValue);
                col2.VisibleIndex = 1;
                col2.Width = 200;
                col2.Caption = "Tất cả";
                cbo.Properties.PopupFormWidth = 200;
                cbo.Properties.View.OptionsView.ShowColumnHeaders = true;
                cbo.Properties.View.OptionsSelection.MultiSelect = true;

                GridCheckMarksSelection gridCheckMark = cbo.Properties.Tag as GridCheckMarksSelection;
                if (gridCheckMark != null)
                {
                    gridCheckMark.SelectAll(cbo.Properties.DataSource);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SelectionGrid__Department(object sender, EventArgs e)
        {
            try
            {
                listDepartment_Selected = new List<HIS_DEPARTMENT>();
                foreach (HIS_DEPARTMENT rv in (sender as GridCheckMarksSelection).Selection)
                {
                    if (rv != null)
                        listDepartment_Selected.Add(rv);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        #endregion
        #endregion

        #region Public method
        public void Search()
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

        public void Refreshs()
        {
            try
            {
                btnRefresh_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public void Export()
        {
            try
            {
                btnExport_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        #endregion

        #region report
        private void btnExport_Click(object sender, EventArgs e)
        {
            try
            {
                if (!btnExport.Enabled) return;
                if (lciBtnExport.Visibility == DevExpress.XtraLayout.Utils.LayoutVisibility.Never) return;

                if (dtImpTimeFrom.EditValue == null || dtImpTimeTo.EditValue == null)
                {
                    MessageBox.Show(Resources.ResourceMessage.BanChuaChonThoiGianThucXuat);
                    if (dtImpTimeFrom.EditValue == null)
                    {
                        dtImpTimeFrom.Focus();
                        dtImpTimeFrom.SelectAll();
                    }
                    else if (dtImpTimeTo.EditValue == null)
                    {
                        dtImpTimeTo.Focus();
                        dtImpTimeTo.SelectAll();
                    }
                    return;
                }

                Thread thread = new System.Threading.Thread(new System.Threading.ThreadStart(CreateReport));
                thread.Priority = ThreadPriority.Normal;
                thread.IsBackground = true;
                thread.SetApartmentState(System.Threading.ApartmentState.STA);
                try
                {
                    thread.Start();
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Error(ex);
                    thread.Abort();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void CreateReport()
        {
            try
            {
                List<string> expCode = new List<string>();
                Inventec.Common.FlexCellExport.Store store = new Inventec.Common.FlexCellExport.Store(true);

                string templateFile = System.IO.Path.Combine(Application.StartupPath + "\\Tmp\\Exp", "DanhSachCacMaPhieuTra.xlsx");

                //chọn đường dẫn
                saveFileDialog1.Filter = "Excel 2007 later file (*.xlsx)|*.xlsx|Excel 97-2003 file(*.xls)|*.xls";
                if (saveFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {

                    //getdata
                    WaitingManager.Show();

                    if (String.IsNullOrEmpty(templateFile))
                    {
                        store = null;
                        DevExpress.XtraEditors.XtraMessageBox.Show(String.Format(Resources.ResourceMessage.KhongTimThayBieuMauIn, templateFile));
                        return;
                    }

                    store.ReadTemplate(System.IO.Path.GetFullPath(templateFile));
                    if (store.TemplatePath == "")
                    {
                        DevExpress.XtraEditors.XtraMessageBox.Show("Biểu mẫu đang mở hoặc không tồn tại file template. Vui lòng kiểm tra lại. (" + templateFile + ")");
                        return;
                    }

                    GetDataProcessor(ref expCode);

                    ProcessData(expCode, ref store);
                    WaitingManager.Hide();

                    if (store != null)
                    {
                        try
                        {
                            if (store.OutFile(saveFileDialog1.FileName))
                            {
                                DevExpress.XtraEditors.XtraMessageBox.Show(Resources.ResourceMessage.TaiThanhCong);

                                if (MessageBox.Show(Resources.ResourceMessage.BanCoMuonMoFile,
                                    Resources.ResourceMessage.ThongBao, MessageBoxButtons.YesNo,
                                    MessageBoxIcon.Question) == DialogResult.Yes)
                                    System.Diagnostics.Process.Start(saveFileDialog1.FileName);
                            }
                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Warn(ex);
                        }
                    }
                    else
                    {
                        DevExpress.XtraEditors.XtraMessageBox.Show(Resources.ResourceMessage.XuLyThatBai);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ProcessData(List<string> expCode, ref Inventec.Common.FlexCellExport.Store store)
        {
            try
            {
                Inventec.Common.FlexCellExport.ProcessSingleTag singleTag = new Inventec.Common.FlexCellExport.ProcessSingleTag();
                Inventec.Common.FlexCellExport.ProcessObjectTag objectTag = new Inventec.Common.FlexCellExport.ProcessObjectTag();
                List<Base.ExportListCodeRDO> listRdo = new List<Base.ExportListCodeRDO>();
                Dictionary<string, object> singleValueDictionary = new Dictionary<string, object>();

                var room = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<MOS.EFMODEL.DataModels.V_HIS_ROOM>().FirstOrDefault(o => o.ID == this.roomId);

                if (expCode != null && expCode.Count > 0)
                {
                    Dictionary<int, List<string>> dicExpCode = new Dictionary<int, List<string>>();

                    int count = expCode.Count;
                    int max = count / 6;
                    int size = count % 6;
                    string emty = "";

                    if (count > 31)
                    {
                        for (int i = 0; i < 6; i++)
                        {
                            if (i != 5)
                            {
                                dicExpCode[i] = new List<string>();
                                dicExpCode[i].AddRange(expCode.GetRange(0, (size <= 0 ? max : max + 1)));
                                expCode.RemoveRange(0, (size <= 0 ? max : max + 1));
                            }
                            else
                                dicExpCode.Add(i, expCode);

                            if (dicExpCode[i].Count < dicExpCode[0].Count)
                            {
                                int loop = dicExpCode[0].Count - dicExpCode[i].Count;
                                for (int j = 0; j < loop; j++)
                                {
                                    dicExpCode[i].Add(emty);
                                }
                            }
                        }
                    }
                    else
                    {
                        for (int i = 0; i < 6; i++)
                        {
                            if (i != 5)
                            {
                                dicExpCode[i] = new List<string>();
                                dicExpCode[i].AddRange(expCode.GetRange(0, (size <= 0 ? max : max + 1)));
                                expCode.RemoveRange(0, (size <= 0 ? max : max + 1));
                                size--;
                            }
                            else
                                dicExpCode.Add(i, expCode);

                            if (dicExpCode[i].Count < dicExpCode[0].Count)
                            {
                                dicExpCode[i].Add(emty);
                            }
                        }
                    }

                    for (int i = 0; i < dicExpCode[0].Count; i++)
                    {
                        Base.ExportListCodeRDO a = new Base.ExportListCodeRDO();
                        a.EXPORT_CODE1 = dicExpCode[0][i];
                        a.EXPORT_CODE2 = dicExpCode[1][i];
                        a.EXPORT_CODE3 = dicExpCode[2][i];
                        a.EXPORT_CODE4 = dicExpCode[3][i];
                        a.EXPORT_CODE5 = dicExpCode[4][i];
                        a.EXPORT_CODE6 = dicExpCode[5][i];

                        listRdo.Add(a);
                    }
                }
                var mediStockName = medistock != null ? medistock.MEDI_STOCK_NAME : room.ROOM_NAME;
                singleTag.AddSingleKey(store, "TYPE", "THỰC NHẬP");
                singleTag.AddSingleKey(store, "MEDI_STOCK_NAME", mediStockName.ToUpper());
                singleTag.AddSingleKey(store, "EXP_TIME_FROM", dtImpTimeFrom.DateTime.ToString("dd/MM/yyyy"));
                singleTag.AddSingleKey(store, "EXP_TIME_TO", dtImpTimeTo.DateTime.ToString("dd/MM/yyyy"));
                HIS.Desktop.Print.SetCommonKey.SetCommonSingleKey(singleValueDictionary);
                singleTag.ProcessData(store, singleValueDictionary);

                store.SetCommonFunctions();
                objectTag.AddObjectData(store, "List", listRdo);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                store = null;
            }
        }

        private void GetDataProcessor(ref List<string> expCode)
        {
            try
            {
                CommonParam param = new CommonParam();
                MOS.Filter.HisImpMestViewFilter expFilter = new MOS.Filter.HisImpMestViewFilter();
                expFilter.IMP_MEST_STT_IDs = new List<long>();
                expFilter.IMP_MEST_STT_IDs.Add(IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_STT.ID__IMPORT);
                expFilter.IMP_MEST_TYPE_IDs = new List<long>();
                expFilter.IMP_MEST_TYPE_IDs.Add(IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__THT);
                expFilter.MEDI_STOCK_ID = medistock.ID;


                if (dtImpTimeFrom.EditValue != null && dtImpTimeFrom.DateTime != DateTime.MinValue)
                    expFilter.IMP_TIME_FROM = Inventec.Common.TypeConvert.Parse.ToInt64(
                        dtImpTimeFrom.DateTime.ToString("yyyyMMdd") + "000000");

                if (dtImpTimeTo.EditValue != null && dtImpTimeTo.DateTime != DateTime.MinValue)
                    expFilter.IMP_TIME_TO = Inventec.Common.TypeConvert.Parse.ToInt64(
                        dtImpTimeTo.DateTime.ToString("yyyyMMdd") + "235959");

                var exportList = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<MOS.EFMODEL.DataModels.V_HIS_IMP_MEST>>(ApiConsumer.HisRequestUriStore.HIS_IMP_MEST_GETVIEW, ApiConsumer.ApiConsumers.MosConsumer, expFilter, param);
                if (exportList != null && exportList.Count > 0)
                {
                    expCode = exportList.Select(s => s.IMP_MEST_CODE).OrderBy(o => o).ToList();
                }
            }
            catch (Exception ex)
            {
                expCode = null;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        #endregion

        private void cboReqDepartment_CustomDisplayText(object sender, DevExpress.XtraEditors.Controls.CustomDisplayTextEventArgs e)
        {
            try
            {
                e.DisplayText = "";
                string departmentName = "";
                if (listDepartment_Selected != null && listDepartment_Selected.Count > 0)
                {
                    foreach (var item in listDepartment_Selected)
                    {
                        departmentName += item.DEPARTMENT_NAME + ", ";
                    }
                }

                e.DisplayText = departmentName;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridView_MouseDown(object sender, MouseEventArgs e)
        {
            try
            {
                DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                GridHitInfo hi = view.CalcHitInfo(e.Location);
                if (hi.InRowCell)
                {
                    var listData = gridControl.DataSource as List<V_HIS_IMP_MEST>;
                    var data = (V_HIS_IMP_MEST)gridView.GetRow(hi.RowHandle);
                    if (e.Button == System.Windows.Forms.MouseButtons.Right)
                    {
                        DevExpress.XtraBars.BarManager barManager1 = new DevExpress.XtraBars.BarManager();
                        barManager1.Form = this;
                        this.currentImpMest = data;

                        List<V_HIS_IMP_MEST> _ImpMestChecks = new List<V_HIS_IMP_MEST>();
                        if (gridView.RowCount > 0)
                        {
                            for (int i = 0; i < gridView.SelectedRowsCount; i++)
                            {
                                if (gridView.GetSelectedRows()[i] >= 0)
                                {
                                    _ImpMestChecks.Add((V_HIS_IMP_MEST)gridView.GetRow(gridView.GetSelectedRows()[i]));
                                }
                            }
                        }
                        ImpMestAggregateListPopupMenuProcessor processor = new ImpMestAggregateListPopupMenuProcessor(this.currentImpMest, _ImpMestChecks, ImpMestAggregateMouseRightClick, barManager1);
                        processor.InitMenu();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        void ImpMestAggregateMouseRightClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                if (e.Item.Tag is ImpMestAggregateListPopupMenuProcessor.PrintType)
                {
                    var moduleType = (ImpMestAggregateListPopupMenuProcessor.PrintType)e.Item.Tag;
                    switch (moduleType)
                    {
                        case ImpMestAggregateListPopupMenuProcessor.PrintType.InTraDoiThuocTongHop:
                            ShowFormFilter(Convert.ToInt64(7), true);
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ShowFormFilter(long printType, bool selectMulti = false)
        {
            try
            {
                WaitingManager.Show();
                Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.AggrImpMestPrintFilter").FirstOrDefault();
                if (moduleData == null) Inventec.Common.Logging.LogSystem.Error("khong tim thay moduleLink = HIS.Desktop.Plugins.AggrImpMestPrintFilter");
                if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                {
                    List<V_HIS_IMP_MEST> _ImpMestTraDoiChecks = new List<V_HIS_IMP_MEST>();
                    List<object> listArgs = new List<object>();
                    if (selectMulti)
                    {
                        if (gridView.RowCount > 0)
                        {
                            for (int i = 0; i < gridView.SelectedRowsCount; i++)
                            {
                                if (gridView.GetSelectedRows()[i] >= 0)
                                {
                                    _ImpMestTraDoiChecks.Add((V_HIS_IMP_MEST)gridView.GetRow(gridView.GetSelectedRows()[i]));
                                }
                            }
                        }
                        if (_ImpMestTraDoiChecks != null && _ImpMestTraDoiChecks.Count > 0)
                        {
                            listArgs.Add(_ImpMestTraDoiChecks);
                        }
                    }
                    listArgs.Add(this.currentImpMest);
                    listArgs.Add(printType);
                    listArgs.Add(PluginInstance.GetModuleWithWorkingRoom(moduleData, this.currentModule.RoomId, this.currentModule.RoomTypeId));
                    var extenceInstance = PluginInstance.GetPluginInstance(PluginInstance.GetModuleWithWorkingRoom(moduleData, this.currentModule.RoomId, this.currentModule.RoomTypeId), listArgs);
                    if (extenceInstance == null) throw new ArgumentNullException("moduleData is null");
                    if (extenceInstance.GetType() == typeof(bool))
                    {
                        return;
                    }
                    ((Form)extenceInstance).ShowDialog();
                }
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridView_RowCellClick(object sender, DevExpress.XtraGrid.Views.Grid.RowCellClickEventArgs e)
        {
            try
            {
                long statusIdCheckForButtonEdit = Inventec.Common.TypeConvert.Parse.ToInt64((gridView.GetRowCellValue(e.RowHandle, "IMP_MEST_STT_ID") ?? "").ToString());
                long mediStockId = Inventec.Common.TypeConvert.Parse.ToInt64((gridView.GetRowCellValue(e.RowHandle, "MEDI_STOCK_ID") ?? "").ToString());
                long typeIdCheckForButtonEdit = Inventec.Common.TypeConvert.Parse.ToInt64((gridView.GetRowCellValue(e.RowHandle, "IMP_MEST_TYPE_ID") ?? "").ToString());
                string creator = (gridView.GetRowCellValue(e.RowHandle, "CREATOR") ?? "").ToString().Trim();
                long departmentId = Inventec.Common.TypeConvert.Parse.ToInt64((gridView.GetRowCellValue(e.RowHandle, "REQ_DEPARTMENT_ID") ?? "0").ToString());
                long reqRoomId = Inventec.Common.TypeConvert.Parse.ToInt64((gridView.GetRowCellValue(e.RowHandle, "REQ_ROOM_ID") ?? "0").ToString());
                if (e.Column.FieldName == "DETAIL_DATA_DISPLAY")
                {
                    ButtonViewDetail_ButtonClick(null, null);
                }
                if (e.Column.FieldName == "EDIT_DISPLAY") // sửa
                {
                    if ((_Room != null && _Room.DEPARTMENT_ID == departmentId)
                        && (statusIdCheckForButtonEdit == IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_STT.ID__DRAFT ||
                        ((statusIdCheckForButtonEdit == IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_STT.ID__REQUEST ||
                        statusIdCheckForButtonEdit == IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_STT.ID__REJECT) &&
                        (typeIdCheckForButtonEdit != IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__DNTTL &&
                        typeIdCheckForButtonEdit != IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__CK))))
                    {
                        ButtonEdit_ButtonClick(null, null);
                    }
                }
                else if (e.Column.FieldName == "DISCARD_DISPLAY") //hủy
                {
                    if (((_Room != null && _Room.DEPARTMENT_ID == departmentId)
                        && (statusIdCheckForButtonEdit == IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_STT.ID__DRAFT ||
                        statusIdCheckForButtonEdit == IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_STT.ID__REJECT ||
                        statusIdCheckForButtonEdit == IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_STT.ID__REQUEST)
                        && (!GlobalStore.isEnableDeleteAggregate || ((CheckLoginAdmin.IsAdmin(LoggingName) || creator.Equals(LoggingName)) && reqRoomId == _Room.ID))))
                    {
                        ButtonDiscard_ButtonClick(null, null);
                    }
                }
                else if (e.Column.FieldName == "APPROVAL_DISPLAY") //duyet
                {
                    if (medistock != null && medistock.ID == mediStockId &&
                        ((statusIdCheckForButtonEdit == IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_STT.ID__REJECT ||
                        statusIdCheckForButtonEdit == IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_STT.ID__REQUEST)))
                    {
                        ButtonApproval_ButtonClick(null, null);
                    }
                }
                else if (e.Column.FieldName == "DIS_APPROVAL")// Từ chối duyệt
                {
                    if (medistock != null && medistock.ID == mediStockId &&
                        (statusIdCheckForButtonEdit != IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_STT.ID__DRAFT &&
                        statusIdCheckForButtonEdit != IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_STT.ID__REJECT &&
                        statusIdCheckForButtonEdit != IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_STT.ID__IMPORT))
                    { ButtonDisApproval_ButtonClick(null, null); }
                }
                else if (e.Column.FieldName == "IMPORT_DISPLAY")// thực xuất
                {
                    if (statusIdCheckForButtonEdit == IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_STT.ID__APPROVAL)
                    {
                        if (medistock != null && medistock.ID == mediStockId)
                        { ButtonImport_ButtonClick(null, null); }

                    }
                    else if (statusIdCheckForButtonEdit == IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_STT.ID__IMPORT)
                    {
                        if ((creator == LoggingName || CheckLoginAdmin.IsAdmin(LoggingName)) && medistock != null && medistock.ID == mediStockId)
                        {
                            Btn_CancelImport_Enable_ButtonClick(null, null);
                        }
                    }
                }
                else if (e.Column.FieldName == "REQUEST_DISPLAY")// Hủy duyệt
                {
                    if (medistock != null && medistock.ID == mediStockId &&
                        (statusIdCheckForButtonEdit == IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_STT.ID__APPROVAL ||
                        statusIdCheckForButtonEdit == IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_STT.ID__REJECT))
                    {
                        ButtonRequest_ButtonClick(null, null);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void dtImpTimeTo_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (dtImpTimeTo.DateTime != null)
                {
                    string dateTime = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(dtImpTimeTo.DateTime).ToString().Substring(0, 8) + "235959";
                    dtImpTimeTo.DateTime = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(Inventec.Common.TypeConvert.Parse.ToInt64(dateTime)) ?? DateTime.MinValue;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
