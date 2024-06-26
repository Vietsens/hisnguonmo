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
using DevExpress.Data;
using System.Collections;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using DevExpress.Utils;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Grid;
using System.IO;
using Inventec.Common.RichEditor.Base;
using System.Threading;
using FlexCel.Report;
using MOS.EFMODEL.DataModels;
using HIS.Desktop.LocalStorage.BackendData;
using DevExpress.XtraEditors;
using HIS.Desktop.Utilities.Extensions;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Utility;
using MOS.Filter;
using Inventec.Common.Adapter;
using MOS.SDO;
using HIS.Desktop.ApiConsumer;
using Inventec.Desktop.Common.LanguageManager;
using System.Resources;
using HIS.Desktop.LocalStorage.ConfigSystem;
using HIS.Desktop.Plugins.HisRadixChangeCabinetList.Base;
using HIS.Desktop.IsAdmin;
using HIS.Desktop.LocalStorage.HisConfig;
using DevExpress.XtraBars;
using HIS.Desktop.ADO;
using AutoMapper;

namespace HIS.Desktop.Plugins.HisRadixChangeCabinetList
{
    public partial class UCHisRadixChangeCabinet : UserControlBase
    {
        #region Declare
        int rowCount = 0;
        int dataTotal = 0;
        int startPage = 0;
        long roomId = 0;
        long roomTypeId = 0;
        ToolTipControlInfo lastInfo;
        GridColumn lastColumn = null;
        int lastRowHandle = -1;
        MOS.EFMODEL.DataModels.V_HIS_MEDI_STOCK medistock;
        private string LoggingName = "";
        List<MOS.EFMODEL.DataModels.V_HIS_EXP_MEST_4> listExpMest;
        bool IsType = true;
        List<ACS.EFMODEL.DataModels.ACS_CONTROL> controlAcs;
        List<HIS_EXP_MEST_TYPE> expMestTypes;
        List<V_HIS_MEDI_STOCK> medistocks;
        List<V_HIS_ROOM> rooms;
        V_HIS_EXP_MEST rightClickData;
        //ACS.SDO.AcsAuthorizeSDO acsAuthorizeSDO;

        List<HIS_EXP_MEST_STT> _StatusSelecteds;

        List<HIS_IMP_MEST> listImpMest;

        ToolTip toolTip = new ToolTip();
        V_HIS_ROOM room;

        Inventec.Desktop.Common.Modules.Module currentModule;

        #endregion

