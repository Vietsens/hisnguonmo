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
using Inventec.Desktop.Common.Message;
using HIS.Desktop.LocalStorage.ConfigApplication;
using DevExpress.Data;
using System.Collections;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.Utils;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using System.Threading;
using System.IO;
using Inventec.Common.RichEditor.Base;
using MOS.EFMODEL.DataModels;
using HIS.Desktop.LocalStorage.BackendData;
using MOS.SDO;
using Inventec.Common.Adapter;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Plugins.HisAggrExpMestList.Base;
using HIS.Desktop.IsAdmin;
using HIS.Desktop.Utility;
using HIS.Desktop.LocalStorage.HisConfig;
using HIS.Desktop.Common;
using DevExpress.XtraBars;
using MOS.Filter;
using HIS.Desktop.ApiConsumer;

namespace HIS.Desktop.Plugins.HisAggrExpMestList
{
    public partial class UCHisAggrExpMestList : HIS.Desktop.Utility.UserControlBase
    {
        #region Declare
        int rowCount = 0;
        int dataTotal = 0;
        int startPage = 0;
        private string LoggingName = "";
        MOS.EFMODEL.DataModels.V_HIS_MEDI_STOCK medistock;
        MOS.EFMODEL.DataModels.V_HIS_ROOM _Room = null;
        long roomId = 0;
        long roomTypeId = 0;
        int lastRowHandle = -1;
        ToolTipControlInfo lastInfo;
        GridColumn lastColumn = null;
        List<ACS.EFMODEL.DataModels.ACS_CONTROL> controlAcs;

        BarManager baManager = null;
        PopupMenuProcessor popupMenuProcessor = null;
        String[] CaptionPres = { "Tất cả", "Có chứa thuốc xuất hủy", "Không chứa thuốc xuất hủy" };

        #endregion