        #region Construct
        public UCHisRadixChangeCabinet()
        {
            InitializeComponent();
            try
            {

                //FillDataToNavBarStatus();
                //FillDataToNavBarTypes();
                LoggingName = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public UCHisRadixChangeCabinet(Inventec.Desktop.Common.Modules.Module _module)
            : base(_module)
        {
            InitializeComponent();
            try
            {
                this.currentModule = _module;
                this.roomId = _module.RoomId;
                this.roomTypeId = _module.RoomTypeId;
                LoggingName = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void UCHisExportMestMedicine_Load(object sender, EventArgs e)
        {
            try
            {
                CommonParam param = new CommonParam();
                HisConfigCFG.LoadConfig();
                HisMediStockViewFilter medistockFilter = new HisMediStockViewFilter();
                medistockFilter.IS_ACTIVE = 1;
                medistocks = new BackendAdapter(param).Get<List<V_HIS_MEDI_STOCK>>("api/HisMediStock/GetView", ApiConsumers.MosConsumer, medistockFilter, param);

                HisRoomViewFilter roomFilter = new HisRoomViewFilter();
                roomFilter.IS_ACTIVE = 1;
                rooms = new BackendAdapter(param).Get<List<V_HIS_ROOM>>("api/HisRoom/GetView", ApiConsumers.MosConsumer, roomFilter, param);

                medistock = medistocks.FirstOrDefault(o => o.ROOM_ID == this.roomId && o.ROOM_TYPE_ID == this.roomTypeId);
                room = rooms.FirstOrDefault(o => o.ID == this.roomId);

                if (GlobalVariables.AcsAuthorizeSDO != null)
                {
                    controlAcs = GlobalVariables.AcsAuthorizeSDO.ControlInRoles;
                }
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData("UCHisExportMestMedicine_Load controlAcs ", controlAcs));

                //Gan ngon ngu
                LoadKeysFromlanguage();

                //Load Combo
                InitCheck(cboStatus, SelectionGrid__Status);

                var dataSourceSTT = BackendDataWorker.Get<HIS_EXP_MEST_STT>().Where(o =>
                    o.ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__REQUEST
                    || o.ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__DONE
                    || o.ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__EXECUTE
                    ).ToList();

                InitCombo(cboStatus, dataSourceSTT, "EXP_MEST_STT_NAME", "ID");


                //Gan gia tri mac dinh
                SetDefaultValueControl();

                //Load du lieu
                RefreshData();

                //focus truong du lieu dau tien
                txtExpMestCode.Focus();
                gridControl.ToolTipController = this.toolTipController;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        #endregion

        #region Private method

        private void GetImpMest(List<long> chmsId)
        {
            try
            {
                listImpMest = new List<HIS_IMP_MEST>();
                CommonParam param = new CommonParam();
                HisImpMestFilter filter = new HisImpMestFilter();
                filter.CHMS_EXP_MEST_IDs = chmsId;
                listImpMest = new BackendAdapter(param).Get<List<HIS_IMP_MEST>>("api/HisImpMest/Get", ApiConsumer.ApiConsumers.MosConsumer, filter, param);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        public void RefreshData()
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

        private void LoadKeysFromlanguage()
        {
            try
            {
                Resources.ResourceLanguageManager.LanguageResource = new ResourceManager("HIS.Desktop.Plugins.HisRadixChangeCabinetList.Resources.Lang", typeof(HIS.Desktop.Plugins.HisRadixChangeCabinetList.UCHisRadixChangeCabinet).Assembly);

                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.layoutControl1.Text = Inventec.Common.Resource.Get.Value("UCHisRadixChangeCabinet.layoutControl1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnListDepa.Text = Inventec.Common.Resource.Get.Value("UCHisRadixChangeCabinet.btnListDepa.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnExportCodeList.Text = Inventec.Common.Resource.Get.Value("UCHisRadixChangeCabinet.btnExportCodeList.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl2.Text = Inventec.Common.Resource.Get.Value("UCHisRadixChangeCabinet.layoutControl2.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboStatus.Properties.NullText = Inventec.Common.Resource.Get.Value("UCHisRadixChangeCabinet.cboStatus.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.chkInStock.Properties.Caption = Inventec.Common.Resource.Get.Value("UCHisRadixChangeCabinet.chkInStock.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.txtExpMestCode.Properties.NullValuePrompt = Inventec.Common.Resource.Get.Value("UCHisRadixChangeCabinet.txtExpMestCode.Properties.NullValuePrompt", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnRefresh.Text = Inventec.Common.Resource.Get.Value("UCHisRadixChangeCabinet.btnRefresh.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnSearch.Text = Inventec.Common.Resource.Get.Value("UCHisRadixChangeCabinet.btnSearch.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.txtKeyWord.Properties.NullValuePrompt = Inventec.Common.Resource.Get.Value("UCHisRadixChangeCabinet.txtKeyWord.Properties.NullValuePrompt", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciCreateTimeFrom.Text = Inventec.Common.Resource.Get.Value("UCHisRadixChangeCabinet.lciCreateTimeFrom.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciExpTimeFrom.Text = Inventec.Common.Resource.Get.Value("UCHisRadixChangeCabinet.lciExpTimeFrom.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem27.Text = Inventec.Common.Resource.Get.Value("UCHisRadixChangeCabinet.layoutControlItem27.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciCreateTimeTo.Text = Inventec.Common.Resource.Get.Value("UCHisRadixChangeCabinet.lciCreateTimeTo.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciExpTimeTo.Text = Inventec.Common.Resource.Get.Value("UCHisRadixChangeCabinet.lciExpTimeTo.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem9.Text = Inventec.Common.Resource.Get.Value("UCHisRadixChangeCabinet.layoutControlItem9.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.STT.Caption = Inventec.Common.Resource.Get.Value("UCHisRadixChangeCabinet.STT.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn1.Caption = Inventec.Common.Resource.Get.Value("UCHisRadixChangeCabinet.gridColumn1.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn15.Caption = Inventec.Common.Resource.Get.Value("UCHisRadixChangeCabinet.gridColumn15.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn6.Caption = Inventec.Common.Resource.Get.Value("UCHisRadixChangeCabinet.gridColumn6.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn3.Caption = Inventec.Common.Resource.Get.Value("UCHisRadixChangeCabinet.gridColumn3.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn20.Caption = Inventec.Common.Resource.Get.Value("UCHisRadixChangeCabinet.gridColumn20.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn4.Caption = Inventec.Common.Resource.Get.Value("UCHisRadixChangeCabinet.gridColumn4.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn7.Caption = Inventec.Common.Resource.Get.Value("UCHisRadixChangeCabinet.gridColumn7.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn8.Caption = Inventec.Common.Resource.Get.Value("UCHisRadixChangeCabinet.gridColumn8.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn8.ToolTip = Inventec.Common.Resource.Get.Value("UCHisRadixChangeCabinet.gridColumn8.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn2.Caption = Inventec.Common.Resource.Get.Value("UCHisRadixChangeCabinet.gridColumn2.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn9.Caption = Inventec.Common.Resource.Get.Value("UCHisRadixChangeCabinet.gridColumn9.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn5.Caption = Inventec.Common.Resource.Get.Value("UCHisRadixChangeCabinet.gridColumn5.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn16.Caption = Inventec.Common.Resource.Get.Value("UCHisRadixChangeCabinet.gridColumn16.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.IMP_MEST_STT_NAME.Caption = Inventec.Common.Resource.Get.Value("UCHisRadixChangeCabinet.IMP_MEST_STT_NAME.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn10.Caption = Inventec.Common.Resource.Get.Value("UCHisRadixChangeCabinet.gridColumn10.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn10.ToolTip = Inventec.Common.Resource.Get.Value("UCHisRadixChangeCabinet.gridColumn10.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.GcExpMestCode.Caption = Inventec.Common.Resource.Get.Value("UCHisRadixChangeCabinet.GcExpMestCode.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.GcExpMestTypeName.Caption = Inventec.Common.Resource.Get.Value("UCHisRadixChangeCabinet.GcExpMestTypeName.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.GcReqName.Caption = Inventec.Common.Resource.Get.Value("UCHisRadixChangeCabinet.GcReqName.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.GcMediStockName.Caption = Inventec.Common.Resource.Get.Value("UCHisRadixChangeCabinet.GcMediStockName.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.GcApprovalName.Caption = Inventec.Common.Resource.Get.Value("UCHisRadixChangeCabinet.GcApprovalName.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.GcApprovalTime.Caption = Inventec.Common.Resource.Get.Value("UCHisRadixChangeCabinet.GcApprovalTime.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.GcExpLoginName.Caption = Inventec.Common.Resource.Get.Value("UCHisRadixChangeCabinet.GcExpLoginName.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.GcFinishTime.Caption = Inventec.Common.Resource.Get.Value("UCHisRadixChangeCabinet.GcFinishTime.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.GcReqDepartmentCode.Caption = Inventec.Common.Resource.Get.Value("UCHisRadixChangeCabinet.GcReqDepartmentCode.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.GcReqDepartmentName.Caption = Inventec.Common.Resource.Get.Value("UCHisRadixChangeCabinet.GcReqDepartmentName.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.GcCreateTime.Caption = Inventec.Common.Resource.Get.Value("UCHisRadixChangeCabinet.GcCreateTime.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.GcCreator.Caption = Inventec.Common.Resource.Get.Value("UCHisRadixChangeCabinet.GcCreator.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.GcModifyTime.Caption = Inventec.Common.Resource.Get.Value("UCHisRadixChangeCabinet.GcModifyTime.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.GcModifier.Caption = Inventec.Common.Resource.Get.Value("UCHisRadixChangeCabinet.GcModifier.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.ButtonAssignTestDisable.Buttons[0].ToolTip = this.ButtonAssignTest.Buttons[0].ToolTip;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
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
                cbo.Properties.View.OptionsSelection.ShowCheckBoxSelectorInColumnHeader = DefaultBoolean.True;

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

        private void ResetComboPatientType(GridLookUpEdit cbo)
        {
            try
            {
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

        private void SetDefaultValueControl()
        {
            try
            {
                cboStatus.Enabled = false;
                cboStatus.Enabled = true;

                txtExpMestCode.Text = "";
                txtKeyWord.Text = "";
                dtCreateTimeFrom.EditValue = DateTime.Now;
                dtCreateTimeTo.EditValue = DateTime.Now;
                dtExpTimeFrom.EditValue = null;
                dtExpTimeTo.EditValue = null;
                chkBCS.CheckState = CheckState.Checked;
                chkHCS.CheckState = CheckState.Checked;
                //SetDefaultValueStatus();
                //SetDefaultValueType();
                txtExpMestCode.Focus();
                txtExpMestCode.SelectAll();

                if (this.medistock != null)
                {
                    chkInStock.Enabled = true;
                    chkInStock.Checked = true;
                }
                else
                {
                    chkInStock.Enabled = false;
                    chkInStock.Checked = false;
                }

                long showbtn = Inventec.Common.TypeConvert.Parse.ToInt64(HisConfigs.Get<string>(Base.GlobalStore.showButton));

                if (showbtn == 1)
                {
                    lciBtnExport.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                    lciBtnListDepa.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                }
                else
                {
                    lciBtnExport.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                    lciBtnListDepa.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void FillDataToControl()
        {
            try
            {
                WaitingManager.Show();
                int pagingSize = 0;
                if (ucPaging.pagingGrid != null)
                {
                    pagingSize = ucPaging.pagingGrid.PageSize;
                }
                else
                {
                    pagingSize = ConfigApplicationWorker.Get<int>("CONFIG_KEY__NUM_PAGESIZE");
                }

                GridPaging(new CommonParam(0, pagingSize));
                CommonParam param = new CommonParam();
                param.Limit = rowCount;
                param.Count = dataTotal;
                ucPaging.Init(GridPaging, param, pagingSize, this.gridControl);
                gridControl.RefreshDataSource();
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
                ApiResultObject<List<MOS.EFMODEL.DataModels.V_HIS_EXP_MEST_4>> apiResult = null;
                MOS.Filter.HisExpMestView4Filter filter = new MOS.Filter.HisExpMestView4Filter();
                SetFilter(ref filter);
                gridView.BeginUpdate();
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData("filter:", filter));
                apiResult = new Inventec.Common.Adapter.BackendAdapter
                    (paramCommon).GetRO<List<MOS.EFMODEL.DataModels.V_HIS_EXP_MEST_4>>
                    ("api/HisExpMest/GetView4", ApiConsumer.ApiConsumers.MosConsumer, filter, paramCommon);
                if (apiResult != null)
                {
                    listExpMest = apiResult.Data;
                    if (listExpMest != null && listExpMest.Count > 0)
                    {
                        GetImpMest(listExpMest.Select(o => o.ID).ToList());
                        gridControl.DataSource = listExpMest;
                        rowCount = (listExpMest == null ? 0 : listExpMest.Count);
                        dataTotal = (apiResult.Param == null ? 0 : apiResult.Param.Count ?? 0);
                    }
                    else
                    {
                        gridControl.DataSource = null;
                        rowCount = (listExpMest == null ? 0 : listExpMest.Count);
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
            }
        }

        private void SetFilter(ref MOS.Filter.HisExpMestView4Filter filter)
        {
            try
            {
                filter.ORDER_FIELD = "MODIFY_TIME";
                filter.ORDER_DIRECTION = "DESC";
                filter.EXP_MEST_TYPE_ID = IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__CK;
                if (!String.IsNullOrEmpty(txtExpMestCode.Text))
                {
                    string code = txtExpMestCode.Text.Trim();
                    if (code.Length < 12 && checkDigit(code))
                    {
                        code = string.Format("{0:000000000000}", Convert.ToInt64(code));
                        txtExpMestCode.Text = code;
                    }
                    filter.EXP_MEST_CODE__EXACT = code;
                    //filter.DATA_DOMAIN_FILTER = true;
                    filter.WORKING_ROOM_ID = roomId;
                }
                else
                {
                    //filter.HAS_AGGR_EXP_MEST_ID = false;
                    filter.KEY_WORD = txtKeyWord.Text.Trim();

                    if (chkInStock.Checked && this.medistock != null)
                    {
                        filter.MEDI_STOCK_ID__OR__IMP_MEDI_STOCK_ID = this.medistock.ID;
                    }
                    else if (!chkInStock.Checked && this.medistock != null)
                    {
                        filter.DATA_DOMAIN_FILTER = true;
                        filter.WORKING_ROOM_ID = roomId;
                    }
                    else if (this.medistock == null && this.room != null)
                    {
                        filter.REQ_DEPARTMENT_ID = this.room.DEPARTMENT_ID;
                    }

                    if (dtCreateTimeFrom.EditValue != null && dtCreateTimeFrom.DateTime != DateTime.MinValue)
                        filter.CREATE_DATE_FROM = Inventec.Common.TypeConvert.Parse.ToInt64(
                            Convert.ToDateTime(dtCreateTimeFrom.EditValue).ToString("yyyyMMdd") + "000000");

                    if (dtCreateTimeTo.EditValue != null && dtCreateTimeTo.DateTime != DateTime.MinValue)
                        filter.CREATE_DATE_TO = Inventec.Common.TypeConvert.Parse.ToInt64(
                            Convert.ToDateTime(dtCreateTimeTo.EditValue).ToString("yyyyMMdd") + "000000");

                    if (dtExpTimeFrom.EditValue != null && dtExpTimeFrom.DateTime != DateTime.MinValue)
                        filter.FINISH_DATE_FROM = Inventec.Common.TypeConvert.Parse.ToInt64(
                            Convert.ToDateTime(dtExpTimeFrom.EditValue).ToString("yyyyMMdd") + "000000");

                    if (dtExpTimeTo.EditValue != null && dtExpTimeTo.DateTime != DateTime.MinValue)
                        filter.FINISH_DATE_TO = Inventec.Common.TypeConvert.Parse.ToInt64(
                            Convert.ToDateTime(dtExpTimeTo.EditValue).ToString("yyyyMMdd") + "000000");

                    SetFilterStatus(ref filter);

                    if (chkHCS.CheckState == CheckState.Checked && chkBCS.CheckState == CheckState.Checked)
                    {
                        filter.CHMS_TYPE_IDs = new List<long> { 1, 2 };
                    }

                    else if (chkBCS.CheckState == CheckState.Checked && chkHCS.CheckState == CheckState.Unchecked)
                    {
                        filter.CHMS_TYPE_ID = 1;
                    }
                    else if (chkHCS.CheckState == CheckState.Checked && chkBCS.CheckState == CheckState.Unchecked)
                    {
                        filter.CHMS_TYPE_ID = 2;
                    }

                }
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

        private void SetFilterStatus(ref MOS.Filter.HisExpMestView4Filter filter)
        {
            try
            {
                if (_StatusSelecteds != null && _StatusSelecteds.Count > 0)
                {
                    filter.EXP_MEST_STT_IDs = _StatusSelecteds.Select(o => o.ID).ToList();
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

        private void btnSearch_Click(object sender, EventArgs e)
        {
            try
            {
                RefreshData();
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
                ResetCombo(cboStatus);
                SetDefaultValueControl();
                RefreshData();
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
                if (e.KeyCode == Keys.Enter) btnSearch_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtExpMestCode_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (!String.IsNullOrEmpty(txtExpMestCode.Text))
                    {
                        RefreshData();
                        txtExpMestCode.SelectAll();
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

        private void gridView_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)
                {
                    MOS.EFMODEL.DataModels.V_HIS_EXP_MEST_4 data = (MOS.EFMODEL.DataModels.V_HIS_EXP_MEST_4)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
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
                            else if (data.EXP_MEST_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__PL) //Tong hop
                            {
                                e.Value = imageListStatus.Images[5];
                            }
                        }
                        else if (e.Column.FieldName == "CREATE_TIME_DISPLAY")
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(data.CREATE_TIME ?? 0);
                        }
                        else if (e.Column.FieldName == "FINISH_TIME_DISPLAY")
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(data.FINISH_TIME ?? 0);
                        }
                        else if (e.Column.FieldName == "MODIFY_TIME_DISPLAY")
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(data.MODIFY_TIME ?? 0);
                        }
                        else if (e.Column.FieldName == "DOB_STR")
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToDateString(data.TDL_PATIENT_DOB ?? 0);
                        }
                        // else if (e.Column.FieldName == "APPROVAL_TIME_DISPLAY")
                        //{
                        //e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(data.APPROVAL_TIME ?? 0);
                        // }
                        //else if (e.Column.FieldName == "APPROVAL_LOGINNAME_DISPLAY")
                        // {
                        //string APPROVAL_LOGINNAME = data.APPROVAL_LOGINNAME;
                        //string APPROVAL_USERNAME = data.APPROVAL_USERNAME;
                        //e.Value = DisplayName(APPROVAL_LOGINNAME, APPROVAL_USERNAME);
                        //}
                        //else if (e.Column.FieldName == "EXP_LOGINNAME_DISPLAY")
                        //{
                        //string IMP_LOGINNAME = data.EXP_LOGINNAME;
                        //string IMP_USERNAME = data.EXP_USERNAME;
                        //e.Value = DisplayName(IMP_LOGINNAME, IMP_USERNAME);
                        // }
                        else if (e.Column.FieldName == "REQ_LOGINNAME_DISPLAY")
                        {
                            string Req_loginName = data.REQ_LOGINNAME;
                            string Req_UserName = data.REQ_USERNAME;
                            e.Value = DisplayName(Req_loginName, Req_UserName);
                        }
                        // else if (e.Column.FieldName == "EXP_MEST_TYPE_NAME_DISPLAY")
                        //{
                        //Review
                        ////if (data.EXP_MEST_TYPE_ID == Base.HisExpMestTypeCFG.EXP_MEST_TYPE_ID__PRES)
                        ////{
                        //if (data.BLOOD_TYPE_COUNT > 0)
                        //{
                        //    e.Value = data.EXP_MEST_TYPE_NAME + Inventec.Common.Resource.Get.Value(
                        //        "IVT_LANGUAGE_KEY__UC_HIS_EXP_MEST_LIST__GRID_CELL_EXP_MEST_TYPE",
                        //        Resources.ResourceLanguageManager.LanguageUCHisExportMestMedicine,
                        //        Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                        //}
                        //else
                        //    e.Value = data.EXP_MEST_TYPE_NAME;
                        ////}
                        ////else
                        ////    e.Value = data.EXP_MEST_TYPE_NAME;
                        //}
                    }
                }
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
                    V_HIS_EXP_MEST_4 pData = (V_HIS_EXP_MEST_4)gridView.GetRow(e.RowHandle);
                    long statusIdCheckForButtonEdit = Inventec.Common.TypeConvert.Parse.ToInt64((gridView.GetRowCellValue(e.RowHandle, "EXP_MEST_STT_ID") ?? "").ToString());
                    long mediStockId = Inventec.Common.TypeConvert.Parse.ToInt64((gridView.GetRowCellValue(e.RowHandle, "MEDI_STOCK_ID") ?? "").ToString());
                    long chmsTypeId = Inventec.Common.TypeConvert.Parse.ToInt64((gridView.GetRowCellValue(e.RowHandle, "CHMS_TYPE_ID") ?? "").ToString());
                    long impMediStockId = Inventec.Common.TypeConvert.Parse.ToInt64((gridView.GetRowCellValue(e.RowHandle, "IMP_MEDI_STOCK_ID") ?? "").ToString());
                    long expMestTypeId = Inventec.Common.TypeConvert.Parse.ToInt64((gridView.GetRowCellValue(e.RowHandle, "EXP_MEST_TYPE_ID") ?? "").ToString());
                    string creator = (gridView.GetRowCellValue(e.RowHandle, "CREATOR") ?? "").ToString().Trim();
                    long currentDepartment = WorkPlace.GetDepartmentId();
                    if (e.Column.FieldName == "EDIT_DISPLAY") // sửa
                    {

                        if ((creator == LoggingName || mediStockId == medistock.ID) &&
                            (statusIdCheckForButtonEdit == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__DRAFT ||
                            statusIdCheckForButtonEdit == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__REQUEST))
                            e.RepositoryItem = ButtonEnableEdit;
                        else
                            e.RepositoryItem = ButtonDisableEdit;

                    }
                    else if (e.Column.FieldName == "DISCARD_DISPLAY") //xóa
                    {

                        if ((creator == LoggingName || mediStockId == medistock.ID) && (statusIdCheckForButtonEdit == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__REQUEST
                            || statusIdCheckForButtonEdit == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__DRAFT
                            || statusIdCheckForButtonEdit == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__REJECT))
                        {
                            e.RepositoryItem = ButtonEnableDiscard;
                        }
                        else
                            e.RepositoryItem = ButtonDisableDiscard;
                    }
                    else if (e.Column.FieldName == "APPROVAL_DISPLAY") //duyet
                    {
                        if (statusIdCheckForButtonEdit == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__REQUEST
                            && controlAcs != null
                            && controlAcs.FirstOrDefault(o => o.CONTROL_CODE == ControlCode.BtnApprove) != null
                            && medistock != null
                            && medistock.IS_CABINET != 1)
                        {
                            e.RepositoryItem = ButtonEnableApproval;
                        }
                        else if (statusIdCheckForButtonEdit == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__EXECUTE
                            && medistock != null
                            && medistock.IS_CABINET != 1)
                        {
                            e.RepositoryItem = ButtonRequest;
                        }
                        else
                            e.RepositoryItem = ButtonDisableApproval;
                    }
                    else if (e.Column.FieldName == "DIS_APPROVAL")// Từ chối duyệt duyệt
                    {

                        if (medistock != null && medistock.ID == mediStockId &&
                            (statusIdCheckForButtonEdit == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__REQUEST || statusIdCheckForButtonEdit == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__REJECT))
                        {
                            if (statusIdCheckForButtonEdit == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__REJECT)
                            {
                                e.RepositoryItem = Btn_HuyTuChoiDuyet_Enable;
                            }
                            else if (statusIdCheckForButtonEdit == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__REQUEST)
                            {
                                e.RepositoryItem = ButtonEnableDisApproval;
                            }
                            else
                            {
                                e.RepositoryItem = ButtonDisableDisApproval;
                            }
                        }
                        else
                        {
                            e.RepositoryItem = ButtonDisableDisApproval;
                        }

                    }
                    else if (e.Column.FieldName == "REQUEST_DISPLAY")// Hủy duyệt
                    {
                        if (medistock != null && medistock.IS_CABINET != 1 && statusIdCheckForButtonEdit == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__DONE)
                        {
                            e.RepositoryItem = ButtonRequest;
                        }
                        else
                        {
                            e.RepositoryItem = ButtonRequestDisable;
                        }
                    }
                    else if (e.Column.FieldName == "ExportEqualApprove")// Trạng thái duyệt
                    {
                        if (expMestTypeId != IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__BCT)
                        {
                            if (statusIdCheckForButtonEdit == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__EXECUTE)
                            {
                                if (pData.IS_EXPORT_EQUAL_APPROVE == 1)
                                {
                                    e.RepositoryItem = NotApprove;
                                }
                                else
                                    e.RepositoryItem = Approve;
                            }
                        }
                        else
                        {
                            e.RepositoryItem = NotApprove;
                        }
                    }
                    else if (e.Column.FieldName == "EXPORT_DISPLAY")
                    {
                        if (statusIdCheckForButtonEdit == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__EXECUTE
                            && controlAcs != null
                            && controlAcs.FirstOrDefault(o => o.CONTROL_CODE == ControlCode.BtnExport) != null
                            && medistock != null
                            && medistock.IS_CABINET != 1)
                        {
                            e.RepositoryItem = ButtonEnableActualExport;
                        }
                        else if (statusIdCheckForButtonEdit == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__DONE
                            && controlAcs != null
                            && controlAcs.FirstOrDefault(o => o.CONTROL_CODE == ControlCode.BtnCancelExport) != null
                            && medistock != null
                            && medistock.IS_CABINET != 1)
                        {
                            e.RepositoryItem = Btn_HuyThucXuat_Enable;
                        }
                        else
                            e.RepositoryItem = ButtonDisableActualExport;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SetToolTipButton(DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit btn, long expMestTypeId)
        {
            try
            {
                //if (expMestTypeId == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__HCS)
                //    btn.Buttons[0].ToolTip = "Tạo phiếu nhập hoàn cơ số";
                if (expMestTypeId == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__BCS)
                    btn.Buttons[0].ToolTip = "Tạo phiếu nhập bù cơ số";
                else if (expMestTypeId == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__CK)
                    btn.Buttons[0].ToolTip = "Tạo phiếu nhập chuyển kho";
                else if (expMestTypeId == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__BL)
                    btn.Buttons[0].ToolTip = "Tạo phiếu nhập bù lẻ";
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
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
                            if (info.Column.FieldName == "EXP_MEST_STT_ICON")
                            {
                                text = (view.GetRowCellValue(lastRowHandle, "EXP_MEST_STT_NAME") ?? "").ToString();
                            }
                            else
                            {
                                var isNotTaken = (short?)view.GetRowCellValue(lastRowHandle, "IS_NOT_TAKEN");
                                if (isNotTaken == 1)
                                {
                                    text = "Bệnh nhân không lấy";
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

        private void SelectionGrid__Status(object sender, EventArgs e)
        {
            try
            {
                _StatusSelecteds = new List<HIS_EXP_MEST_STT>();
                foreach (HIS_EXP_MEST_STT rv in (sender as GridCheckMarksSelection).Selection)
                {
                    if (rv != null)
                        _StatusSelecteds.Add(rv);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        #endregion

        #region Public method
        public void Search()
        {
            try
            {
                if (btnSearch.Enabled)
                {
                    btnSearch.Focus();
                    btnSearch_Click(null, null);
                }
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
                if (btnRefresh.Enabled)
                {
                    btnRefresh.Focus();
                    btnRefresh_Click(null, null);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public void FocusExpCode()
        {
            try
            {
                txtExpMestCode.Focus();
                txtExpMestCode.SelectAll();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public void Export()
        {
            try
            {
                if (btnExportCodeList.Enabled)
                {
                    btnExportCodeList.Focus();
                    btnExportCodeList_Click(null, null);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        #endregion

        #region repot
        private void btnExportCodeList_Click(object sender, EventArgs e)
        {
            try
            {
                if (!btnExportCodeList.Enabled) return;

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

                string templateFile = System.IO.Path.Combine(Application.StartupPath + "\\Tmp\\Exp", "DanhSachCacMaPhieuXuat.xlsx");

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
                expFilter.MEDI_STOCK_ID = medistock.ID;
                expFilter.EXP_MEST_TYPE_IDs = new List<long>();
                expFilter.EXP_MEST_TYPE_IDs.Add(IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__CK);
                expFilter.EXP_MEST_TYPE_IDs.Add(IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__DDT);
                expFilter.EXP_MEST_TYPE_IDs.Add(IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__DPK);
                expFilter.EXP_MEST_TYPE_IDs.Add(IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__TNCC);
                expFilter.EXP_MEST_TYPE_IDs.Add(IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__KHAC);
                expFilter.EXP_MEST_TYPE_IDs.Add(IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__BAN);
                expFilter.EXP_MEST_TYPE_IDs.Add(IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__BCS);
                expFilter.EXP_MEST_TYPE_IDs.Add(IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__HPKP);
                //Review
                //expFilter.EXP_MEST_TYPE_IDs.Add(Base.HisExpMestTypeCFG.EXP_MEST_TYPE_ID__CHMS);
                //expFilter.EXP_MEST_TYPE_IDs.Add(Base.HisExpMestTypeCFG.EXP_MEST_TYPE_ID__DEPA);
                //expFilter.EXP_MEST_TYPE_IDs.Add(Base.HisExpMestTypeCFG.EXP_MEST_TYPE_ID__EXPE);
                //expFilter.EXP_MEST_TYPE_IDs.Add(Base.HisExpMestTypeCFG.EXP_MEST_TYPE_ID__LIQU);
                //expFilter.EXP_MEST_TYPE_IDs.Add(Base.HisExpMestTypeCFG.EXP_MEST_TYPE_ID__LOST);
                //expFilter.EXP_MEST_TYPE_IDs.Add(Base.HisExpMestTypeCFG.EXP_MEST_TYPE_ID__MANU);
                //expFilter.EXP_MEST_TYPE_IDs.Add(Base.HisExpMestTypeCFG.EXP_MEST_TYPE_ID__OTHER);
                //expFilter.EXP_MEST_TYPE_IDs.Add(Base.HisExpMestTypeCFG.EXP_MEST_TYPE_ID__PRES);
                //expFilter.EXP_MEST_TYPE_IDs.Add(Base.HisExpMestTypeCFG.EXP_MEST_TYPE_ID__SALE);

                if (dtExpTimeFrom.EditValue != null && dtExpTimeFrom.DateTime != DateTime.MinValue)
                    expFilter.FINISH_DATE_FROM = Inventec.Common.TypeConvert.Parse.ToInt64(
                        dtExpTimeFrom.DateTime.ToString("yyyyMMdd") + "000000");

                if (dtExpTimeTo.EditValue != null && dtExpTimeTo.DateTime != DateTime.MinValue)
                    expFilter.FINISH_DATE_TO = Inventec.Common.TypeConvert.Parse.ToInt64(
                        dtExpTimeTo.DateTime.ToString("yyyyMMdd") + "000000");

                var exportList = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<MOS.EFMODEL.DataModels.V_HIS_EXP_MEST_4>>(ApiConsumer.HisRequestUriStore.HIS_EXP_MEST_GETVIEW, ApiConsumer.ApiConsumers.MosConsumer, expFilter, param);
                if (exportList != null && exportList.Count > 0)
                {
                    //exportList = exportList.Where(w => w.EXP_MEST_TYPE_ID != Base.HisExpMestTypeCFG.EXP_MEST_TYPE_ID__AGGR).ToList();
                    expCode = exportList.Select(s => s.EXP_MEST_CODE).OrderBy(o => o).ToList();
                }
            }
            catch (Exception ex)
            {
                expCode = null;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnListDepa_Click(object sender, EventArgs e)
        {
            try
            {
                if (!btnListDepa.Enabled) return;
                if (lciBtnListDepa.Visibility == DevExpress.XtraLayout.Utils.LayoutVisibility.Never) return;

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

                Thread thread = new System.Threading.Thread(new System.Threading.ThreadStart(DepaReport));
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

        private void DepaReport()
        {
            try
            {
                Inventec.Common.FlexCellExport.Store store = new Inventec.Common.FlexCellExport.Store(true);
                List<MOS.EFMODEL.DataModels.V_HIS_EXP_MEST_4> listDepa = new List<MOS.EFMODEL.DataModels.V_HIS_EXP_MEST_4>();
                string templateFile = System.IO.Path.Combine(Application.StartupPath + "\\Tmp\\Exp", "DanhSachCacMaPhieuXuatSuDung.xlsx");

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
                    //Review
                    GetDataDepaProcessor(ref listDepa);

                    ProcessDataDepa(listDepa, ref store);
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
                WaitingManager.Hide();
            }
        }

        //Reiew
        private void ProcessDataDepa(List<MOS.EFMODEL.DataModels.V_HIS_EXP_MEST_4> listDepa, ref Inventec.Common.FlexCellExport.Store store)
        {
            try
            {
                Inventec.Common.FlexCellExport.ProcessSingleTag singleTag = new Inventec.Common.FlexCellExport.ProcessSingleTag();
                Inventec.Common.FlexCellExport.ProcessObjectTag objectTag = new Inventec.Common.FlexCellExport.ProcessObjectTag();
                List<Base.ExportListCodeRDO> listRdo = new List<Base.ExportListCodeRDO>();
                List<Base.ExportListCodeRDO> listDepartment = new List<Base.ExportListCodeRDO>();
                Dictionary<string, object> singleValueDictionary = new Dictionary<string, object>();

                if (listDepa != null && listDepa.Count > 0)
                {
                    listDepa = listDepa.OrderBy(o => o.EXP_MEST_CODE).ToList();
                    var groups = listDepa.GroupBy(g => new { g.REQ_DEPARTMENT_ID }).ToList();
                    foreach (var depa in groups)
                    {
                        Base.ExportListCodeRDO department = new Base.ExportListCodeRDO();
                        listDepartment.Add(department);
                        var roomGroups = depa.ToList().GroupBy(g => new { g.REQ_ROOM_ID }).ToList();
                        foreach (var rooms in roomGroups)
                        {

                            var expCode = rooms.ToList();
                            Dictionary<int, List<MOS.EFMODEL.DataModels.V_HIS_EXP_MEST_4>> dicExpCode = new Dictionary<int, List<MOS.EFMODEL.DataModels.V_HIS_EXP_MEST_4>>();

                            int count = expCode.Count;
                            int max = count / 6;
                            int size = count % 6;
                            MOS.EFMODEL.DataModels.V_HIS_EXP_MEST_4 emty = new MOS.EFMODEL.DataModels.V_HIS_EXP_MEST_4();

                            if (count > 31)
                            {
                                for (int i = 0; i < 6; i++)
                                {
                                    if (i != 5)
                                    {
                                        dicExpCode[i] = new List<MOS.EFMODEL.DataModels.V_HIS_EXP_MEST_4>();
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
                                        dicExpCode[i] = new List<MOS.EFMODEL.DataModels.V_HIS_EXP_MEST_4>();
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
                                a.EXPORT_CODE1 = dicExpCode[0][i].EXP_MEST_CODE;
                                a.EXPORT_CODE2 = dicExpCode[1][i].EXP_MEST_CODE;
                                a.EXPORT_CODE3 = dicExpCode[2][i].EXP_MEST_CODE;
                                a.EXPORT_CODE4 = dicExpCode[3][i].EXP_MEST_CODE;
                                a.EXPORT_CODE5 = dicExpCode[4][i].EXP_MEST_CODE;
                                a.EXPORT_CODE6 = dicExpCode[5][i].EXP_MEST_CODE;
                                a.ROOM_ID = rooms.First().REQ_ROOM_ID;
                                listRdo.Add(a);
                            }
                        }
                    }
                }

                //sắp xếp để merge dòng
                listDepartment = listDepartment.OrderBy(o => o.DEPARTMENT_NAME).ToList();
                listRdo = listRdo.OrderBy(o => o.DEPARTMENT_NAME).ThenBy(t => t.ROOM_ID).ToList();

                singleTag.AddSingleKey(store, "TYPE", "THỰC XUẤT");
                singleTag.AddSingleKey(store, "MEDI_STOCK_NAME", medistock.MEDI_STOCK_NAME.ToUpper());
                singleTag.AddSingleKey(store, "EXP_TIME_FROM", dtExpTimeFrom.DateTime.ToString("dd/MM/yyyy"));
                singleTag.AddSingleKey(store, "EXP_TIME_TO", dtExpTimeTo.DateTime.ToString("dd/MM/yyyy"));
                HIS.Desktop.Print.SetCommonKey.SetCommonSingleKey(singleValueDictionary);
                singleTag.ProcessData(store, singleValueDictionary);
                store.SetCommonFunctions();
                objectTag.AddObjectData(store, "List", listRdo);
                objectTag.AddObjectData(store, "department", listDepartment);
                objectTag.AddRelationship(store, "department", "List", "DEPARTMENT_NAME", "DEPARTMENT_NAME");
                objectTag.SetUserFunction(store, "FuncSameTitleColRemedy", new CustomerFuncMergeSameData(listRdo));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void GetDataDepaProcessor(ref List<MOS.EFMODEL.DataModels.V_HIS_EXP_MEST_4> listDepa)
        {
            try
            {
                CommonParam param = new CommonParam();
                MOS.Filter.HisExpMestViewFilter expFilter = new MOS.Filter.HisExpMestViewFilter();
                expFilter.EXP_MEST_STT_ID = IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__DONE;
                expFilter.MEDI_STOCK_ID = medistock.ID;
                expFilter.EXP_MEST_TYPE_IDs = new List<long>();
                expFilter.EXP_MEST_TYPE_IDs.Add(IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__HPKP);

                //expFilter.MEDI_STOCK_ID = medistock.ID;

                if (dtExpTimeFrom.EditValue != null && dtExpTimeFrom.DateTime != DateTime.MinValue)
                    expFilter.FINISH_DATE_FROM = Inventec.Common.TypeConvert.Parse.ToInt64(
                        Convert.ToDateTime(dtExpTimeFrom.EditValue).ToString("yyyyMMdd") + "000000");

                if (dtExpTimeTo.EditValue != null && dtExpTimeTo.DateTime != DateTime.MinValue)
                    expFilter.FINISH_DATE_TO = Inventec.Common.TypeConvert.Parse.ToInt64(
                        Convert.ToDateTime(dtExpTimeTo.EditValue).ToString("yyyyMMdd") + "000000");

                var exportList = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<MOS.EFMODEL.DataModels.V_HIS_EXP_MEST_4>>(ApiConsumer.HisRequestUriStore.HIS_EXP_MEST_GETVIEW, ApiConsumer.ApiConsumers.MosConsumer, expFilter, param);
                if (exportList != null && exportList.Count > 0)
                {
                    listDepa = exportList;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        #endregion

        private void gridView_RowCellClick(object sender, RowCellClickEventArgs e)
        {
            try
            {
                var row = (V_HIS_EXP_MEST_4)gridView.GetFocusedRow();
                if (row != null)
                {
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void cboStatus_CustomDisplayText(object sender, DevExpress.XtraEditors.Controls.CustomDisplayTextEventArgs e)
        {
            try
            {
                e.DisplayText = "";
                string statusName = "";
                if (_StatusSelecteds != null && _StatusSelecteds.Count > 0)
                {
                    foreach (var item in _StatusSelecteds)
                    {
                        statusName += item.EXP_MEST_STT_NAME + ", ";
                    }
                }

                e.DisplayText = statusName;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }


        private void cboStatus_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                //cboType.Focus();
                //cboType.ShowPopup();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void chkInStock_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    dtCreateTimeFrom.Focus();
                    dtCreateTimeFrom.ShowPopup();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void dtCreateTimeFrom_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                // dtCreateTimeTo.Focus();
                //dtCreateTimeTo.ShowPopup();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void dtCreateTimeTo_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                //dtExpTimeFrom.Focus();
                //dtExpTimeFrom.ShowPopup();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void dtExpTimeFrom_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                //dtExpTimeTo.Focus();
                //dtExpTimeTo.ShowPopup();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void dtExpTimeTo_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                //cboStatus.Focus();
                //cboStatus.ShowPopup();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void Btn_MobaDepaCreate_Enable_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                var row = (V_HIS_EXP_MEST_4)gridView.GetFocusedRow();
                if (row != null)
                {
                    WaitingManager.Show();

                    List<object> listArgs = new List<object>();
                    listArgs.Add(row.ID);
                    CallModule callModule = new CallModule(CallModule.MobaDepaCreate, this.roomId, this.roomTypeId, listArgs);

                    WaitingManager.Hide();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void Btn_ImpMestCreate_Enable_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                var row = (V_HIS_EXP_MEST_4)gridView.GetFocusedRow();
                if (row != null)
                {
                    var expMest = new V_HIS_EXP_MEST();
                    Inventec.Common.Mapper.DataObjectMapper.Map<V_HIS_EXP_MEST>(expMest, row);
                    WaitingManager.Show();

                    List<object> listArgs = new List<object>();
                    listArgs.Add(expMest);
                    listArgs.Add(expMest.EXP_MEST_TYPE_ID);
                    listArgs.Add((HIS.Desktop.Common.DelegateRefreshData)RefreshData);
                    CallModule callModule = new CallModule(CallModule.ImpMestChmsCreate, this.roomId, this.roomTypeId, listArgs);

                    WaitingManager.Hide();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void dtCreateTimeFrom_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    dtCreateTimeTo.Focus();
                    dtCreateTimeTo.ShowPopup();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void dtCreateTimeTo_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    dtExpTimeFrom.Focus();
                    dtExpTimeFrom.ShowPopup();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void dtExpTimeFrom_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    dtExpTimeTo.Focus();
                    dtExpTimeTo.ShowPopup();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void dtExpTimeTo_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    cboStatus.Focus();
                    cboStatus.ShowPopup();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboStatus_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    //cboType.Focus();
                    //cboType.ShowPopup();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboStatus_Popup(object sender, EventArgs e)
        {

        }

        private void gridViewStatus_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                PhimTatCombo(e);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void PhimTatCombo(KeyEventArgs e)
        {
            try
            {

                if (e.KeyCode == Keys.Control | e.KeyCode == Keys.F)
                {
                    Search();
                }
                else if (e.KeyCode == Keys.Control | e.KeyCode == Keys.R)
                {
                    Refresh();
                }
                else if (e.KeyCode == Keys.Control | e.KeyCode == Keys.E)
                {
                    Export();
                }
                else if (e.KeyCode == Keys.F2)
                {
                    FocusExpCode();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void Btn_HuyThucXuat_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                var rowDataExpMest = (V_HIS_EXP_MEST_4)gridView.GetFocusedRow();
                if (rowDataExpMest != null)
                {
                    WaitingManager.Show();
                    bool success = false;
                    CommonParam param = new CommonParam();
                    HisExpMestSDO hisExpMestApproveSDO = new MOS.SDO.HisExpMestSDO();

                    hisExpMestApproveSDO.ExpMestId = rowDataExpMest.ID;
                    hisExpMestApproveSDO.ReqRoomId = this.roomId;

                    var rs = new Inventec.Common.Adapter.BackendAdapter(param).Post<HIS_EXP_MEST>(
               "api/HisExpMest/BaseUnexport", ApiConsumers.MosConsumer, hisExpMestApproveSDO, param);
                    if (rs != null)
                    {
                        success = true;
                        btnSearch_Click(null, null);
                    }

                    WaitingManager.Hide();
                    #region Show message
                    Inventec.Desktop.Common.Message.MessageManager.Show(this.ParentForm, param, success);
                    #endregion

                    #region Process has exception
                    HIS.Desktop.Controls.Session.SessionManager.ProcessTokenLost(param);
                    #endregion
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void Btn_EvenLog_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                WaitingManager.Show();
                var rowDataExpMest = (V_HIS_EXP_MEST_4)gridView.GetFocusedRow();
                V_HIS_EXP_MEST expMestData = new V_HIS_EXP_MEST();
                Inventec.Common.Mapper.DataObjectMapper.Map<V_HIS_EXP_MEST>(expMestData, rowDataExpMest);
                if (expMestData != null)
                {
                    WaitingManager.Show();

                    List<object> listArgs = new List<object>();
                    Inventec.UC.EventLogControl.Data.DataInit3 dataInit3 = new Inventec.UC.EventLogControl.Data.DataInit3(ConfigSystems.URI_API_SDA, GlobalVariables.APPLICATION_CODE, ConfigApplications.NumPageSize, "EXP_MEST_CODE: " + expMestData.EXP_MEST_CODE);
                    listArgs.Add(dataInit3);
                    listArgs.Add(Inventec.Desktop.Common.Modules.Module.MODULE_TYPE_ID__FORM);
                    CallModule callModule = new CallModule(CallModule.EventLog, this.roomId, this.roomTypeId, listArgs);

                    WaitingManager.Hide();
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void Btn_HuyTuChoiDuyet_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                var rowDataExpMest = (V_HIS_EXP_MEST_4)gridView.GetFocusedRow();
                V_HIS_EXP_MEST row = new V_HIS_EXP_MEST();
                Inventec.Common.Mapper.DataObjectMapper.Map<V_HIS_EXP_MEST>(row, rowDataExpMest);
                if (row != null)
                {
                    WaitingManager.Show();
                    bool success = false;
                    CommonParam param = new CommonParam();
                    HisExpMestSDO hisExpMestApproveSDO = new MOS.SDO.HisExpMestSDO();

                    hisExpMestApproveSDO.ExpMestId = row.ID;
                    //hisExpMestApproveSDO.IsFinish = true;
                    hisExpMestApproveSDO.ReqRoomId = this.roomId;
                    if (gridControl.DataSource != null)
                    {
                        var data = (List<V_HIS_EXP_MEST_4>)gridControl.DataSource;

                        var rs = new Inventec.Common.Adapter.BackendAdapter(param).Post<HIS_EXP_MEST>(
                   "api/HisExpMest/Undecline", ApiConsumers.MosConsumer, hisExpMestApproveSDO, param);
                        if (rs != null)
                        {
                            foreach (var item in data)
                            {
                                if (item.ID == rs.ID)
                                {
                                    var ExpMestSTT = BackendDataWorker.Get<HIS_EXP_MEST_STT>().FirstOrDefault(o => o.ID == rs.EXP_MEST_STT_ID);
                                    item.EXP_MEST_STT_ID = ExpMestSTT.ID;
                                    item.EXP_MEST_STT_NAME = ExpMestSTT.EXP_MEST_STT_NAME;
                                    item.EXP_MEST_STT_CODE = ExpMestSTT.EXP_MEST_STT_CODE;
                                    item.MODIFY_TIME = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(DateTime.Now);
                                    break;
                                }
                            }
                            success = true;
                            gridView.BeginUpdate();
                            data = data.OrderByDescending(p => p.MODIFY_TIME).ToList();
                            gridControl.DataSource = data;
                            gridView.EndUpdate();
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
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void gridView_RowCellStyle(object sender, RowCellStyleEventArgs e)
        {

            try
            {
                var data = (V_HIS_EXP_MEST_4)gridView.GetRow(e.RowHandle);
                if (data != null && data.IS_NOT_TAKEN == 1)
                {
                    e.Appearance.Font = new System.Drawing.Font(e.Appearance.Font, System.Drawing.FontStyle.Strikeout);

                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void Btn_HoanThanh_Enable_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                var rowDataExpMest = (V_HIS_EXP_MEST_4)gridView.GetFocusedRow();
                V_HIS_EXP_MEST row = new V_HIS_EXP_MEST();
                Inventec.Common.Mapper.DataObjectMapper.Map<V_HIS_EXP_MEST>(row, rowDataExpMest);
                if (row != null)
                {
                    WaitingManager.Show();
                    bool success = false;
                    CommonParam param = new CommonParam();
                    HisExpMestSDO hisExpMestFinishSDO = new MOS.SDO.HisExpMestSDO();

                    hisExpMestFinishSDO.ExpMestId = row.ID;
                    //hisExpMestApproveSDO.IsFinish = true;
                    hisExpMestFinishSDO.ReqRoomId = this.roomId;
                    if (gridControl.DataSource != null)
                    {
                        var data = (List<V_HIS_EXP_MEST_4>)gridControl.DataSource;
                        var rs = new Inventec.Common.Adapter.BackendAdapter(param).Post<HIS_EXP_MEST>(
                   "api/HisExpMest/Finish", ApiConsumers.MosConsumer, hisExpMestFinishSDO, param);
                        if (rs != null)
                        {
                            if (rs != null)
                            {
                                foreach (var item in data)
                                {
                                    if (item.ID == rs.ID)
                                    {
                                        var expMestSTT = BackendDataWorker.Get<HIS_EXP_MEST_STT>().FirstOrDefault(o => o.ID == rs.EXP_MEST_STT_ID);
                                        item.EXP_MEST_STT_ID = expMestSTT.ID;
                                        item.EXP_MEST_STT_NAME = expMestSTT.EXP_MEST_STT_NAME;
                                        item.EXP_MEST_STT_CODE = expMestSTT.EXP_MEST_STT_CODE;
                                        item.MODIFY_TIME = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(DateTime.Now);
                                        break;
                                    }
                                }
                                success = true;
                                gridView.BeginUpdate();
                                data = data.OrderByDescending(p => p.MODIFY_TIME).ToList();
                                gridControl.DataSource = data;
                                gridView.EndUpdate();
                            }
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
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }


        private void Btn_Check_TruyenMau_Enable_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                var rowDataExpMest = (V_HIS_EXP_MEST_4)gridView.GetFocusedRow();
                V_HIS_EXP_MEST row = new V_HIS_EXP_MEST();
                Inventec.Common.Mapper.DataObjectMapper.Map<V_HIS_EXP_MEST>(row, rowDataExpMest);
                if (row != null)
                {
                    List<object> listArgs = new List<object>();
                    listArgs.Add(row.ID);

                    HIS.Desktop.ModuleExt.PluginInstanceBehavior.ShowModule("HIS.Desktop.Plugins.HisCheckBeforeTransfusionBlood", this.currentModule.RoomId, this.currentModule.RoomTypeId, listArgs);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridView_PopupMenuShowing(object sender, PopupMenuShowingEventArgs e)
        {
            try
            {
                //this.rightClickData = null;
                //GridHitInfo hi = e.HitInfo;
                //if (hi.InRowCell)
                //{
                //    int rowHandle = gridView.GetVisibleRowHandle(hi.RowHandle);

                //    var rowDataExpMest = (V_HIS_EXP_MEST_4)gridView.GetRow(rowHandle);
                //    V_HIS_EXP_MEST row = new V_HIS_EXP_MEST();
                //    Inventec.Common.Mapper.DataObjectMapper.Map<V_HIS_EXP_MEST>(row, rowDataExpMest);
                //    if (row != null)
                //    {
                //        this.rightClickData = row;

                //        gridView.OptionsSelection.EnableAppearanceFocusedCell = true;
                //        gridView.OptionsSelection.EnableAppearanceFocusedRow = true;

                //        BarManager barManager = new BarManager();
                //        barManager.Form = this.ParentForm;

                //        ExpMestADO expMestAdo = new ExpMestADO();
                //        expMestAdo.LoginName = LoggingName;
                //        expMestAdo.MediStock = medistock;
                //        expMestAdo.listImpMest = listImpMest;
                //        expMestAdo.controlAcs = controlAcs;

                //        PopupMenuProcessor popupMenuProcessor = new PopupMenuProcessor(expMestAdo, row, barManager, MouseRight_Click);
                //        popupMenuProcessor.InitMenu();
                //    }
                //}
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void MouseRight_Click(object sender, ItemClickEventArgs e)
        {
            try
            {
                if (e.Item is BarButtonItem && this.rightClickData != null)
                {
                    var bbtnItem = sender as BarButtonItem;
                    PopupMenuProcessor.ItemType type = (PopupMenuProcessor.ItemType)(e.Item.Tag);
                    switch (type)
                    {
                        #region -----ThaoTac

                        case PopupMenuProcessor.ItemType.Sua:
                            Sua(this.rightClickData);
                            break;
                        case PopupMenuProcessor.ItemType.Xoa:
                            Xoa(this.rightClickData);
                            break;
                        case PopupMenuProcessor.ItemType.Duyet:
                            Duyet(this.rightClickData);
                            break;
                        case PopupMenuProcessor.ItemType.HuyDuyet:
                            HuyDuyet(this.rightClickData);
                            break;
                        case PopupMenuProcessor.ItemType.TuChoiDuyet:
                            TuChoiDuyet(this.rightClickData);
                            break;
                        case PopupMenuProcessor.ItemType.HuyTuChoiDuyet:
                            HuyTuChoiDuyet(this.rightClickData);
                            break;
                        case PopupMenuProcessor.ItemType.ThucXuat:
                            ThucXuat(this.rightClickData);
                            break;
                        case PopupMenuProcessor.ItemType.HuyThucXuat:
                            HuyThucXuat(this.rightClickData);
                            break;
                        case PopupMenuProcessor.ItemType.HoanThanh:
                            HoanThanh(this.rightClickData);
                            break;

                        #endregion

                        case PopupMenuProcessor.ItemType.XemChiTiet:
                            XemChiTiet(this.rightClickData);
                            break;
                        case PopupMenuProcessor.ItemType.TaoPhieuNhapHaoPhiTraLai:
                            TaoPhieuNhapHaoPhiTraLai(this.rightClickData);
                            break;
                        case PopupMenuProcessor.ItemType.TaoPhieuNhapThuHoiMau:
                            TaoPhieuNhapThuHoiMau(this.rightClickData);
                            break;
                        case PopupMenuProcessor.ItemType.TaoPhieuNhapThuHoi:
                            TaoPhieuNhapHaoPhiTraLai(this.rightClickData);
                            break;
                        case PopupMenuProcessor.ItemType.TaoPhieuNhapBuCoSo:
                            TaoPhieuNhapBuCoSo(this.rightClickData);
                            break;
                        case PopupMenuProcessor.ItemType.TaoPhieuNhapChuyenKho:
                            TaoPhieuNhapChuyenKho(this.rightClickData);
                            break;
                        case PopupMenuProcessor.ItemType.TaoPhieuNhapBuLe:
                            TaoPhieuNhapBuLe(this.rightClickData);
                            break;
                        case PopupMenuProcessor.ItemType.ChiDinhXetNghiem:
                            ChiDinhXetNghiem(this.rightClickData);
                            break;
                        case PopupMenuProcessor.ItemType.LichSuTacDong:
                            LichSuTacDong(this.rightClickData);
                            break;
                        case PopupMenuProcessor.ItemType.ThuHoiDonPhongKham:
                            ThuHoiDonPhongKham(this.rightClickData);
                            break;
                        case PopupMenuProcessor.ItemType.KhongLay:
                            KhongLayXuatBan(this.rightClickData);
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        #region Click

        private void XemChiTiet(V_HIS_EXP_MEST row)
        {
            try
            {
                if (row != null)
                {
                    try
                    {
                        WaitingManager.Show();
                        var ExpMestData = row;

                        if (ExpMestData.EXP_MEST_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__PL)
                        {
                            HIS.Desktop.ADO.ApproveAggrExpMestSDO exeMestView = new HIS.Desktop.ADO.ApproveAggrExpMestSDO(ExpMestData.ID, ExpMestData.EXP_MEST_STT_ID);
                            List<object> listArgs = new List<object>();
                            listArgs.Add(exeMestView);
                            CallModule callModule = new CallModule(CallModule.ApproveAggrExpMest, this.roomId, this.roomTypeId, listArgs);

                            WaitingManager.Hide();
                        }
                        else if (ExpMestData.EXP_MEST_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__THPK)
                        {
                            List<object> listArgs = new List<object>();
                            listArgs.Add(ExpMestData);
                            listArgs.Add((HIS.Desktop.Common.DelegateSelectData)FillDataApterSave);
                            CallModule callModule = new CallModule(CallModule.AggrExpMestDetail, this.roomId, this.roomTypeId, listArgs);

                            WaitingManager.Hide();
                        }
                        else
                        {
                            List<object> listArgs = new List<object>();
                            listArgs.Add(ExpMestData);
                            listArgs.Add((HIS.Desktop.Common.DelegateSelectData)FillDataApterSave);
                            CallModule callModule = new CallModule(CallModule.ExpMestViewDetail, this.roomId, this.roomTypeId, listArgs);

                            WaitingManager.Hide();
                        }
                    }
                    catch (Exception ex)
                    {
                        Inventec.Common.Logging.LogSystem.Error(ex);
                        WaitingManager.Hide();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void Xoa(V_HIS_EXP_MEST row)
        {
            try
            {
                if (row != null)
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
                            if (row != null)
                            {
                                WaitingManager.Show();

                                if (row.EXP_MEST_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__THPK)
                                {
                                    var apiresul = new Inventec.Common.Adapter.BackendAdapter
                                        (param).Post<bool>
                                        ("api/HisExpMest/AggrExamDelete", ApiConsumer.ApiConsumers.MosConsumer, row.ID, param);
                                    if (apiresul)
                                    {
                                        success = true;
                                        RefreshData();
                                    }
                                }
                                else
                                {
                                    HisExpMestSDO sdo = new HisExpMestSDO();
                                    sdo.ExpMestId = row.ID;
                                    sdo.ReqRoomId = this.roomId;
                                    var apiresul = new Inventec.Common.Adapter.BackendAdapter
                                        (param).Post<bool>
                                        (ApiConsumer.HisRequestUriStore.HIS_EXP_MEST_DELETE, ApiConsumer.ApiConsumers.MosConsumer, sdo, param);
                                    if (apiresul)
                                    {
                                        success = true;
                                        RefreshData();
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
                        }
                    }
                    catch (Exception ex)
                    {
                        Inventec.Common.Logging.LogSystem.Error(ex);
                        WaitingManager.Hide();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void Duyet(V_HIS_EXP_MEST row)
        {
            try
            {
                if (row != null)
                {
                    try
                    {
                        if (row != null)
                        {
                            if (row.EXP_MEST_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__CK
                                     || row.EXP_MEST_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__HPKP
                                     || row.EXP_MEST_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__DM)
                            {
                                List<object> listArgs = new List<object>();
                                listArgs.Add(row.ID);
                                listArgs.Add((HIS.Desktop.Common.DelegateSelectData)FillDataApterSave);
                                CallModule callModule = new CallModule(CallModule.BrowseExportTicket, this.roomId, this.roomTypeId, listArgs);

                                WaitingManager.Hide();
                                FillDataApterSave(true);
                            }
                            //else if (row.EXP_MEST_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__BCS)
                            //{
                            //    List<object> listArgs = new List<object>();
                            //    listArgs.Add(row.ID);
                            //    listArgs.Add((HIS.Desktop.Common.DelegateSelectData)FillDataApterSave);
                            //    CallModule callModule = new CallModule(CallModule.ApprovalExpMestBcs, this.roomId, this.roomTypeId, listArgs);

                            //    WaitingManager.Hide();
                            //    FillDataApterSave(true);
                            //}
                            else if (row.EXP_MEST_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__DDT)
                            {
                                WaitingManager.Show();
                                bool success = false;
                                CommonParam param = new CommonParam();
                                HisExpMestApproveSDO hisExpMestApproveSDO = new MOS.SDO.HisExpMestApproveSDO();

                                hisExpMestApproveSDO.ExpMestId = row.ID;
                                hisExpMestApproveSDO.IsFinish = true;
                                hisExpMestApproveSDO.ReqRoomId = this.roomId;
                                if (gridControl.DataSource != null)
                                {
                                    var griddata = (List<V_HIS_EXP_MEST_4>)gridControl.DataSource;
                                    var rs = new Inventec.Common.Adapter.BackendAdapter(param).Post<HisExpMestResultSDO>(
                               "api/HisExpMest/InPresApprove", ApiConsumers.MosConsumer, hisExpMestApproveSDO, param);
                                    if (rs != null)
                                    {
                                        foreach (var item in griddata)
                                        {
                                            if (item.ID == rs.ExpMest.ID)
                                            {
                                                var expMestSTT = BackendDataWorker.Get<HIS_EXP_MEST_STT>().FirstOrDefault(o => o.ID == rs.ExpMest.EXP_MEST_STT_ID);
                                                item.EXP_MEST_STT_ID = expMestSTT.ID;
                                                item.EXP_MEST_STT_NAME = expMestSTT.EXP_MEST_STT_NAME;
                                                item.EXP_MEST_STT_CODE = expMestSTT.EXP_MEST_STT_CODE;
                                                item.MODIFY_TIME = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(DateTime.Now);
                                                break;
                                            }
                                        }
                                        success = true;
                                        gridView.BeginUpdate();
                                        griddata = griddata.OrderByDescending(p => p.MODIFY_TIME).ToList();
                                        gridControl.DataSource = griddata;
                                        gridView.EndUpdate();
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
                            else if (row.EXP_MEST_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__THPK)
                            {
                                WaitingManager.Show();
                                bool success = false;
                                CommonParam param = new CommonParam();
                                HisExpMestSDO hisExpMestSDO = new MOS.SDO.HisExpMestSDO();

                                hisExpMestSDO.ExpMestId = row.ID;
                                hisExpMestSDO.ReqRoomId = this.roomId;

                                var rs = new Inventec.Common.Adapter.BackendAdapter(param).Post<List<HIS_EXP_MEST>>(
                           "api/HisExpMest/AggrExamApprove", ApiConsumers.MosConsumer, hisExpMestSDO, param);
                                if (rs != null && rs.Count > 0)
                                {
                                    success = true;
                                    RefreshData();
                                }

                                WaitingManager.Hide();
                                #region Show message
                                Inventec.Desktop.Common.Message.MessageManager.Show(this.ParentForm, param, success);
                                #endregion

                                #region Process has exception
                                HIS.Desktop.Controls.Session.SessionManager.ProcessTokenLost(param);
                                #endregion

                            }
                            else
                            {
                                WaitingManager.Show();
                                bool success = false;
                                CommonParam param = new CommonParam();
                                HisExpMestApproveSDO hisExpMestApproveSDO = new MOS.SDO.HisExpMestApproveSDO();

                                hisExpMestApproveSDO.ExpMestId = row.ID;
                                hisExpMestApproveSDO.IsFinish = true;
                                hisExpMestApproveSDO.ReqRoomId = this.roomId;
                                if (gridControl.DataSource != null)
                                {
                                    var griddata = (List<V_HIS_EXP_MEST_4>)gridControl.DataSource;
                                    var rs = new Inventec.Common.Adapter.BackendAdapter(param).Post<HisExpMestResultSDO>(
                               "api/HisExpMest/Approve", ApiConsumers.MosConsumer, hisExpMestApproveSDO, param);
                                    if (rs != null)
                                    {
                                        foreach (var item in griddata)
                                        {
                                            if (item.ID == rs.ExpMest.ID)
                                            {
                                                var expMestSTT = BackendDataWorker.Get<HIS_EXP_MEST_STT>().FirstOrDefault(o => o.ID == rs.ExpMest.EXP_MEST_STT_ID);
                                                item.EXP_MEST_STT_ID = expMestSTT.ID;
                                                item.EXP_MEST_STT_NAME = expMestSTT.EXP_MEST_STT_NAME;
                                                item.EXP_MEST_STT_CODE = expMestSTT.EXP_MEST_STT_CODE;
                                                item.MODIFY_TIME = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(DateTime.Now);
                                                break;
                                            }
                                        }
                                        success = true;
                                        gridView.BeginUpdate();
                                        griddata = griddata.OrderByDescending(p => p.MODIFY_TIME).ToList();
                                        gridControl.DataSource = griddata;
                                        gridView.EndUpdate();
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
                        }
                    }
                    catch (Exception ex)
                    {
                        Inventec.Common.Logging.LogSystem.Error(ex);
                        WaitingManager.Hide();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void HuyDuyet(V_HIS_EXP_MEST row)
        {
            try
            {
                if (row != null)
                {
                    try
                    {
                        if (row != null)
                        {
                            if (row.EXP_MEST_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__THPK)
                            {
                                WaitingManager.Show();
                                bool success = false;
                                CommonParam param = new CommonParam();

                                HisExpMestSDO data = new HisExpMestSDO();
                                data.ExpMestId = row.ID;
                                data.ReqRoomId = this.roomId;
                                if (gridControl.DataSource != null)
                                {
                                    var griddata = (List<V_HIS_EXP_MEST_4>)gridControl.DataSource;
                                    var apiresul = new Inventec.Common.Adapter.BackendAdapter(param).Post<HIS_EXP_MEST>("api/HisExpMest/AggrExamUnapprove", ApiConsumer.ApiConsumers.MosConsumer, data, param);
                                    if (apiresul != null)
                                    {
                                        foreach (var item in griddata)
                                        {
                                            if (item.ID == apiresul.ID)
                                            {
                                                var expMestSTT = BackendDataWorker.Get<HIS_EXP_MEST_STT>().FirstOrDefault(o => o.ID == apiresul.EXP_MEST_STT_ID);
                                                item.EXP_MEST_STT_ID = expMestSTT.ID;
                                                item.EXP_MEST_STT_NAME = expMestSTT.EXP_MEST_STT_NAME;
                                                item.EXP_MEST_STT_CODE = expMestSTT.EXP_MEST_STT_CODE;
                                                item.MODIFY_TIME = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(DateTime.Now);
                                                break;
                                            }
                                        }
                                        success = true;
                                        gridView.BeginUpdate();
                                        griddata = griddata.OrderByDescending(p => p.MODIFY_TIME).ToList();
                                        gridControl.DataSource = griddata;
                                        gridView.EndUpdate();
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
                            else if (row.EXP_MEST_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__DDT)
                            {
                                WaitingManager.Show();
                                bool success = false;
                                CommonParam param = new CommonParam();

                                HisExpMestSDO data = new HisExpMestSDO();
                                data.ExpMestId = row.ID;
                                data.ReqRoomId = this.roomId;
                                if (gridControl.DataSource != null)
                                {
                                    var griddata = (List<V_HIS_EXP_MEST_4>)gridControl.DataSource;
                                    var apiresul = new Inventec.Common.Adapter.BackendAdapter(param).Post<HIS_EXP_MEST>("api/HisExpMest/InPresUnapprove", ApiConsumer.ApiConsumers.MosConsumer, data, param);
                                    if (apiresul != null)
                                    {
                                        foreach (var item in griddata)
                                        {
                                            if (item.ID == apiresul.ID)
                                            {
                                                var expMestSTT = BackendDataWorker.Get<HIS_EXP_MEST_STT>().FirstOrDefault(o => o.ID == apiresul.EXP_MEST_STT_ID);
                                                item.EXP_MEST_STT_ID = expMestSTT.ID;
                                                item.EXP_MEST_STT_NAME = expMestSTT.EXP_MEST_STT_NAME;
                                                item.EXP_MEST_STT_CODE = expMestSTT.EXP_MEST_STT_CODE;
                                                item.MODIFY_TIME = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(DateTime.Now);
                                                break;
                                            }
                                        }
                                        success = true;
                                        gridView.BeginUpdate();
                                        griddata = griddata.OrderByDescending(p => p.MODIFY_TIME).ToList();
                                        gridControl.DataSource = griddata;
                                        gridView.EndUpdate();
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
                            else
                            {
                                WaitingManager.Show();
                                bool success = false;
                                CommonParam param = new CommonParam();

                                HisExpMestSDO data = new HisExpMestSDO();
                                data.ExpMestId = row.ID;
                                data.ReqRoomId = this.roomId;
                                if (gridControl.DataSource != null)
                                {
                                    var griddata = (List<V_HIS_EXP_MEST_4>)gridControl.DataSource;
                                    var apiresul = new Inventec.Common.Adapter.BackendAdapter(param).Post<HIS_EXP_MEST>("api/HisExpMest/Unapprove", ApiConsumer.ApiConsumers.MosConsumer, data, param);
                                    if (apiresul != null)
                                    {
                                        foreach (var item in griddata)
                                        {
                                            if (item.ID == apiresul.ID)
                                            {
                                                var expMestSTT = BackendDataWorker.Get<HIS_EXP_MEST_STT>().FirstOrDefault(o => o.ID == apiresul.EXP_MEST_STT_ID);
                                                item.EXP_MEST_STT_ID = expMestSTT.ID;
                                                item.EXP_MEST_STT_NAME = expMestSTT.EXP_MEST_STT_NAME;
                                                item.EXP_MEST_STT_CODE = expMestSTT.EXP_MEST_STT_CODE;
                                                item.MODIFY_TIME = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(DateTime.Now);
                                                break;
                                            }
                                        }
                                        success = true;
                                        gridView.BeginUpdate();
                                        griddata = griddata.OrderByDescending(p => p.MODIFY_TIME).ToList();
                                        gridControl.DataSource = griddata;
                                        gridView.EndUpdate();
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
                        }
                    }
                    catch (Exception ex)
                    {
                        Inventec.Common.Logging.LogSystem.Error(ex);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void TuChoiDuyet(V_HIS_EXP_MEST row)
        {
            try
            {
                if (row != null)
                {
                    try
                    {
                        if (row != null)
                        {
                            WaitingManager.Show();
                            bool success = false;
                            CommonParam param = new CommonParam();
                            HisExpMestSDO ado = new HisExpMestSDO();
                            ado.ExpMestId = row.ID;
                            ado.ReqRoomId = this.roomId;
                            if (gridControl.DataSource != null)
                            {
                                var griddata = (List<V_HIS_EXP_MEST_4>)gridControl.DataSource;
                                var apiresul = new Inventec.Common.Adapter.BackendAdapter(param).Post<HIS_EXP_MEST>("api/HisExpMest/Decline", ApiConsumer.ApiConsumers.MosConsumer, ado, param);
                                if (apiresul != null)
                                {
                                    foreach (var item in griddata)
                                    {
                                        if (item.ID == apiresul.ID)
                                        {
                                            var expMestSTT = BackendDataWorker.Get<HIS_EXP_MEST_STT>().FirstOrDefault(o => o.ID == apiresul.EXP_MEST_STT_ID);
                                            item.EXP_MEST_STT_ID = expMestSTT.ID;
                                            item.EXP_MEST_STT_NAME = expMestSTT.EXP_MEST_STT_NAME;
                                            item.EXP_MEST_STT_CODE = expMestSTT.EXP_MEST_STT_CODE;
                                            item.MODIFY_TIME = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(DateTime.Now);
                                            break;
                                        }
                                    }
                                    success = true;
                                    gridView.BeginUpdate();
                                    griddata = griddata.OrderByDescending(p => p.MODIFY_TIME).ToList();
                                    gridControl.DataSource = griddata;
                                    gridView.EndUpdate();
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
                    }
                    catch (Exception ex)
                    {
                        Inventec.Common.Logging.LogSystem.Error(ex);
                        WaitingManager.Hide();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void HuyTuChoiDuyet(V_HIS_EXP_MEST row)
        {
            try
            {
                if (row != null)
                {
                    try
                    {
                        if (row != null)
                        {
                            WaitingManager.Show();
                            bool success = false;
                            CommonParam param = new CommonParam();
                            HisExpMestSDO hisExpMestApproveSDO = new MOS.SDO.HisExpMestSDO();

                            hisExpMestApproveSDO.ExpMestId = row.ID;
                            //hisExpMestApproveSDO.IsFinish = true;
                            hisExpMestApproveSDO.ReqRoomId = this.roomId;
                            if (gridControl.DataSource != null)
                            {
                                var griddata = (List<V_HIS_EXP_MEST_4>)gridControl.DataSource;
                                var rs = new Inventec.Common.Adapter.BackendAdapter(param).Post<HIS_EXP_MEST>(
                           "api/HisExpMest/Undecline", ApiConsumers.MosConsumer, hisExpMestApproveSDO, param);
                                if (rs != null)
                                {
                                    foreach (var item in griddata)
                                    {
                                        if (item.ID == rs.ID)
                                        {
                                            var expMestSTT = BackendDataWorker.Get<HIS_EXP_MEST_STT>().FirstOrDefault(o => o.ID == rs.EXP_MEST_STT_ID);
                                            item.EXP_MEST_STT_ID = expMestSTT.ID;
                                            item.EXP_MEST_STT_NAME = expMestSTT.EXP_MEST_STT_NAME;
                                            item.EXP_MEST_STT_CODE = expMestSTT.EXP_MEST_STT_CODE;
                                            item.MODIFY_TIME = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(DateTime.Now);
                                            break;
                                        }
                                    }
                                    success = true;
                                    gridView.BeginUpdate();
                                    griddata = griddata.OrderByDescending(p => p.MODIFY_TIME).ToList();
                                    gridControl.DataSource = griddata;
                                    gridView.EndUpdate();
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
                    }
                    catch (Exception ex)
                    {
                        Inventec.Common.Logging.LogSystem.Warn(ex);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ThucXuat(V_HIS_EXP_MEST row)
        {
            try
            {
                if (row != null)
                {
                    try
                    {
                        bool success = false;
                        CommonParam param = new CommonParam();
                        if (row != null)
                        {
                            if (row.EXP_MEST_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__BCS)
                            {
                                bool IsFinish = false;
                                if (row.IS_EXPORT_EQUAL_APPROVE == 1)
                                {
                                    DevExpress.XtraEditors.XtraMessageBox.Show("Đã xuất hết số lượng duyệt", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                    return;
                                }
                                else if (row.IS_EXPORT_EQUAL_APPROVE == null || row.IS_EXPORT_EQUAL_APPROVE != 1)
                                {
                                    HisExpMestMetyReqFilter expMestMetyReqFilter = new HisExpMestMetyReqFilter();
                                    expMestMetyReqFilter.EXP_MEST_ID = row.ID;

                                    var listExpMestMetyReq = new BackendAdapter(param).Get<List<HIS_EXP_MEST_METY_REQ>>("api/HisExpMestMetyReq/Get", ApiConsumers.MosConsumer, expMestMetyReqFilter, param);

                                    HisExpMestMatyReqFilter expMestMatyReqFilter = new HisExpMestMatyReqFilter();
                                    expMestMatyReqFilter.EXP_MEST_ID = row.ID;

                                    var listExpMestMatyReq = new BackendAdapter(param).Get<List<HIS_EXP_MEST_MATY_REQ>>("api/HisExpMestMatyReq/Get", ApiConsumers.MosConsumer, expMestMatyReqFilter, param);

                                    List<AmountADO> amountAdo = new List<AmountADO>();

                                    if (listExpMestMetyReq != null && listExpMestMetyReq.Count > 0)
                                    {
                                        foreach (var item in listExpMestMetyReq)
                                        {
                                            var ado = new AmountADO(item);
                                            amountAdo.Add(ado);
                                        }
                                    }

                                    if (listExpMestMatyReq != null && listExpMestMatyReq.Count > 0)
                                    {
                                        foreach (var item in listExpMestMatyReq)
                                        {
                                            var ado = new AmountADO(item);
                                            amountAdo.Add(ado);
                                        }
                                    }

                                    if (amountAdo != null && amountAdo.Count > 0)
                                    {
                                        var dataAdo = amountAdo.Where(o => o.Amount > o.Dd_Amount || o.Dd_Amount == null).ToList();
                                        //if (dataAdo != null && dataAdo.Count > 0)
                                        //{
                                        //    if (XtraMessageBox.Show("Phiếu chưa duyệt đủ số lượng yêu cầu. Bạn có muốn hoàn thành phiếu xuất?", "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                                        //    {
                                        //        IsFinish = true;
                                        //    }
                                        //}
                                        //else
                                        IsFinish = true;
                                    }

                                }

                                HisExpMestExportSDO sdo = new HisExpMestExportSDO();
                                sdo.ExpMestId = row.ID;
                                sdo.ReqRoomId = this.roomId;
                                sdo.IsFinish = IsFinish;
                                if (gridControl.DataSource != null)
                                {
                                    var griddata = (List<V_HIS_EXP_MEST_4>)gridControl.DataSource;
                                    var apiresult = new Inventec.Common.Adapter.BackendAdapter
                                        (param).Post<MOS.EFMODEL.DataModels.HIS_EXP_MEST>
                                        (ApiConsumer.HisRequestUriStore.HIS_EXP_MEST_EXPORT, ApiConsumer.ApiConsumers.MosConsumer, sdo, param);
                                    if (apiresult != null)
                                    {
                                        foreach (var item in griddata)
                                        {
                                            if (item.ID == apiresult.ID)
                                            {
                                                var expMestSTT = BackendDataWorker.Get<HIS_EXP_MEST_STT>().FirstOrDefault(o => o.ID == apiresult.EXP_MEST_STT_ID);
                                                item.EXP_MEST_STT_ID = expMestSTT.ID;
                                                item.EXP_MEST_STT_NAME = expMestSTT.EXP_MEST_STT_NAME;
                                                item.EXP_MEST_STT_CODE = expMestSTT.EXP_MEST_STT_CODE;
                                                item.MODIFY_TIME = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(DateTime.Now);
                                                break;
                                            }
                                        }
                                        success = true;
                                        gridView.BeginUpdate();
                                        griddata = griddata.OrderByDescending(p => p.MODIFY_TIME).ToList();
                                        gridControl.DataSource = griddata;
                                        gridView.EndUpdate();
                                    }
                                }
                                #region Show message
                                Inventec.Desktop.Common.Message.MessageManager.Show(this.ParentForm, param, success);
                                #endregion

                                #region Process has exception
                                HIS.Desktop.Controls.Session.SessionManager.ProcessTokenLost(param);
                                #endregion

                            }
                            else if (row.EXP_MEST_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__DDT)
                            {
                                HisExpMestSDO sdo = new HisExpMestSDO();
                                sdo.ExpMestId = row.ID;
                                sdo.ReqRoomId = this.roomId;
                                //sdo.IsFinish = true;
                                if (gridControl.DataSource != null)
                                {
                                    var griddata = (List<V_HIS_EXP_MEST_4>)gridControl.DataSource;
                                    var apiresult = new Inventec.Common.Adapter.BackendAdapter
                                        (param).Post<MOS.EFMODEL.DataModels.HIS_EXP_MEST>
                                        ("api/HisExpMest/InPresExport", ApiConsumer.ApiConsumers.MosConsumer, sdo, param);
                                    if (apiresult != null)
                                    {
                                        foreach (var item in griddata)
                                        {
                                            if (item.ID == apiresult.ID)
                                            {
                                                var expMestSTT = BackendDataWorker.Get<HIS_EXP_MEST_STT>().FirstOrDefault(o => o.ID == apiresult.EXP_MEST_STT_ID);
                                                item.EXP_MEST_STT_ID = expMestSTT.ID;
                                                item.EXP_MEST_STT_NAME = expMestSTT.EXP_MEST_STT_NAME;
                                                item.EXP_MEST_STT_CODE = expMestSTT.EXP_MEST_STT_CODE;
                                                item.MODIFY_TIME = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(DateTime.Now);
                                                break;
                                            }
                                        }
                                        success = true;
                                        gridView.BeginUpdate();
                                        griddata = griddata.OrderByDescending(p => p.MODIFY_TIME).ToList();
                                        gridControl.DataSource = griddata;
                                        gridView.EndUpdate();
                                    }
                                }
                                #region Show message
                                Inventec.Desktop.Common.Message.MessageManager.Show(this.ParentForm, param, success);
                                #endregion

                                #region Process has exception
                                HIS.Desktop.Controls.Session.SessionManager.ProcessTokenLost(param);
                                #endregion

                            }
                            else if (row.EXP_MEST_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__THPK)
                            {
                                HisExpMestSDO sdo = new HisExpMestSDO();
                                sdo.ExpMestId = row.ID;
                                sdo.ReqRoomId = this.roomId;
                                //sdo.IsFinish = true;
                                if (gridControl.DataSource != null)
                                {
                                    var griddata = (List<V_HIS_EXP_MEST_4>)gridControl.DataSource;
                                    var apiresult = new Inventec.Common.Adapter.BackendAdapter
                                        (param).Post<MOS.EFMODEL.DataModels.HIS_EXP_MEST>
                                        ("api/HisExpMest/AggrExamExport", ApiConsumer.ApiConsumers.MosConsumer, sdo, param);
                                    if (apiresult != null)
                                    {
                                        foreach (var item in griddata)
                                        {
                                            if (item.ID == apiresult.ID)
                                            {
                                                var expMestSTT = BackendDataWorker.Get<HIS_EXP_MEST_STT>().FirstOrDefault(o => o.ID == apiresult.EXP_MEST_STT_ID);
                                                item.EXP_MEST_STT_ID = expMestSTT.ID;
                                                item.EXP_MEST_STT_NAME = expMestSTT.EXP_MEST_STT_NAME;
                                                item.EXP_MEST_STT_CODE = expMestSTT.EXP_MEST_STT_CODE;
                                                item.MODIFY_TIME = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(DateTime.Now);
                                                break;
                                            }
                                        }
                                        success = true;
                                        gridView.BeginUpdate();
                                        griddata = griddata.OrderByDescending(p => p.MODIFY_TIME).ToList();
                                        gridControl.DataSource = griddata;
                                        gridView.EndUpdate();
                                    }
                                }
                                #region Show message
                                Inventec.Desktop.Common.Message.MessageManager.Show(this.ParentForm, param, success);
                                #endregion

                                #region Process has exception
                                HIS.Desktop.Controls.Session.SessionManager.ProcessTokenLost(param);
                                #endregion

                            }
                            else
                            {
                                HisExpMestExportSDO sdo = new HisExpMestExportSDO();
                                sdo.ExpMestId = row.ID;
                                sdo.ReqRoomId = this.roomId;
                                sdo.IsFinish = true;
                                if (gridControl.DataSource != null)
                                {
                                    var griddata = (List<V_HIS_EXP_MEST_4>)gridControl.DataSource;
                                    var apiresult = new Inventec.Common.Adapter.BackendAdapter
                                        (param).Post<MOS.EFMODEL.DataModels.HIS_EXP_MEST>
                                        (ApiConsumer.HisRequestUriStore.HIS_EXP_MEST_EXPORT, ApiConsumer.ApiConsumers.MosConsumer, sdo, param);
                                    if (apiresult != null)
                                    {
                                        foreach (var item in griddata)
                                        {
                                            if (item.ID == apiresult.ID)
                                            {
                                                var expMestSTT = BackendDataWorker.Get<HIS_EXP_MEST_STT>().FirstOrDefault(o => o.ID == apiresult.EXP_MEST_STT_ID);
                                                item.EXP_MEST_STT_ID = expMestSTT.ID;
                                                item.EXP_MEST_STT_NAME = expMestSTT.EXP_MEST_STT_NAME;
                                                item.EXP_MEST_STT_CODE = expMestSTT.EXP_MEST_STT_CODE;
                                                item.MODIFY_TIME = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(DateTime.Now);
                                                break;
                                            }
                                        }
                                        success = true;
                                        gridView.BeginUpdate();
                                        griddata = griddata.OrderByDescending(p => p.MODIFY_TIME).ToList();
                                        gridControl.DataSource = griddata;
                                        gridView.EndUpdate();
                                    }
                                }
                                #region Show message
                                Inventec.Desktop.Common.Message.MessageManager.Show(this.ParentForm, param, success);
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
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void HuyThucXuat(V_HIS_EXP_MEST row)
        {
            try
            {
                if (row != null)
                {
                    try
                    {
                        if (row != null)
                        {
                            if (row.EXP_MEST_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__THPK)
                            {
                                WaitingManager.Show();
                                bool success = false;
                                CommonParam param = new CommonParam();
                                HisExpMestSDO hisExpMestApproveSDO = new MOS.SDO.HisExpMestSDO();

                                hisExpMestApproveSDO.ExpMestId = row.ID;
                                //hisExpMestApproveSDO.IsFinish = true;
                                hisExpMestApproveSDO.ReqRoomId = this.roomId;
                                if (gridControl.DataSource != null)
                                {
                                    var griddata = (List<V_HIS_EXP_MEST_4>)gridControl.DataSource;
                                    var rs = new Inventec.Common.Adapter.BackendAdapter(param).Post<HIS_EXP_MEST>(
                               "api/HisExpMest/AggrExamUnexport", ApiConsumers.MosConsumer, hisExpMestApproveSDO, param);
                                    if (rs != null)
                                    {
                                        foreach (var item in griddata)
                                        {
                                            if (item.ID == rs.ID)
                                            {
                                                var expMestSTT = BackendDataWorker.Get<HIS_EXP_MEST_STT>().FirstOrDefault(o => o.ID == rs.EXP_MEST_STT_ID);
                                                item.EXP_MEST_STT_ID = expMestSTT.ID;
                                                item.EXP_MEST_STT_NAME = expMestSTT.EXP_MEST_STT_NAME;
                                                item.EXP_MEST_STT_CODE = expMestSTT.EXP_MEST_STT_CODE;
                                                item.MODIFY_TIME = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(DateTime.Now);
                                                break;
                                            }
                                        }
                                        success = true;
                                        gridView.BeginUpdate();
                                        griddata = griddata.OrderByDescending(p => p.MODIFY_TIME).ToList();
                                        gridControl.DataSource = griddata;
                                        gridView.EndUpdate();
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
                            else if (row.EXP_MEST_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__DDT)
                            {
                                WaitingManager.Show();
                                bool success = false;
                                CommonParam param = new CommonParam();
                                HisExpMestSDO hisExpMestApproveSDO = new MOS.SDO.HisExpMestSDO();

                                hisExpMestApproveSDO.ExpMestId = row.ID;
                                //hisExpMestApproveSDO.IsFinish = true;
                                hisExpMestApproveSDO.ReqRoomId = this.roomId;
                                if (gridControl.DataSource != null)
                                {
                                    var griddata = (List<V_HIS_EXP_MEST_4>)gridControl.DataSource;
                                    var rs = new Inventec.Common.Adapter.BackendAdapter(param).Post<HIS_EXP_MEST>(
                               "api/HisExpMest/InPresUnexport", ApiConsumers.MosConsumer, hisExpMestApproveSDO, param);
                                    if (rs != null)
                                    {
                                        foreach (var item in griddata)
                                        {
                                            if (item.ID == rs.ID)
                                            {
                                                var expMestSTT = BackendDataWorker.Get<HIS_EXP_MEST_STT>().FirstOrDefault(o => o.ID == rs.EXP_MEST_STT_ID);
                                                item.EXP_MEST_STT_ID = expMestSTT.ID;
                                                item.EXP_MEST_STT_NAME = expMestSTT.EXP_MEST_STT_NAME;
                                                item.EXP_MEST_STT_CODE = expMestSTT.EXP_MEST_STT_CODE;
                                                item.MODIFY_TIME = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(DateTime.Now);
                                                break;
                                            }
                                        }
                                        success = true;
                                        gridView.BeginUpdate();
                                        griddata = griddata.OrderByDescending(p => p.MODIFY_TIME).ToList();
                                        gridControl.DataSource = griddata;
                                        gridView.EndUpdate();
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
                            else
                            {
                                WaitingManager.Show();
                                bool success = false;
                                CommonParam param = new CommonParam();
                                HisExpMestSDO hisExpMestApproveSDO = new MOS.SDO.HisExpMestSDO();

                                hisExpMestApproveSDO.ExpMestId = row.ID;
                                //hisExpMestApproveSDO.IsFinish = true;
                                hisExpMestApproveSDO.ReqRoomId = this.roomId;
                                if (gridControl.DataSource != null)
                                {
                                    var griddata = (List<V_HIS_EXP_MEST_4>)gridControl.DataSource;
                                    var rs = new Inventec.Common.Adapter.BackendAdapter(param).Post<HIS_EXP_MEST>(
                               "api/HisExpMest/Unexport", ApiConsumers.MosConsumer, hisExpMestApproveSDO, param);
                                    if (rs != null)
                                    {
                                        foreach (var item in griddata)
                                        {
                                            if (item.ID == rs.ID)
                                            {
                                                var expMestSTT = BackendDataWorker.Get<HIS_EXP_MEST_STT>().FirstOrDefault(o => o.ID == rs.EXP_MEST_STT_ID);
                                                item.EXP_MEST_STT_ID = expMestSTT.ID;
                                                item.EXP_MEST_STT_NAME = expMestSTT.EXP_MEST_STT_NAME;
                                                item.EXP_MEST_STT_CODE = expMestSTT.EXP_MEST_STT_CODE;
                                                item.MODIFY_TIME = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(DateTime.Now);
                                                break;
                                            }
                                        }
                                        success = true;
                                        gridView.BeginUpdate();
                                        griddata = griddata.OrderByDescending(p => p.MODIFY_TIME).ToList();
                                        gridControl.DataSource = griddata;
                                        gridView.EndUpdate();
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
                        }
                    }
                    catch (Exception ex)
                    {
                        Inventec.Common.Logging.LogSystem.Warn(ex);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void HoanThanh(V_HIS_EXP_MEST row)
        {
            try
            {
                if (row != null)
                {
                    try
                    {
                        if (row != null)
                        {
                            WaitingManager.Show();
                            bool success = false;
                            CommonParam param = new CommonParam();
                            HisExpMestSDO hisExpMestFinishSDO = new MOS.SDO.HisExpMestSDO();

                            hisExpMestFinishSDO.ExpMestId = row.ID;
                            //hisExpMestApproveSDO.IsFinish = true;
                            hisExpMestFinishSDO.ReqRoomId = this.roomId;
                            if (gridControl.DataSource != null)
                            {
                                var griddata = (List<V_HIS_EXP_MEST_4>)gridControl.DataSource;
                                var rs = new Inventec.Common.Adapter.BackendAdapter(param).Post<HIS_EXP_MEST>(
                           "api/HisExpMest/Finish", ApiConsumers.MosConsumer, hisExpMestFinishSDO, param);
                                if (rs != null)
                                {
                                    foreach (var item in griddata)
                                    {
                                        if (item.ID == rs.ID)
                                        {
                                            var expMestSTT = BackendDataWorker.Get<HIS_EXP_MEST_STT>().FirstOrDefault(o => o.ID == rs.EXP_MEST_STT_ID);
                                            item.EXP_MEST_STT_ID = expMestSTT.ID;
                                            item.EXP_MEST_STT_NAME = expMestSTT.EXP_MEST_STT_NAME;
                                            item.EXP_MEST_STT_CODE = expMestSTT.EXP_MEST_STT_CODE;
                                            item.MODIFY_TIME = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(DateTime.Now);
                                            break;
                                        }
                                    }
                                    success = true;
                                    gridView.BeginUpdate();
                                    griddata = griddata.OrderByDescending(p => p.MODIFY_TIME).ToList();
                                    gridControl.DataSource = griddata;
                                    gridView.EndUpdate();
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
                    }
                    catch (Exception ex)
                    {
                        Inventec.Common.Logging.LogSystem.Warn(ex);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void TaoPhieuNhapHaoPhiTraLai(V_HIS_EXP_MEST row)
        {
            try
            {
                if (row != null)
                {
                    try
                    {
                        WaitingManager.Show();
                        var ExpMestData = row;

                        if (ExpMestData != null)
                        {
                            if (ExpMestData.EXP_MEST_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__HPKP)
                            {
                                List<object> listArgs = new List<object>();
                                listArgs.Add(ExpMestData.ID);
                                CallModule callModule = new CallModule(CallModule.MobaDepaCreate, this.roomId, this.roomTypeId, listArgs);

                                WaitingManager.Hide();
                                FillDataApterClose(ExpMestData);
                            }
                            else if (ExpMestData.EXP_MEST_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__DM)
                            {
                                List<object> listArgs = new List<object>();
                                listArgs.Add(ExpMestData.ID);
                                CallModule callModule = new CallModule(CallModule.MobaBloodCreate, this.roomId, this.roomTypeId, listArgs);

                                WaitingManager.Hide();
                                FillDataApterClose(ExpMestData);
                            }
                            else if (ExpMestData.EXP_MEST_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__BAN)
                            {
                                List<object> listArgs = new List<object>();
                                listArgs.Add(ExpMestData.ID);
                                CallModule callModule = new CallModule(CallModule.MobaSaleCreate, this.roomId, this.roomTypeId, listArgs);

                                WaitingManager.Hide();
                                FillDataApterClose(ExpMestData);
                            }
                        }

                    }
                    catch (Exception ex)
                    {
                        Inventec.Common.Logging.LogSystem.Error(ex);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void TaoPhieuNhapThuHoiMau(V_HIS_EXP_MEST row)
        {
            try
            {
                if (row != null)
                {
                    try
                    {
                        WaitingManager.Show();
                        var ExpMestData = row;

                        if (ExpMestData != null)
                        {
                            if (ExpMestData.EXP_MEST_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__HPKP)
                            {
                                List<object> listArgs = new List<object>();
                                listArgs.Add(ExpMestData.ID);
                                CallModule callModule = new CallModule(CallModule.MobaDepaCreate, this.roomId, this.roomTypeId, listArgs);

                                WaitingManager.Hide();
                                FillDataApterClose(ExpMestData);
                            }
                            else if (ExpMestData.EXP_MEST_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__DM)
                            {
                                List<object> listArgs = new List<object>();
                                listArgs.Add(ExpMestData.ID);
                                CallModule callModule = new CallModule(CallModule.MobaBloodCreate, this.roomId, this.roomTypeId, listArgs);

                                WaitingManager.Hide();
                                FillDataApterClose(ExpMestData);
                            }
                            else if (ExpMestData.EXP_MEST_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__BAN)
                            {
                                List<object> listArgs = new List<object>();
                                listArgs.Add(ExpMestData.ID);
                                CallModule callModule = new CallModule(CallModule.MobaSaleCreate, this.roomId, this.roomTypeId, listArgs);

                                WaitingManager.Hide();
                                FillDataApterClose(ExpMestData);
                            }
                        }

                    }
                    catch (Exception ex)
                    {
                        Inventec.Common.Logging.LogSystem.Error(ex);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void TaoPhieuNhapBuCoSo(V_HIS_EXP_MEST row)
        {
            try
            {
                if (row != null)
                {
                    WaitingManager.Show();

                    List<object> listArgs = new List<object>();
                    listArgs.Add(row);
                    listArgs.Add(row.EXP_MEST_TYPE_ID);
                    listArgs.Add((HIS.Desktop.Common.DelegateRefreshData)RefreshData);
                    CallModule callModule = new CallModule(CallModule.ImpMestChmsCreate, this.roomId, this.roomTypeId, listArgs);

                    WaitingManager.Hide();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void TaoPhieuNhapChuyenKho(V_HIS_EXP_MEST row)
        {
            try
            {
                if (row != null)
                {
                    WaitingManager.Show();

                    List<object> listArgs = new List<object>();
                    listArgs.Add(row);
                    listArgs.Add(row.EXP_MEST_TYPE_ID);
                    listArgs.Add((HIS.Desktop.Common.DelegateRefreshData)RefreshData);
                    CallModule callModule = new CallModule(CallModule.ImpMestChmsCreate, this.roomId, this.roomTypeId, listArgs);

                    WaitingManager.Hide();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void TaoPhieuNhapBuLe(V_HIS_EXP_MEST row)
        {
            try
            {
                if (row != null)
                {
                    WaitingManager.Show();

                    List<object> listArgs = new List<object>();
                    listArgs.Add(row);
                    listArgs.Add(row.EXP_MEST_TYPE_ID);
                    listArgs.Add((HIS.Desktop.Common.DelegateRefreshData)RefreshData);
                    CallModule callModule = new CallModule(CallModule.ImpMestChmsCreate, this.roomId, this.roomTypeId, listArgs);

                    WaitingManager.Hide();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ChiDinhXetNghiem(V_HIS_EXP_MEST row)
        {
            try
            {
                if (row != null)
                {
                    WaitingManager.Show();
                    var expMestData = row;
                    if (expMestData != null)
                    {
                        List<object> listArgs = new List<object>();
                        AssignServiceTestADO assignBloodADO = new AssignServiceTestADO(0, 0, 0, null);
                        GetTreatmentIdFromResultData(expMestData, ref assignBloodADO);
                        listArgs.Add(assignBloodADO);
                        CallModule callModule = new CallModule(CallModule.AssignServiceTest, this.roomId, this.roomTypeId, listArgs);

                        WaitingManager.Hide();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LichSuTacDong(V_HIS_EXP_MEST row)
        {
            try
            {
                if (row != null)
                {
                    var expMestData = row;
                    WaitingManager.Show();

                    List<object> listArgs = new List<object>();
                    Inventec.UC.EventLogControl.Data.DataInit3 dataInit3 = new Inventec.UC.EventLogControl.Data.DataInit3(ConfigSystems.URI_API_SDA, GlobalVariables.APPLICATION_CODE, ConfigApplications.NumPageSize, "EXP_MEST_CODE: " + expMestData.EXP_MEST_CODE);
                    listArgs.Add(dataInit3);
                    listArgs.Add(Inventec.Desktop.Common.Modules.Module.MODULE_TYPE_ID__FORM);
                    CallModule callModule = new CallModule(CallModule.EventLog, this.roomId, this.roomTypeId, listArgs);

                    WaitingManager.Hide();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void Sua(V_HIS_EXP_MEST row)
        {
            try
            {
                if (row != null)
                {
                    CommonParam param = new CommonParam();
                    var expMestData = row;
                    if (expMestData != null)
                    {
                        if (expMestData.EXP_MEST_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__KHAC)
                        {
                            List<object> listArgs = new List<object>();
                            listArgs.Add(expMestData.ID);
                            listArgs.Add((HIS.Desktop.Common.DelegateSelectData)FillDataApterSave);
                            CallModule callModule = new CallModule(CallModule.ExpMestOtherExport, this.roomId, this.roomTypeId, listArgs);

                            RefreshData();
                        }
                        else if (expMestData.EXP_MEST_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__BAN)
                        {
                            List<object> listArgs = new List<object>();
                            listArgs.Add(expMestData.ID);
                            listArgs.Add((HIS.Desktop.Common.DelegateSelectData)FillDataApterSave);
                            CallModule callModule = new CallModule(CallModule.ExpMestSaleCreate, this.roomId, this.roomTypeId, listArgs);

                            RefreshData();
                        }
                        else if (expMestData.EXP_MEST_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__HPKP)
                        {
                            List<object> listArgs = new List<object>();
                            listArgs.Add(expMestData);
                            CallModule callModule = new CallModule(CallModule.ExpMestDepaUpdate, this.roomId, this.roomTypeId, listArgs);

                            RefreshData();
                        }
                        else if (expMestData.EXP_MEST_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__TNCC)
                        {
                            List<object> listArgs = new List<object>();
                            listArgs.Add(expMestData);
                            CallModule callModule = new CallModule(CallModule.ManuExpMestCreate, this.roomId, this.roomTypeId, listArgs);

                            RefreshData();
                        }
                        else
                            MessageManager.Show(Resources.ResourceMessage.ChucNangDangPhatTrienVuiLongThuLaiSau);
                    }
                    else
                        MessageManager.Show(Resources.ResourceMessage.ChucNangDangPhatTrienVuiLongThuLaiSau);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ThuHoiDonPhongKham(V_HIS_EXP_MEST row)
        {
            try
            {
                if (row != null)
                {
                    try
                    {
                        WaitingManager.Show();
                        var ExpMestData = row;

                        if (ExpMestData != null)
                        {
                            WaitingManager.Show();

                            List<object> listObj = new List<object>();
                            listObj.Add(ExpMestData.SERVICE_REQ_ID);
                            CallModule callModule = new CallModule(CallModule.MobaExamPresCreate, this.roomId, this.roomTypeId, listObj);

                            WaitingManager.Hide();
                        }

                    }
                    catch (Exception ex)
                    {
                        Inventec.Common.Logging.LogSystem.Error(ex);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void KhongLayXuatBan(V_HIS_EXP_MEST row)
        {
            try
            {
                if (row != null)
                {
                    if (XtraMessageBox.Show("Bạn có muốn tích KHÔNG LẤY phiếu xuất bán?", "Thông báo", MessageBoxButtons.YesNo, MessageBoxIcon.Question, DefaultBoolean.True) != DialogResult.Yes)
                    {
                        return;
                    }
                    try
                    {
                        WaitingManager.Show();
                        CommonParam param = new CommonParam();
                        bool success = false;
                        Mapper.CreateMap<V_HIS_EXP_MEST, HIS_EXP_MEST>();
                        HIS_EXP_MEST ExpMestData = Mapper.Map<HIS_EXP_MEST>(row);
                        if (gridControl.DataSource != null)
                        {
                            var griddata = (List<V_HIS_EXP_MEST_4>)gridControl.DataSource;
                            var rs = new Inventec.Common.Adapter.BackendAdapter(param).Post<HIS_EXP_MEST>(
                               "api/HisExpMest/NotTaken", ApiConsumers.MosConsumer, ExpMestData, param);
                            if (rs != null)
                            {
                                foreach (var item in griddata)
                                {
                                    if (item.ID == rs.ID)
                                    {
                                        var expMestSTT = BackendDataWorker.Get<HIS_EXP_MEST_STT>().FirstOrDefault(o => o.ID == rs.EXP_MEST_STT_ID);
                                        item.EXP_MEST_STT_ID = expMestSTT.ID;
                                        item.EXP_MEST_STT_NAME = expMestSTT.EXP_MEST_STT_NAME;
                                        item.EXP_MEST_STT_CODE = expMestSTT.EXP_MEST_STT_CODE;
                                        item.MODIFY_TIME = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(DateTime.Now);
                                        break;
                                    }
                                }
                                success = true;
                                gridView.BeginUpdate();
                                griddata = griddata.OrderByDescending(p => p.MODIFY_TIME).ToList();
                                gridControl.DataSource = griddata;
                                gridView.EndUpdate();
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
                        WaitingManager.Hide();
                        Inventec.Common.Logging.LogSystem.Error(ex);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        #endregion

        private void Btn_Check_TruyenMau_Disable_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {

        }

        class CustomerFuncMergeSameData : TFlexCelUserFunction
        {
            List<Base.ExportListCodeRDO> rdo;

            public CustomerFuncMergeSameData(List<Base.ExportListCodeRDO> rdo)
            {
                this.rdo = rdo;
            }

            public override object Evaluate(object[] parameters)
            {
                if (parameters == null || parameters.Length < 1)
                    throw new ArgumentException("Bad parameter count in call to Orders() user-defined function");
                bool result = false;
                try
                {
                    int row = Convert.ToInt32(parameters[0]);
                    long id = Convert.ToInt64(parameters[1]);
                    if (row > 0)
                    {
                        if (rdo[row - 1].ROOM_ID == id)
                        {
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Debug(ex);
                }

                return result;
            }

        }

        private void gridViewStatus_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                string status = "";
                GridView grd = sender as GridView;
                if (grd != null)
                {
                    int[] selectRow = grd.GetSelectedRows();
                    foreach (var item in selectRow)
                    {
                        var selectTex = (HIS_EXP_MEST_STT)grd.GetRow(item);
                        status += selectTex.EXP_MEST_STT_NAME + ", ";
                    }
                }
                cboStatus.Text = status;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewStatus_MouseDown(object sender, MouseEventArgs e)
        {
            try
            {
                DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                GridHitInfo info = view.CalcHitInfo(e.Location);
                if (info.Column != null && info.HitTest == GridHitTest.Column && info.Column.FieldName == "CheckMarkSelection")
                {
                    string status = "";
                    if (view != null)
                    {
                        int[] selectRow = view.GetSelectedRows();
                        foreach (var item in selectRow)
                        {
                            var selectTex = (HIS_EXP_MEST_STT)view.GetRow(item);
                            status += selectTex.EXP_MEST_STT_NAME + ", ";
                        }
                    }
                    cboStatus.Text = status;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

    }
}