        #region Construct
        public UCHisAggrExpMestList()
        {
            InitializeComponent();
            try
            {
                Resources.ResourceLanguageManager.InitResourceLanguageManager();
                gridControl.ToolTipController = this.toolTipController;
                LoggingName = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public UCHisAggrExpMestList(Inventec.Desktop.Common.Modules.Module _moduleData)
            : base(_moduleData)
        {
            InitializeComponent();
            try
            {
                Resources.ResourceLanguageManager.InitResourceLanguageManager();
                gridControl.ToolTipController = this.toolTipController;
                LoggingName = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
                this.roomId = _moduleData.RoomId;
                this.roomTypeId = _moduleData.RoomTypeId;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void UCHisAggrExpMestList_Load(object sender, EventArgs e)
        {
            try
            {
                if (GlobalVariables.AcsAuthorizeSDO != null)
                {
                    controlAcs = GlobalVariables.AcsAuthorizeSDO.ControlInRoles;
                }
                GlobalStore.LoadConfig();
                //Gan ngon ngu
                LoadKeysFromlanguage();

                FillDataToNavBarStatus();

                //       FillDataToNavBarPres();
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
        #endregion

        #region Private method
        private void LoadKeysFromlanguage()
        {
            try
            {
                var cultureLang = Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture();
                //filter
                this.txtKeyWord.Properties.NullValuePrompt = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_HIS_AGGR_EXP_MEST_LIST__TXT_KEYWORD",
                    Resources.ResourceLanguageManager.LanguageUCHisAggrExpMestList,
                    cultureLang);
                this.navBarGroupCreateTime.Caption = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_HIS_AGGR_EXP_MEST_LIST__NAV_BAR_CREATE_TIME",
                    Resources.ResourceLanguageManager.LanguageUCHisAggrExpMestList,
                    cultureLang);
                this.lciCreateTimeForm.Text = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_HIS_AGGR_EXP_MEST_LIST__LCI_CREATE_TIME_FROM",
                    Resources.ResourceLanguageManager.LanguageUCHisAggrExpMestList,
                    cultureLang);
                this.lciCreateTimeTo.Text = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_HIS_AGGR_EXP_MEST_LIST__LCI_CREATE_TIME_TO",
                    Resources.ResourceLanguageManager.LanguageUCHisAggrExpMestList,
                    cultureLang);
                this.btnRefresh.Text = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_HIS_AGGR_EXP_MEST_LIST__BTN_REFRESH",
                    Resources.ResourceLanguageManager.LanguageUCHisAggrExpMestList,
                    cultureLang);
                this.btnSearch.Text = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_HIS_AGGR_EXP_MEST_LIST__BTN_SEARCH",
                    Resources.ResourceLanguageManager.LanguageUCHisAggrExpMestList,
                    cultureLang);
                this.navBarGroupStatus.Caption = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_HIS_AGGR_EXP_MEST_LIST__NAV_BAR_STATUS",
                    Resources.ResourceLanguageManager.LanguageUCHisAggrExpMestList,
                    cultureLang);
                this.navBarGroupExpTime.Caption = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_HIS_AGGR_EXP_MEST_LIST__NAV_BAR_EXP_TIME",
                    Resources.ResourceLanguageManager.LanguageUCHisAggrExpMestList,
                    cultureLang);
                this.lciExpTimeFrom.Text = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_HIS_AGGR_EXP_MEST_LIST__LCI_EXP_TIME_FROM",
                    Resources.ResourceLanguageManager.LanguageUCHisAggrExpMestList,
                    cultureLang);
                this.lciExpTimeTo.Text = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_HIS_AGGR_EXP_MEST_LIST__LCI_EXP_TIME_TO",
                    Resources.ResourceLanguageManager.LanguageUCHisAggrExpMestList,
                    cultureLang);
                this.btnExportList.Text = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_HIS_AGGR_EXP_MEST_LIST__BTN_EXPORT_LIST",
                    Resources.ResourceLanguageManager.LanguageUCHisAggrExpMestList,
                    cultureLang);

                //gridView
                this.GcApprovalName.Caption = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_HIS_AGGR_EXP_MEST_LIST__GRID_COLUMN__APPROVAL_NAME",
                    Resources.ResourceLanguageManager.LanguageUCHisAggrExpMestList,
                    cultureLang);
                this.GcApprovalTime.Caption = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_HIS_AGGR_EXP_MEST_LIST__GRID_COLUMN__APPROVAL_TIME",
                    Resources.ResourceLanguageManager.LanguageUCHisAggrExpMestList,
                    cultureLang);
                this.GcCreateTime.Caption = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_HIS_AGGR_EXP_MEST_LIST__GRID_COLUMN__CREATE_TIME",
                    Resources.ResourceLanguageManager.LanguageUCHisAggrExpMestList,
                    cultureLang);
                this.GcCreator.Caption = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_HIS_AGGR_EXP_MEST_LIST__GRID_COLUMN__CREATOR",
                    Resources.ResourceLanguageManager.LanguageUCHisAggrExpMestList,
                    cultureLang);
                this.GcExpLoginName.Caption = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_HIS_AGGR_EXP_MEST_LIST__GRID_COLUMN__EXP_NAME",
                    Resources.ResourceLanguageManager.LanguageUCHisAggrExpMestList,
                    cultureLang);
                this.GcExpMestCode.Caption = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_HIS_AGGR_EXP_MEST_LIST__GRID_COLUMN__EXP_MEST_CODE",
                    Resources.ResourceLanguageManager.LanguageUCHisAggrExpMestList,
                    cultureLang);
                this.GcExpMestTypeName.Caption = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_HIS_AGGR_EXP_MEST_LIST__GRID_COLUMN__EXP_MEST_TYPE_NAME",
                    Resources.ResourceLanguageManager.LanguageUCHisAggrExpMestList,
                    cultureLang);
                this.GcExpTime.Caption = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_HIS_AGGR_EXP_MEST_LIST__GRID_COLUMN__EXP_TIME",
                    Resources.ResourceLanguageManager.LanguageUCHisAggrExpMestList,
                    cultureLang);
                this.GcMediStockName.Caption = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_HIS_AGGR_EXP_MEST_LIST__GRID_COLUMN__MEDI_STOCK_NAME",
                    Resources.ResourceLanguageManager.LanguageUCHisAggrExpMestList,
                    cultureLang);
                this.GcModifier.Caption = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_HIS_AGGR_EXP_MEST_LIST__GRID_COLUMN__MODIFIER",
                    Resources.ResourceLanguageManager.LanguageUCHisAggrExpMestList,
                    cultureLang);
                this.GcModifyTime.Caption = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_HIS_AGGR_EXP_MEST_LIST__GRID_COLUMN__MODIFY_TIME",
                    Resources.ResourceLanguageManager.LanguageUCHisAggrExpMestList,
                    cultureLang);
                this.GcReqName.Caption = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_HIS_AGGR_EXP_MEST_LIST__GRID_COLUMN__REQ_NAME",
                    Resources.ResourceLanguageManager.LanguageUCHisAggrExpMestList,
                    cultureLang);
                this.GcUseTime.Caption = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_HIS_AGGR_EXP_MEST_LIST__GRID_COLUMN__USE_TIME",
                    Resources.ResourceLanguageManager.LanguageUCHisAggrExpMestList,
                    cultureLang);
                this.STT.Caption = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_HIS_AGGR_EXP_MEST_LIST__GRID_COLUMN__STT",
                    Resources.ResourceLanguageManager.LanguageUCHisAggrExpMestList,
                    cultureLang);
                this.GcReqDepartmentName.Caption = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_HIS_AGGR_EXP_MEST_LIST__GRID_COLUMN__REQ_DEPARTMENT_NAME",
                    Resources.ResourceLanguageManager.LanguageUCHisAggrExpMestList,
                    cultureLang);
                this.GcExpMestCode2.Caption = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_HIS_AGGR_EXP_MEST_LIST__GRID_COLUMN__EXP_MEST_SUB_CODE_2",
                    Resources.ResourceLanguageManager.LanguageUCHisAggrExpMestList,
                    cultureLang);
                //grid button
                this.ButtonApprovalDisable.Buttons[0].ToolTip = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_HIS_AGGR_EXP_MEST_LIST__GR_BUTTON_APPROVAL",
                    Resources.ResourceLanguageManager.LanguageUCHisAggrExpMestList,
                    cultureLang);
                this.ButtonApprovalEnable.Buttons[0].ToolTip = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_HIS_AGGR_EXP_MEST_LIST__GR_BUTTON_APPROVAL",
                    Resources.ResourceLanguageManager.LanguageUCHisAggrExpMestList,
                    cultureLang);
                this.ButtonDisApprovalDisable.Buttons[0].ToolTip = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_HIS_AGGR_EXP_MEST_LIST__GR_BUTTON_DIS_APPROVAL",
                    Resources.ResourceLanguageManager.LanguageUCHisAggrExpMestList,
                    cultureLang);
                this.ButtonDisApprovalEnable.Buttons[0].ToolTip = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_HIS_AGGR_EXP_MEST_LIST__GR_BUTTON_DIS_APPROVAL",
                    Resources.ResourceLanguageManager.LanguageUCHisAggrExpMestList,
                    cultureLang);
                this.ButtonDiscardDisable.Buttons[0].ToolTip = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_HIS_AGGR_EXP_MEST_LIST__GR_BUTTON_DISCARD",
                    Resources.ResourceLanguageManager.LanguageUCHisAggrExpMestList,
                    cultureLang);
                this.ButtonDiscardEnable.Buttons[0].ToolTip = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_HIS_AGGR_EXP_MEST_LIST__GR_BUTTON_DISCARD",
                    Resources.ResourceLanguageManager.LanguageUCHisAggrExpMestList,
                    cultureLang);
                this.ButtonEditDisable.Buttons[0].ToolTip = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_HIS_AGGR_EXP_MEST_LIST__GR_BUTTON_EDIT",
                    Resources.ResourceLanguageManager.LanguageUCHisAggrExpMestList,
                    cultureLang);
                this.ButtonEditEnable.Buttons[0].ToolTip = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_HIS_AGGR_EXP_MEST_LIST__GR_BUTTON_EDIT",
                    Resources.ResourceLanguageManager.LanguageUCHisAggrExpMestList,
                    cultureLang);
                this.ButtonExportDisable.Buttons[0].ToolTip = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_HIS_AGGR_EXP_MEST_LIST__GR_BUTTON_EXPORT",
                    Resources.ResourceLanguageManager.LanguageUCHisAggrExpMestList,
                    cultureLang);
                this.ButtonExportEnable.Buttons[0].ToolTip = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_HIS_AGGR_EXP_MEST_LIST__GR_BUTTON_EXPORT",
                    Resources.ResourceLanguageManager.LanguageUCHisAggrExpMestList,
                    cultureLang);
                this.ButtonReApproval.Buttons[0].ToolTip = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_HIS_AGGR_EXP_MEST_LIST__GR_BUTTON_RE_APPROVAL",
                    Resources.ResourceLanguageManager.LanguageUCHisAggrExpMestList,
                    cultureLang);
                this.ButtonReApprovalDisable.Buttons[0].ToolTip = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_HIS_AGGR_EXP_MEST_LIST__GR_BUTTON_RE_APPROVAL",
                    Resources.ResourceLanguageManager.LanguageUCHisAggrExpMestList,
                    cultureLang);
                this.ButtonViewDetail.Buttons[0].ToolTip = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_HIS_AGGR_EXP_MEST_LIST__GR_BUTTON_VIEW",
                    Resources.ResourceLanguageManager.LanguageUCHisAggrExpMestList,
                    cultureLang);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void FillDataToNavBarStatus()
        {
            try
            {
                navBarControlFilter.BeginUpdate();
                int d = 0;
                foreach (var item in Base.GlobalStore.HisExpMestStts)
                {
                    navBarGroupStatus.GroupClientHeight += 25;
                    DevExpress.XtraEditors.CheckEdit checkEdit = new DevExpress.XtraEditors.CheckEdit();
                    layoutControlStatus.Controls.Add(checkEdit);
                    checkEdit.Location = new System.Drawing.Point(50, 2 + (d * 23));
                    checkEdit.Name = item.ID.ToString();
                    checkEdit.Properties.Caption = item.EXP_MEST_STT_NAME;
                    checkEdit.Size = new System.Drawing.Size(150, 19);
                    checkEdit.StyleController = this.layoutControlStatus;
                    checkEdit.TabIndex = 4 + d;
                    checkEdit.EnterMoveNextControl = false;
                    d++;
                }
                navBarControlFilter.EndUpdate();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                navBarControlFilter.EndUpdate();
            }
        }
        //Style

        //private void FillDataToNavBarPres()
        //{
        //    try
        //    {
        //        navBarControlFilter.BeginUpdate();
        //        int d = 0;

        //        foreach (var item in CaptionPres)
        //        {
        //            navBarGroupPres.GroupClientHeight += 25;
        //            RadioButton rd = new RadioButton();
        //            rd.Dock = System.Windows.Forms.DockStyle.Fill;
        //            layoutControlPres.Controls.Add(rd);
        //            rd.Location = new System.Drawing.Point(50, 2 + (d * 23));

        //            rd.Text = item;
        //            rd.Size = new System.Drawing.Size(150, 19);

        //            rd.TabIndex = 8 + d;
        //            //     rd.EnterMoveNextControl = false;
        //            d++;
        //        }
        //        navBarControlFilter.EndUpdate();
        //    }
        //    catch (Exception ex)
        //    {
        //        Inventec.Common.Logging.LogSystem.Error(ex);
        //        navBarControlFilter.EndUpdate();
        //    }
        //}


        private void SetDefaultValueControl()
        {
            try
            {
                medistock = Base.GlobalStore.ListMediStock.FirstOrDefault(o => o.ROOM_ID == roomId && o.ROOM_TYPE_ID == roomTypeId);
                _Room = BackendDataWorker.Get<V_HIS_ROOM>().FirstOrDefault(o => o.ID == roomId);
                txtKeyWord.Text = "";
                txtSearchPatientCode.Text = "";
                txtSearchTreatmentCode.Text = "";
                dtCreateTimeFrom.DateTime = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(Inventec.Common.DateTime.Get.StartDay() ?? 0) ?? DateTime.MinValue;
                dtCreateTimeTo.DateTime = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(Inventec.Common.DateTime.Get.EndDay() ?? 0) ?? DateTime.MinValue;
                txtKeyWord.Focus();
                dtExpTimeFrom.EditValue = null;
                dtExpTimeTo.EditValue = null;
                SetDefaultValueStatus();
                SetDefaultValuePres();
                long showbtn = Inventec.Common.TypeConvert.Parse.ToInt64(HisConfigs.Get<string>(Base.GlobalStore.showButton));

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

        private void SetDefaultValueStatus()
        {
            try
            {
                if (layoutControlStatus.Controls.Count > 0)
                {
                    for (int i = 0; i < layoutControlStatus.Controls.Count; i++)
                    {
                        if (layoutControlStatus.Controls[i] is DevExpress.XtraEditors.CheckEdit)
                        {
                            var checkEdit = layoutControlStatus.Controls[i] as DevExpress.XtraEditors.CheckEdit;
                            if (Inventec.Common.TypeConvert.Parse.ToInt64(checkEdit.Name) == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__REQUEST || Inventec.Common.TypeConvert.Parse.ToInt64(checkEdit.Name) == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__EXECUTE)
                            {
                                checkEdit.Checked = true;
                            }
                            else
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
        private void SetDefaultValuePres()
        {
            rbtAll.Checked = true;
            //try
            //{
            //    if (layoutControlPres.Controls.Count > 0)
            //    {
            //        for (int i = 0; i < layoutControlPres.Controls.Count; i++)
            //        {
            //            if (layoutControlPres.Controls[i] is RadioButton)
            //            {
            //                var checkEdit = layoutControlPres.Controls[i] as RadioButton;
            //                if (checkEdit.Text.Equals("Tất cả"))
            //                {
            //                    checkEdit.Checked = true;
            //                }
            //            }
            //        }
            //    }
            //}
            //catch (Exception ex)
            //{
            //    Inventec.Common.Logging.LogSystem.Error(ex);
            //}
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
                ApiResultObject<List<MOS.EFMODEL.DataModels.V_HIS_EXP_MEST_3>> apiResult = null;
                MOS.Filter.HisExpMestView3Filter filter = new MOS.Filter.HisExpMestView3Filter();
                SetFilter(ref filter);
                gridView.BeginUpdate();
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData("filter(HisExpMestView3Filter)", filter));
                apiResult = new Inventec.Common.Adapter.BackendAdapter
                    (paramCommon).GetRO<List<MOS.EFMODEL.DataModels.V_HIS_EXP_MEST_3>>
                    ("api/HisExpMest/GetView3", ApiConsumer.ApiConsumers.MosConsumer, filter, paramCommon);
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData("apiResult(List<V_HIS_EXP_MEST_3>)", apiResult));
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

        private void SetFilter(ref MOS.Filter.HisExpMestView3Filter filter)
        {
            try
            {
                filter.ORDER_FIELD = "MODIFY_TIME";
                filter.ORDER_DIRECTION = "DESC";
                if (!String.IsNullOrEmpty(txtSearchTreatmentCode.Text))
                {
                    string code = txtSearchTreatmentCode.Text.Trim();
                    if (code.Length < 12 && checkDigit(code))
                    {
                        code = string.Format("{0:000000000000}", Convert.ToInt64(code));
                        txtSearchTreatmentCode.Text = code;
                    }
                    filter.TDL_AGGR_TREATMENT_CODE = code;
                    txtSearchPatientCode.Text = "";
                    txtKeyWord.Text = "";
                    return;
                }

                if (!String.IsNullOrEmpty(txtSearchPatientCode.Text))
                {
                    string code = txtSearchPatientCode.Text.Trim();
                    if (code.Length < 10 && checkDigit(code))
                    {
                        code = string.Format("{0:0000000000}", Convert.ToInt64(code));
                        txtSearchPatientCode.Text = code;
                    }
                    filter.TDL_AGGR_PATIENT_CODE = code;
                }

                filter.KEY_WORD = txtKeyWord.Text.Trim();
                filter.EXP_MEST_TYPE_ID = IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__PL;

                //if (roomId == 0)
                //{
                //    filter.CREATOR = LoggingName;
                //}
                //else
                //{
                //    filter.DATA_DOMAIN_FILTER = true;
                //    filter.WORKING_ROOM_ID = roomId;
                //}

                if (this.medistock != null)
                {
                    filter.MEDI_STOCK_ID = this.medistock.ID;
                }
                else
                {
                    filter.REQ_DEPARTMENT_ID = this._Room.DEPARTMENT_ID;
                }

                if (dtCreateTimeFrom.EditValue != null && dtCreateTimeFrom.DateTime != DateTime.MinValue)
                    filter.CREATE_TIME_FROM = Inventec.Common.TypeConvert.Parse.ToInt64(
                        Convert.ToDateTime(dtCreateTimeFrom.EditValue).ToString("yyyyMMddHHmm") + "00");

                if (dtCreateTimeTo.EditValue != null && dtCreateTimeTo.DateTime != DateTime.MinValue)
                    filter.CREATE_TIME_TO = Inventec.Common.TypeConvert.Parse.ToInt64(
                        Convert.ToDateTime(dtCreateTimeTo.EditValue).ToString("yyyyMMddHHmm") + "59");

                if (dtExpTimeFrom.EditValue != null && dtExpTimeFrom.DateTime != DateTime.MinValue)
                    filter.FINISH_TIME_FROM = Inventec.Common.TypeConvert.Parse.ToInt64(
                        Convert.ToDateTime(dtExpTimeFrom.EditValue).ToString("yyyyMMddHHmm") + "00");

                if (dtExpTimeTo.EditValue != null && dtExpTimeTo.DateTime != DateTime.MinValue)
                    filter.FINISH_TIME_TO = Inventec.Common.TypeConvert.Parse.ToInt64(
                        Convert.ToDateTime(dtExpTimeTo.EditValue).ToString("yyyyMMddHHmm") + "59");


                SetFilterStatus(ref filter);
                SetFilterPres(ref filter);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private bool checkDigit(string s)
        {
            bool result = false;
            try
            {
                for (int i = 0; i < s.Length; i++)
                {
                    if (char.IsDigit(s[i]) == true) result = true;
                    else result = false;
                }
                return result;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                return result;
            }
        }

        private void SetFilterStatus(ref MOS.Filter.HisExpMestView3Filter filter)
        {
            try
            {
                if (layoutControlStatus.Controls.Count > 0)
                {
                    for (int i = 0; i < layoutControlStatus.Controls.Count; i++)
                    {
                        if (layoutControlStatus.Controls[i] is DevExpress.XtraEditors.CheckEdit)
                        {
                            var checkEdit = layoutControlStatus.Controls[i] as DevExpress.XtraEditors.CheckEdit;
                            if (checkEdit.Checked)
                            {
                                if (filter.EXP_MEST_STT_IDs == null)
                                    filter.EXP_MEST_STT_IDs = new List<long>();
                                filter.EXP_MEST_STT_IDs.Add(Inventec.Common.TypeConvert.Parse.ToInt64(checkEdit.Name));
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
        private void SetFilterPres(ref MOS.Filter.HisExpMestView3Filter filter)
        {
            try
            {
                if (rbtAll.Checked)
                {
                    filter.HAS_NOT_PRES = null;
                }
                else if (rbtHasNotPres.Checked)
                {
                    filter.HAS_NOT_PRES = false;
                }
                else if (rbtHasPres.Checked)
                {
                    filter.HAS_NOT_PRES = true;
                }

                //if (layoutControlStatus.Controls.Count > 0)
                //{
                //    for (int i = 0; i < layoutControlPres.Controls.Count; i++)
                //    {
                //        if (layoutControlPres.Controls[i] is RadioButton)
                //        {
                //            var rd = layoutControlPres.Controls[i] as RadioButton;
                //            if (rd.Checked)
                //            {
                //                if (rd.Text.Equals("Tất cả"))
                //                {
                //                    filter.HAS_NOT_PRES = null;
                //                    Inventec.Common.Logging.LogSystem.Error(null + " ######");
                //                }
                //                else if (rd.Text.Equals("Có chứa thuốc xuất hủy"))
                //                {
                //                    filter.HAS_NOT_PRES = true;
                //                    Inventec.Common.Logging.LogSystem.Error(true + " ######");
                //                }
                //                else
                //                {
                //                    filter.HAS_NOT_PRES = false;
                //                    Inventec.Common.Logging.LogSystem.Error(false + " ######");
                //                }
                //            }


                //        }
                //    }
                //}
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
                SetDefaultValueControl();
                FillDataToGrid();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridView_CustomRowCellEdit(object sender, DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs e)
        {
            try
            {
                if (e.RowHandle >= 0)
                {
                    // V_HIS_EXP_MEST hisExpMestData = (V_HIS_EXP_MEST)gridView.GetRow(e.RowHandle);
                    long statusIdCheckForButtonEdit = Inventec.Common.TypeConvert.Parse.ToInt64((gridView.GetRowCellValue(e.RowHandle, "EXP_MEST_STT_ID") ?? "0").ToString());
                    long mediStockId = Inventec.Common.TypeConvert.Parse.ToInt64((gridView.GetRowCellValue(e.RowHandle, "MEDI_STOCK_ID") ?? "0").ToString());
                    long expMestTypeId = Inventec.Common.TypeConvert.Parse.ToInt64((gridView.GetRowCellValue(e.RowHandle, "EXP_MEST_TYPE_ID") ?? "0").ToString());
                    string creator = (gridView.GetRowCellValue(e.RowHandle, "CREATOR") ?? "").ToString().Trim();
                    long departmentId = Inventec.Common.TypeConvert.Parse.ToInt64((gridView.GetRowCellValue(e.RowHandle, "REQ_DEPARTMENT_ID") ?? "0").ToString());
                    long reqRoomId = Inventec.Common.TypeConvert.Parse.ToInt64((gridView.GetRowCellValue(e.RowHandle, "REQ_ROOM_ID") ?? "0").ToString());
                    if (e.Column.FieldName == "EDIT_DISPLAY") // sửa
                    {
                        if ((_Room != null && _Room.DEPARTMENT_ID == departmentId)
                            && (statusIdCheckForButtonEdit == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__DRAFT
                            || statusIdCheckForButtonEdit == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__REJECT
                            || statusIdCheckForButtonEdit == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__REQUEST)
                            )
                        {
                            e.RepositoryItem = ButtonEditEnable;
                        }
                        else
                            e.RepositoryItem = ButtonEditDisable;
                    }
                    else if (e.Column.FieldName == "DISCARD_DISPLAY") //hủy
                    {
                        if ((_Room != null && _Room.DEPARTMENT_ID == departmentId)
                            && (statusIdCheckForButtonEdit == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__DRAFT
                            || statusIdCheckForButtonEdit == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__REJECT
                            || statusIdCheckForButtonEdit == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__REQUEST)
                            )
                        {
                            if (GlobalStore.isEnableDeleteAggregate)
                            {
                                if ((CheckLoginAdmin.IsAdmin(LoggingName) || creator.Equals(LoggingName)) && reqRoomId == _Room.ID)
                                    e.RepositoryItem = ButtonDiscardEnable;
                                else
                                    e.RepositoryItem = ButtonDiscardDisable;
                            }
                            else
                                e.RepositoryItem = ButtonDiscardEnable;
                        }
                        else
                            e.RepositoryItem = ButtonDiscardDisable;
                    }
                    else if (e.Column.FieldName == "APPROVAL_DISPLAY") //duyet
                    {
                        if (controlAcs != null && controlAcs.FirstOrDefault(o => o.CONTROL_CODE == ControlCode.BtnApprove) != null)
                        {
                            if (medistock != null && (statusIdCheckForButtonEdit == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__REQUEST))
                            {
                                if (medistock.ID == mediStockId)
                                    e.RepositoryItem = ButtonApprovalEnable;
                                else
                                    e.RepositoryItem = ButtonApprovalDisable;
                            }
                            else
                                e.RepositoryItem = ButtonApprovalDisable;
                        }
                        else
                            e.RepositoryItem = ButtonApprovalDisable;
                    }
                    else if (e.Column.FieldName == "DIS_APPROVAL")// Không duyệt
                    {
                        if (medistock != null && (statusIdCheckForButtonEdit != IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__EXECUTE
                            && statusIdCheckForButtonEdit != IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__DONE
                            && statusIdCheckForButtonEdit != IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__REJECT))
                        {
                            if (medistock.ID == mediStockId)
                                e.RepositoryItem = ButtonDisApprovalEnable;
                            else
                                e.RepositoryItem = ButtonDisApprovalDisable;
                        }
                        else
                            e.RepositoryItem = ButtonDisApprovalDisable;
                    }
                    else if (e.Column.FieldName == "EXPORT_DISPLAY")// thực xuất
                    {
                        if (medistock != null && statusIdCheckForButtonEdit == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__EXECUTE)
                        {
                            if (medistock.ID == mediStockId)
                                e.RepositoryItem = ButtonExportEnable;
                            else
                                e.RepositoryItem = ButtonExportDisable;
                        }
                        else if (medistock != null && statusIdCheckForButtonEdit == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__DONE)
                        {
                            if (medistock.ID == mediStockId)
                                e.RepositoryItem = Btn_HuyThucXuat_Enable;
                            else
                                e.RepositoryItem = Btn_HuyThucXuat_Disable;
                        }
                        else
                            e.RepositoryItem = ButtonExportDisable;
                    }
                    else if (e.Column.FieldName == "RE_APPROVAL")// Không duyệt
                    {
                        if (medistock != null && statusIdCheckForButtonEdit == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__EXECUTE)
                        {
                            if (medistock.ID == mediStockId)
                                e.RepositoryItem = ButtonReApproval;
                            else
                                e.RepositoryItem = ButtonReApprovalDisable;
                        }
                        else
                            e.RepositoryItem = ButtonReApprovalDisable;
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
                    MOS.EFMODEL.DataModels.V_HIS_EXP_MEST_3 data = (MOS.EFMODEL.DataModels.V_HIS_EXP_MEST_3)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    if (data != null)
                    {
                        if (e.Column.FieldName == "STT")
                        {
                            e.Value = e.ListSourceRowIndex + 1 + startPage;
                        }
                        else if (e.Column.FieldName == "EXP_MEST_STT_ICON")// trạng thái
                        {
                            if (data.EXP_MEST_STT_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__DRAFT) //tam thoi
                            {
                                e.Value = imageListStatus.Images[0];
                            }
                            else if (data.EXP_MEST_STT_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__REQUEST) //yeu cau
                            {
                                e.Value = imageListStatus.Images[1];
                            }
                            else if (data.EXP_MEST_STT_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__REJECT) // tu choi duyet
                            {
                                e.Value = imageListStatus.Images[2];
                            }
                            else if (data.EXP_MEST_STT_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__EXECUTE) // duyet
                            {
                                e.Value = imageListStatus.Images[3];
                            }
                            else if (data.EXP_MEST_STT_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__DONE) // da xuat
                            {
                                e.Value = imageListStatus.Images[4];
                            }
                        }
                        else if (e.Column.FieldName == "CREATE_TIME_DISPLAY")
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(data.CREATE_TIME ?? 0);
                        }
                        else if (e.Column.FieldName == "TDL_INTRUCTION_DATE_MIN_DISPLAY")
                        {
                            // e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString((long)(data.TDL_INTRUCTION_DATE_MIN ?? 0));
                            string dateTime = Inventec.Common.DateTime.Convert.TimeNumberToTimeString((long)(data.TDL_INTRUCTION_DATE_MIN ?? 0));
                            if (dateTime.Length >= 10)
                            {
                                e.Value = dateTime.Substring(0, 10);
                            }
                            else
                            {
                                e.Value = dateTime;
                            }
                        }
                        else if (e.Column.FieldName == "EXP_TIME_DISPLAY")
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(data.FINISH_TIME ?? 0);
                        }
                        else if (e.Column.FieldName == "MODIFY_TIME_DISPLAY")
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(data.MODIFY_TIME ?? 0);
                        }
                        else if (e.Column.FieldName == "APPROVAL_TIME_DISPLAY")
                        {
                            //e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(data.APPROVAL_TIME ?? 0);
                        }
                        else if (e.Column.FieldName == "USE_TIME_DISPLAY")
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(data.AGGR_USE_TIME ?? 0);
                        }
                        else if (e.Column.FieldName == "APPROVAL_LOGINNAME_DISPLAY")
                        {
                            //string APPROVAL_LOGINNAME = data.APPROVAL_LOGINNAME;
                            //string APPROVAL_USERNAME = data.APPROVAL_USERNAME;
                            //e.Value = DisplayName(APPROVAL_LOGINNAME, APPROVAL_USERNAME);
                        }
                        else if (e.Column.FieldName == "EXP_LOGINNAME_DISPLAY")
                        {
                            //string IMP_LOGINNAME = data.EXP_LOGINNAME;
                            //string IMP_USERNAME = data.EXP_USERNAME;
                            //e.Value = DisplayName(IMP_LOGINNAME, IMP_USERNAME);
                        }
                        else if (e.Column.FieldName == "REQ_LOGINNAME_DISPLAY")
                        {
                            string Req_loginName = data.REQ_LOGINNAME;
                            string Req_UserName = data.REQ_USERNAME;
                            e.Value = DisplayName(Req_loginName, Req_UserName);
                        }
                        else if (e.Column.FieldName == "REQ_DEPARTMENT_NAME")
                        {
                            if (data.REQ_DEPARTMENT_ID != null)
                            {
                                e.Value = Base.GlobalStore.ListDepartment.FirstOrDefault(o => o.ID == data.REQ_DEPARTMENT_ID).DEPARTMENT_NAME;
                            }
                            else
                                e.Value = "";
                        }
                        else if (e.Column.FieldName == "EXP_MEST_SUB_CODE_2")
                        {
                            Inventec.Common.Logging.LogSystem.Error("________________$$$$$" + data.EXP_MEST_SUB_CODE_2.ToString());
                            if (data.EXP_MEST_SUB_CODE_2 != null)
                                e.Value = data.EXP_MEST_SUB_CODE_2;
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
                            if (info.Column.FieldName == "EXP_MEST_STT_NAME")
                            {
                                text = (view.GetRowCellValue(lastRowHandle, "EXP_MEST_STT_NAME") ?? "").ToString();
                            }
                            else if (info.Column.FieldName == "EXP_MEST_STT_ICON")
                            {
                                text = (view.GetRowCellValue(lastRowHandle, "EXP_MEST_STT_NAME") ?? "").ToString();
                            }
                            else //if (info.Column.FieldName == "EXP_MEST_CODE")
                            {
                                var a = (view.GetRowCellValue(lastRowHandle, "HAS_NOT_PRES") ?? "").ToString();
                                if (a == "1")
                                {
                                    text = "Phiếu có chứa thuốc xuất hủy cho bệnh nhân (là phần bổ sung thêm nhằm làm tròn khi xuất trong trường hợp bệnh nhân được kê số lượng thuốc sử dụng lẻ)";
                                }
                            }

                            lastInfo = new ToolTipControlInfo(new DevExpress.XtraGrid.GridToolTipInfo(view, new DevExpress.XtraGrid.Views.Base.CellToolTipInfo(info.RowHandle, info.Column, "Text")), text);
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
                btnExportCodeList_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        #endregion

        #region Report
        private void btnExportCodeList_Click(object sender, EventArgs e)
        {
            try
            {
                if (!btnExportList.Enabled) return;

                if (dtExpTimeFrom.EditValue == null || dtExpTimeTo.EditValue == null)
                {
                    MessageBox.Show(Resources.ResourceMessage.BanChuaChonThoiGianThucXuat);
                    if (dtExpTimeFrom.EditValue == null)
                    {
                        dtExpTimeFrom.Focus();
                        dtExpTimeFrom.SelectAll();
                    }
                    else if (dtExpTimeTo.EditValue == null)
                    {
                        dtExpTimeTo.Focus();
                        dtExpTimeTo.SelectAll();
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

                string templateFile = System.IO.Path.Combine(Application.StartupPath + "\\Tmp\\Exp", "DanhSachCacMaPhieuLinh.xlsx");

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

                    //getdata
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
                singleTag.AddSingleKey(store, "TYPE", "THỰC XUẤT");
                singleTag.AddSingleKey(store, "MEDI_STOCK_NAME", medistock.MEDI_STOCK_NAME.ToUpper());
                singleTag.AddSingleKey(store, "EXP_TIME_FROM", dtExpTimeFrom.DateTime.ToString("dd/MM/yyyy"));
                singleTag.AddSingleKey(store, "EXP_TIME_TO", dtExpTimeTo.DateTime.ToString("dd/MM/yyyy"));
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
                MOS.Filter.HisExpMestViewFilter expFilter = new MOS.Filter.HisExpMestViewFilter();
                expFilter.EXP_MEST_STT_ID = IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__DONE;
                expFilter.DATA_DOMAIN_FILTER = true;
                expFilter.WORKING_ROOM_ID = roomId;
                expFilter.EXP_MEST_TYPE_ID = IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__PL;

                if (dtExpTimeFrom.EditValue != null && dtExpTimeFrom.DateTime != DateTime.MinValue)
                    expFilter.FINISH_DATE_FROM = Inventec.Common.TypeConvert.Parse.ToInt64(
                        dtExpTimeFrom.DateTime.ToString("yyyyMMdd") + "000000");

                if (dtExpTimeTo.EditValue != null && dtExpTimeTo.DateTime != DateTime.MinValue)
                    expFilter.FINISH_DATE_TO = Inventec.Common.TypeConvert.Parse.ToInt64(
                        dtExpTimeTo.DateTime.ToString("yyyyMMdd") + "000000");

                var exportList = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<MOS.EFMODEL.DataModels.V_HIS_EXP_MEST>>(ApiConsumer.HisRequestUriStore.HIS_EXP_MEST_GETVIEW, ApiConsumer.ApiConsumers.MosConsumer, expFilter, param);
                if (exportList != null && exportList.Count > 0)
                {
                    exportList = exportList.Where(o => o.MEDI_STOCK_ID == medistock.ID).ToList();
                    expCode = exportList.Select(s => s.EXP_MEST_CODE).OrderBy(o => o).ToList();
                }
            }
            catch (Exception ex)
            {
                expCode = null;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        #endregion

        //private void Btn_HuyThucXuat_Enable_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        //{
        //    try
        //    {
        //        bool success = false;
        //        CommonParam param = new CommonParam();
        //        MOS.EFMODEL.DataModels.V_HIS_EXP_MEST row = (MOS.EFMODEL.DataModels.V_HIS_EXP_MEST)gridView.GetFocusedRow();
        //        if (row != null)
        //        {

        //            WaitingManager.Show();
        //            HisExpMestSDO sdo = new HisExpMestSDO();
        //            sdo.ExpMestId = row.ID;
        //            sdo.ReqRoomId = this.roomId;
        //            //sdo.IsFinish = true;
        //            var apiresult = new Inventec.Common.Adapter.BackendAdapter
        //                (param).Post<MOS.EFMODEL.DataModels.HIS_EXP_MEST>
        //                ("api/HisExpMest/AggrUnexport", ApiConsumer.ApiConsumers.MosConsumer, sdo, param);
        //            if (apiresult != null)
        //            {
        //                success = true;
        //                FillDataToGrid();
        //            }
        //            WaitingManager.Hide();
        //            #region Show message
        //            Inventec.Desktop.Common.Message.MessageManager.Show(this.ParentForm, param, success);
        //            #endregion

        //            #region Process has exception
        //            HIS.Desktop.Controls.Session.SessionManager.ProcessTokenLost(param);
        //            #endregion

        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Inventec.Common.Logging.LogSystem.Error(ex);
        //        WaitingManager.Hide();
        //    }
        //}

        private void btnInTraDoiTongHop_Click(object sender, EventArgs e)
        {
            try
            {
                List<V_HIS_EXP_MEST_3> _ExpMestTraDoiChecks = new List<V_HIS_EXP_MEST_3>();
                if (gridView.RowCount > 0)
                {
                    for (int i = 0; i < gridView.SelectedRowsCount; i++)
                    {
                        if (gridView.GetSelectedRows()[i] >= 0)
                        {
                            _ExpMestTraDoiChecks.Add((V_HIS_EXP_MEST_3)gridView.GetRow(gridView.GetSelectedRows()[i]));
                        }
                    }
                }
                if (_ExpMestTraDoiChecks != null && _ExpMestTraDoiChecks.Count > 0)
                {
                    Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.AggrExpMestPrintFilter").FirstOrDefault();
                    if (moduleData == null) Inventec.Common.Logging.LogSystem.Error("khong tim thay moduleLink = HIS.Desktop.Plugins.AggrExpMestPrintFilter");
                    if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                    {
                        List<object> listArgs = new List<object>();
                        listArgs.Add(_ExpMestTraDoiChecks);
                        listArgs.Add((long)5);
                        listArgs.Add(PluginInstance.GetModuleWithWorkingRoom(moduleData, this.roomId, this.roomTypeId));
                        var extenceInstance = PluginInstance.GetPluginInstance(PluginInstance.GetModuleWithWorkingRoom(moduleData, this.roomId, this.roomTypeId), listArgs);
                        if (extenceInstance == null) throw new ArgumentNullException("moduleData is null");
                        if (extenceInstance.GetType() == typeof(bool))
                        {
                            return;
                        }
                        ((Form)extenceInstance).ShowDialog();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (gridView.RowCount > 0 && gridView.SelectedRowsCount > 0)
                {
                    btnInTraDoiTongHop.Enabled = true;
                }
                else
                {
                    btnInTraDoiTongHop.Enabled = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridView_MouseDown(object sender, MouseEventArgs e)
        {
            try
            {
                DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                GridHitInfo hi = view.CalcHitInfo(e.Location);
                if ((Control.ModifierKeys & Keys.Control) != Keys.Control)
                {
                    if (hi.InRowCell)
                    {
                        var ExpMestData3 = (V_HIS_EXP_MEST_3)gridView.GetRow(hi.RowHandle);
                        MOS.EFMODEL.DataModels.V_HIS_EXP_MEST ExpMestData = new MOS.EFMODEL.DataModels.V_HIS_EXP_MEST();
                        Inventec.Common.Mapper.DataObjectMapper.Map<MOS.EFMODEL.DataModels.V_HIS_EXP_MEST>(ExpMestData, ExpMestData3);
                        string loginName = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
                        if (ExpMestData != null)
                        {
                            if (hi.Column.FieldName == "DETAIL_DATA_DISPLAY")
                            {
                                #region ----- DETAIL_DATA_DISPLAY -----
                                try
                                {
                                    if (ExpMestData != null)
                                    {
                                        WaitingManager.Show();
                                        HIS.Desktop.ADO.ApproveAggrExpMestSDO exeMestView = new HIS.Desktop.ADO.ApproveAggrExpMestSDO(ExpMestData.ID, ExpMestData.EXP_MEST_STT_ID);
                                        List<object> listArgs = new List<object>();
                                        listArgs.Add(ExpMestData);
                                        listArgs.Add((DelegateSelectData)delegateSelectData);
                                        CallModule callModule = new CallModule(CallModule.AggrExpMestDetail, this.roomId, this.roomTypeId, listArgs);

                                        WaitingManager.Hide();
                                    }

                                }
                                catch (Exception ex)
                                {
                                    WaitingManager.Hide();
                                    Inventec.Common.Logging.LogSystem.Error(ex);
                                }
                                #endregion
                            }
                            //else if (hi.Column.FieldName == "DISCARD_DISPLAY")
                            //{
                            //    #region ----- DISCARD_DISPLAY -----
                            //    if ((_Room != null && _Room.DEPARTMENT_ID == ExpMestData.REQ_DEPARTMENT_ID)
                            //    && (ExpMestData.EXP_MEST_STT_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__DRAFT
                            //    || ExpMestData.EXP_MEST_STT_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__REJECT
                            //    || ExpMestData.EXP_MEST_STT_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__REQUEST)
                            //    )
                            //    {
                            //        CommonParam param = new CommonParam();
                            //        try
                            //        {
                            //            bool success = false;
                            //            if (DevExpress.XtraEditors.XtraMessageBox.Show(
                            //                Resources.ResourceMessage.HeThongTBCuaSoThongBaoBanCoMuonHuyDuLieuKhong,
                            //                Resources.ResourceMessage.ThongBao,
                            //                MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                            //            {
                            //                var row = (MOS.EFMODEL.DataModels.V_HIS_EXP_MEST)gridView.GetFocusedRow();
                            //                if (row != null)
                            //                {
                            //                    WaitingManager.Show();
                            //                    var apiresul = new Inventec.Common.Adapter.BackendAdapter
                            //                        (param).Post<bool>
                            //                        ("api/HisExpMest/AggrDelete", ApiConsumer.ApiConsumers.MosConsumer, row.ID, param);
                            //                    if (apiresul)
                            //                    {
                            //                        success = true;
                            //                        FillDataToGrid();
                            //                    }
                            //                    WaitingManager.Hide();
                            //                    Inventec.Desktop.Common.Message.MessageManager.Show(this.ParentForm, param, success);

                            //                }
                            //            }
                            //        }
                            //        catch (Exception ex)
                            //        {
                            //            Inventec.Common.Logging.LogSystem.Error(ex);
                            //            WaitingManager.Hide();
                            //        }
                            //    }
                            //    #endregion
                            //}
                            else if (hi.Column.FieldName == "APPROVAL_DISPLAY")
                            {
                                #region ----- APPROVAL_DISPLAY -----
                                try
                                {
                                    if (controlAcs != null && controlAcs.FirstOrDefault(o => o.CONTROL_CODE == ControlCode.BtnApprove) != null)
                                    {
                                        if (medistock != null && medistock.ID == ExpMestData.MEDI_STOCK_ID && (ExpMestData.EXP_MEST_STT_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__REQUEST))
                                        {
                                            try
                                            {
                                                List<V_HIS_EXP_MEST> expMestChilds = this.GetChildExpMestFromAggExpMest(ExpMestData.ID);
                                                List<long> expMestGetIds = new List<long>();
                                                expMestGetIds.Add(ExpMestData.ID);
                                                if (expMestChilds != null && expMestChilds.Count() > 0)
                                                {
                                                    expMestGetIds.AddRange(expMestChilds.Select(o => o.ID).ToList());
                                                }

                                                // get expMestMedicine
                                                MOS.Filter.HisExpMestMedicineFilter filter = new HisExpMestMedicineFilter();
                                                filter.EXP_MEST_IDs = expMestGetIds;
                                                var expMestMedicines = new BackendAdapter(new CommonParam()).Get<List<HIS_EXP_MEST_MEDICINE>>(ApiConsumer.HisRequestUriStore.HIS_EXP_MEST_MEDICINE_GET, ApiConsumer.ApiConsumers.MosConsumer, filter, null);
                                                if (expMestMedicines != null && expMestMedicines.Count() > 0)
                                                {
                                                    string message = "";
                                                    foreach (var item in expMestMedicines)
                                                    {
                                                        var medicineType = BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>().FirstOrDefault(o => o.ID == item.TDL_MEDICINE_TYPE_ID && o.IS_STAR_MARK == 1);
                                                        if (medicineType != null)
                                                        {
                                                            message += medicineType.MEDICINE_TYPE_NAME + " số lượng: " + item.AMOUNT + "; ";
                                                        }
                                                    }

                                                    if (!String.IsNullOrEmpty(message))
                                                    {
                                                        message = String.Format("Phiếu lĩnh có thuốc * gồm: {0} \nBạn có đồng ý duyệt?", message);
                                                        if (MessageBox.Show(message, "Thông báo", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                                                            return;
                                                    }
                                                }
                                                bool success = false;
                                                CommonParam param = new CommonParam();
                                                WaitingManager.Show();
                                                HisExpMestSDO sdo = new HisExpMestSDO();
                                                sdo.ExpMestId = ExpMestData.ID;
                                                sdo.ReqRoomId = this.roomId;
                                                var apiresult = new Inventec.Common.Adapter.BackendAdapter
                                                    (param).Post<List<MOS.EFMODEL.DataModels.HIS_EXP_MEST>>
                                                    ("api/HisExpMest/AggrApprove", ApiConsumer.ApiConsumers.MosConsumer, sdo, param);
                                                if (apiresult != null && apiresult.Count > 0)
                                                {
                                                    success = true;
                                                    FillDataToGrid();
                                                }
                                                WaitingManager.Hide();
                                                Inventec.Desktop.Common.Message.MessageManager.Show(this.ParentForm, param, success);
                                            }
                                            catch (Exception ex)
                                            {
                                                Inventec.Common.Logging.LogSystem.Error(ex);
                                                WaitingManager.Hide();
                                            }
                                        }
                                    }
                                }
                                catch (Exception ex)
                                {
                                    Inventec.Common.Logging.LogSystem.Error(ex);
                                }
                                #endregion
                            }
                            else if (hi.Column.FieldName == "RE_APPROVAL")
                            {
                                #region ----- RE_APPROVAL -----
                                if (medistock != null && medistock.ID == ExpMestData.MEDI_STOCK_ID
                                    && ExpMestData.EXP_MEST_STT_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__EXECUTE)
                                {
                                    try
                                    {
                                        WaitingManager.Show();
                                        bool success = false;
                                        CommonParam param = new CommonParam();
                                        HisExpMestSDO data = new HisExpMestSDO();
                                        data.ExpMestId = ExpMestData.ID;
                                        data.ReqRoomId = this.roomId;
                                        var apiresul = new Inventec.Common.Adapter.BackendAdapter(param).Post<HIS_EXP_MEST>("api/HisExpMest/AggrUnapprove", ApiConsumer.ApiConsumers.MosConsumer, data, param);
                                        if (apiresul != null)
                                        {
                                            success = true;
                                            FillDataToGrid();
                                        }
                                        WaitingManager.Hide();
                                        Inventec.Desktop.Common.Message.MessageManager.Show(this.ParentForm, param, success);
                                    }
                                    catch (Exception ex)
                                    {
                                        WaitingManager.Hide();
                                        Inventec.Common.Logging.LogSystem.Error(ex);
                                    }
                                }
                                #endregion
                            }
                            else if (hi.Column.FieldName == "EXPORT_DISPLAY")
                            {
                                #region ----- EXPORT_DISPLAY -----
                                if (medistock != null && medistock.ID == ExpMestData.MEDI_STOCK_ID)
                                {
                                    if (ExpMestData.EXP_MEST_STT_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__EXECUTE)
                                    {
                                        #region ----- ThucXuat -----
                                        try
                                        {
                                            bool success = false;
                                            CommonParam param = new CommonParam();
                                            WaitingManager.Show();
                                            HisExpMestSDO sdo = new HisExpMestSDO();
                                            sdo.ExpMestId = ExpMestData.ID;
                                            sdo.ReqRoomId = this.roomId;
                                            var apiresult = new Inventec.Common.Adapter.BackendAdapter
                                                (param).Post<MOS.EFMODEL.DataModels.HIS_EXP_MEST>
                                                (RequestUriStore.HIS_EXP_MEST_AGGREXPORT, ApiConsumer.ApiConsumers.MosConsumer, sdo, param);
                                            if (apiresult != null)
                                            {
                                                success = true;
                                                FillDataToGrid();
                                            }
                                            WaitingManager.Hide();
                                            Inventec.Desktop.Common.Message.MessageManager.Show(this.ParentForm, param, success);
                                        }
                                        catch (Exception ex)
                                        {
                                            Inventec.Common.Logging.LogSystem.Error(ex);
                                            WaitingManager.Hide();
                                        }
                                        #endregion
                                    }
                                    else if (ExpMestData.EXP_MEST_STT_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__DONE)
                                    {
                                        #region ----- HuyThucXuat -----
                                        try
                                        {
                                            bool success = false;
                                            CommonParam param = new CommonParam();
                                            WaitingManager.Show();
                                            HisExpMestSDO sdo = new HisExpMestSDO();
                                            sdo.ExpMestId = ExpMestData.ID;
                                            sdo.ReqRoomId = this.roomId;
                                            //sdo.IsFinish = true;
                                            var apiresult = new Inventec.Common.Adapter.BackendAdapter
                                                (param).Post<MOS.EFMODEL.DataModels.HIS_EXP_MEST>
                                                ("api/HisExpMest/AggrUnexport", ApiConsumer.ApiConsumers.MosConsumer, sdo, param);
                                            if (apiresult != null)
                                            {
                                                success = true;
                                                FillDataToGrid();
                                            }
                                            WaitingManager.Hide();
                                            Inventec.Desktop.Common.Message.MessageManager.Show(this.ParentForm, param, success);
                                        }
                                        catch (Exception ex)
                                        {
                                            Inventec.Common.Logging.LogSystem.Error(ex);
                                            WaitingManager.Hide();
                                        }
                                        #endregion
                                    }
                                }
                                #endregion
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


        private void gridView_PopupMenuShowing(object sender, PopupMenuShowingEventArgs e)
        {
            try
            {
                GridHitInfo hi = e.HitInfo;
                if (hi.InRowCell)
                {
                    var expMest = (V_HIS_EXP_MEST_3)gridView.GetFocusedRow();
                    if (this.baManager == null)
                    {
                        this.baManager = new BarManager();
                        this.baManager.Form = this;
                    }
                    if (expMest != null)
                    {
                        List<V_HIS_EXP_MEST_3> _ExpMestTraDoiChecks = new List<V_HIS_EXP_MEST_3>();
                        if (gridView.RowCount > 0)
                        {
                            for (int i = 0; i < gridView.SelectedRowsCount; i++)
                            {
                                if (gridView.GetSelectedRows()[i] >= 0)
                                {
                                    _ExpMestTraDoiChecks.Add((V_HIS_EXP_MEST_3)gridView.GetRow(gridView.GetSelectedRows()[i]));
                                }
                            }
                        }
                        this.popupMenuProcessor = new PopupMenuProcessor(expMest, _ExpMestTraDoiChecks, this.baManager, MouseRightItemClick);
                        this.popupMenuProcessor.InitMenu();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void MouseRightItemClick(object sender, ItemClickEventArgs e)
        {
            try
            {
                if ((e.Item is BarButtonItem))
                {
                    var type = (PopupMenuProcessor.ItemType)e.Item.Tag;
                    switch (type)
                    {
                        case PopupMenuProcessor.ItemType.PhieuCongKhaiTheoBN:
                            this.OnClickInPhieuCongKhaiTheoBenhNhan();
                            break;
                        case PopupMenuProcessor.ItemType.InPhieuLinhTongHop:
                            this.OnClickInPhieuLinhTongHop();
                            break;
                        case PopupMenuProcessor.ItemType.InTraDoiThuocTongHop:
                            btnInTraDoiTongHop_Click(null, null);
                            break;
                        default:
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void OnClickInPhieuLinhTongHop()
        {
            try
            {
                ShowFormFilter(3);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ShowFormFilter(long printType)
        {
            try
            {
                Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.AggrExpMestPrintFilter").FirstOrDefault();
                if (moduleData == null) Inventec.Common.Logging.LogSystem.Error("khong tim thay moduleLink = HIS.Desktop.Plugins.AggrExpMestPrintFilter");
                if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                {
                    //Review
                    List<object> listArgs = new List<object>();
                    List<V_HIS_EXP_MEST_3> _ExpMestTraDoiChecks = new List<V_HIS_EXP_MEST_3>();
                    if (gridView.RowCount > 0)
                    {
                        for (int i = 0; i < gridView.SelectedRowsCount; i++)
                        {
                            if (gridView.GetSelectedRows()[i] >= 0)
                            {
                                _ExpMestTraDoiChecks.Add((V_HIS_EXP_MEST_3)gridView.GetRow(gridView.GetSelectedRows()[i]));
                            }
                        }
                    }

                    if (_ExpMestTraDoiChecks != null && _ExpMestTraDoiChecks.Count > 0)
                    {
                        listArgs.Add(_ExpMestTraDoiChecks);
                    }
                    else
                    {
                        var expMest = (V_HIS_EXP_MEST_3)gridView.GetFocusedRow();
                        listArgs.Add(expMest);
                    }

                    listArgs.Add(printType);
                    listArgs.Add(PluginInstance.GetModuleWithWorkingRoom(moduleData, this.roomId, this.roomTypeId));
                    var extenceInstance = PluginInstance.GetPluginInstance(PluginInstance.GetModuleWithWorkingRoom(moduleData, this.roomId, this.roomTypeId), listArgs);
                    if (extenceInstance == null) throw new ArgumentNullException("moduleData is null");
                    if (extenceInstance.GetType() == typeof(bool))
                    {
                        return;
                    }
                    ((Form)extenceInstance).ShowDialog();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void OnClickInPhieuCongKhaiTheoBenhNhan()
        {
            try
            {
                InPhieuCongKhaiTheoBN();
                //Inventec.Common.RichEditor.RichEditorStore richEditorMain = new Inventec.Common.RichEditor.RichEditorStore(HIS.Desktop.ApiConsumer.ApiConsumers.SarConsumer, HIS.Desktop.LocalStorage.ConfigSystem.ConfigSystems.URI_API_SAR, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetLanguage(), HIS.Desktop.LocalStorage.Location.PrintStoreLocation.PrintTemplatePath);
                //richEditorMain.RunPrintTemplate("Mps000262", DelegateRunPrinter);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        //private bool DelegateRunPrinter(string printTypeCode, string fileName)
        //{
        //    bool result = false;
        //    try
        //    {
        //        switch (printTypeCode)
        //        {
        //            case "Mps000262":
        //                InPhieuCongKhaiTheoBN(printTypeCode, fileName, ref result);
        //                break;
        //            default:
        //                break;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Inventec.Common.Logging.LogSystem.Error(ex);
        //    }

        //    return result;
        //}

        // lấy các phiếu con từ phiếu lĩnh được chọn
        List<V_HIS_EXP_MEST> GetChildExpMestFromAggExpMest(long AggExpMestId)
        {
            List<V_HIS_EXP_MEST> result = new List<V_HIS_EXP_MEST>();
            try
            {
                if (AggExpMestId > 0)
                {
                    CommonParam param = new CommonParam();
                    MOS.Filter.HisExpMestViewFilter expMestViewFilter = new HisExpMestViewFilter();
                    expMestViewFilter.AGGR_EXP_MEST_ID = AggExpMestId;

                    result = new BackendAdapter(param).Get<List<V_HIS_EXP_MEST>>("api/HisExpMest/GetView", ApiConsumer.ApiConsumers.MosConsumer, expMestViewFilter, param);
                }
            }
            catch (Exception ex)
            {
                result = new List<V_HIS_EXP_MEST>();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        private void InPhieuCongKhaiTheoBN()
        {
            try
            {
                List<V_HIS_EXP_MEST_MEDICINE> expMestMedicineTemps = new List<V_HIS_EXP_MEST_MEDICINE>();
                List<V_HIS_EXP_MEST_MATERIAL> expMestMaterialTemps = new List<V_HIS_EXP_MEST_MATERIAL>();
                var AggExpMest = (V_HIS_EXP_MEST_3)gridView.GetFocusedRow();

                List<V_HIS_EXP_MEST> expMestCheckeds = new List<V_HIS_EXP_MEST>();
                CommonParam param = new CommonParam();
                expMestCheckeds = GetChildExpMestFromAggExpMest(AggExpMest.ID);
                if (expMestCheckeds == null || expMestCheckeds.Count == 0)
                {
                    return;
                }
                //List<long> expMestIds = expMestCheckeds.Select(o => o.ID).ToList();
                //// nếu là load lên mặc định (check all các phiếu xuất)
                //MOS.Filter.HisExpMestMedicineViewFilter filterMedicine = new HisExpMestMedicineViewFilter();
                //filterMedicine.EXP_MEST_IDs = expMestIds;

                //expMestMedicineTemps = new BackendAdapter(param).Get<List<V_HIS_EXP_MEST_MEDICINE>>(HisRequestUriStore.HIS_EXP_MEST_MEDICINE_GETVIEW, ApiConsumer.ApiConsumers.MosConsumer, filterMedicine, param);

                //MOS.Filter.HisExpMestMedicineViewFilter filterMaterial = new HisExpMestMedicineViewFilter();
                //filterMaterial.EXP_MEST_IDs = expMestIds;
                //expMestMaterialTemps = new BackendAdapter(param).Get<List<V_HIS_EXP_MEST_MATERIAL>>(HisRequestUriStore.HIS_EXP_MEST_MATERIAL_GETVIEW, ApiConsumer.ApiConsumers.MosConsumer, filterMaterial, param);

                String message = "";

                foreach (var item in expMestCheckeds)
                {
                    if (item.EXP_MEST_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__BL)
                    {
                        message += item.EXP_MEST_CODE + "; ";
                    }
                }

                if (expMestCheckeds == null || expMestCheckeds.Count() == 0)
                {
                    return;
                }

                if (!String.IsNullOrWhiteSpace(message))
                {
                    MessageBox.Show("Không cho phép chọn phiếu bù lẻ để in phiếu công khai [mã phiếu xuất: " + message + "]");
                }

                WaitingManager.Show();

                var groupPatient = expMestCheckeds.Where(o => o.EXP_MEST_TYPE_ID != IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__BL).ToList();

                var mps262 = new HIS.Desktop.Plugins.Library.PrintAggrExpMest.PrintAggrExpMestProcessor(groupPatient);
                if (mps262 != null)
                {
                    mps262.Print("Mps000262");
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridView_RowCellClick(object sender, RowCellClickEventArgs e)
        {
            try
            {
                if (e.Column.FieldName == "DISCARD_DISPLAY")
                {
                    var ExpMestData = (V_HIS_EXP_MEST_3)gridView.GetRow(e.RowHandle);
                    #region ----- DISCARD_DISPLAY -----
                    if (((_Room != null && _Room.DEPARTMENT_ID == ExpMestData.REQ_DEPARTMENT_ID)
                    && (ExpMestData.EXP_MEST_STT_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__DRAFT
                    || ExpMestData.EXP_MEST_STT_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__REJECT
                    || ExpMestData.EXP_MEST_STT_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__REQUEST)
                    && (!GlobalStore.isEnableDeleteAggregate || ((CheckLoginAdmin.IsAdmin(LoggingName) || ExpMestData.CREATOR == LoggingName) && ExpMestData.REQ_ROOM_ID == _Room.ID)))
                    )
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
                                if (ExpMestData != null)
                                {
                                    WaitingManager.Show();
                                    var apiresul = new Inventec.Common.Adapter.BackendAdapter
                                        (param).Post<bool>
                                        ("api/HisExpMest/AggrDelete", ApiConsumer.ApiConsumers.MosConsumer, ExpMestData.ID, param);
                                    if (apiresul)
                                    {
                                        success = true;
                                        FillDataToGrid();
                                    }
                                    WaitingManager.Hide();
                                    Inventec.Desktop.Common.Message.MessageManager.Show(this.ParentForm, param, success);
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Error(ex);
                            WaitingManager.Hide();
                        }
                    }
                    #endregion
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridView_RowCellStyle(object sender, RowCellStyleEventArgs e)
        {
            try
            {
                try
                {
                    if (e.RowHandle < 0)
                        return;
                    int rowHandleSelected = gridView.GetVisibleRowHandle(e.RowHandle);
                    var data = (V_HIS_EXP_MEST_3)gridView.GetRow(rowHandleSelected);
                    if (data != null && data.HAS_NOT_PRES == 1)
                    {
                        e.Appearance.Font = new Font(e.Appearance.Font, FontStyle.Italic);

                    }
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Error(ex);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void dtExpTimeTo_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (dtExpTimeTo.DateTime != null)
                {
                    string dateTime = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(dtExpTimeTo.DateTime).ToString().Substring(0, 8) + "235959";
                    dtExpTimeTo.DateTime = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(Inventec.Common.TypeConvert.Parse.ToInt64(dateTime)) ?? DateTime.MinValue;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtSearchPatientCode_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (!String.IsNullOrEmpty(txtSearchPatientCode.Text))
                    {
                        FillDataToGrid();
                        txtSearchPatientCode.SelectAll();
                    }
                    else
                    {
                        txtSearchTreatmentCode.Focus();
                        txtSearchTreatmentCode.SelectAll();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtSearchTreatmentCode_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (!String.IsNullOrEmpty(txtSearchTreatmentCode.Text))
                    {
                        FillDataToGrid();
                        txtSearchTreatmentCode.SelectAll();
                    }
                    else
                    {
                        txtKeyWord.Focus();
                        txtKeyWord.SelectAll();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }


    }
}
