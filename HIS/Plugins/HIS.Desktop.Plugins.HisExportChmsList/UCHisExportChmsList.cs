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
using HIS.Desktop.Plugins.HisExportChmsList.Base;
using HIS.Desktop.IsAdmin;
using HIS.Desktop.ADO;
using Inventec.Common.Controls.EditorLoader;
using System.Reflection;
using DevExpress.XtraEditors.Controls;

namespace HIS.Desktop.Plugins.HisExportChmsList
{
    public partial class UCHisExportChmsList : UserControlBase
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
        List<MOS.EFMODEL.DataModels.V_HIS_EXP_MEST> listExpMest;
        bool IsType = true;
        List<ACS.EFMODEL.DataModels.ACS_CONTROL> controlAcs;
        List<HIS_EXP_MEST_TYPE> expMestTypes;
        List<V_HIS_MEDI_STOCK> medistocks;
        List<V_HIS_ROOM> rooms;
        //ACS.SDO.AcsAuthorizeSDO acsAuthorizeSDO;

        List<HIS_EXP_MEST_STT> _StatusSelecteds;
        List<HIS_EXP_MEST_TYPE> _TypeSelecteds;

        List<HIS_IMP_MEST> listImpMest;

        ToolTip toolTip = new ToolTip();
        V_HIS_ROOM room;

        Inventec.Desktop.Common.Modules.Module currentModule;

        bool _ShowBtnCancelExport = false;
        bool _ShowBtnApprove = false;
        bool _ShowBtnExport = false;
        bool _ShowBtnApproveImp = false;
        bool _ShowBtnImport = false;
        List<long> _ChmsExpMestIds { get; set; }
        HIS.Desktop.Library.CacheClient.ControlStateWorker controlStateWorker;
        List<HIS.Desktop.Library.CacheClient.ControlStateRDO> currentControlStateRDO;

        #endregion

        #region Construct
        public UCHisExportChmsList()
        {
            InitializeComponent();
            try
            {
                gridControl.ToolTipController = this.toolTipController;
                gridControlImp.ToolTipController = this.toolTipController1;
                LoggingName = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public UCHisExportChmsList(Inventec.Desktop.Common.Modules.Module _module)
            : base(_module)
        {
            try
            {
                InitializeComponent();
                gridControl.ToolTipController = this.toolTipController;
                gridControlImp.ToolTipController = this.toolTipController1;
                LoggingName = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
                this.currentModule = _module;
                this.roomId = _module.RoomId;
                this.roomTypeId = _module.RoomTypeId;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void UCHisExportChmsList_Load(object sender, EventArgs e)
        {
            try
            {
                //DoubleBuffered(this.gridControl);
                this.InitControlState();
                CommonParam param = new CommonParam();
                this._ChmsExpMestIds = new List<long>();

                HisMediStockViewFilter medistockFilter = new HisMediStockViewFilter();
                medistockFilter.IS_ACTIVE = 1;
                medistocks = new BackendAdapter(param).Get<List<V_HIS_MEDI_STOCK>>("api/HisMediStock/GetView", ApiConsumers.MosConsumer, medistockFilter, param);

                HisRoomViewFilter roomFilter = new HisRoomViewFilter();
                roomFilter.IS_ACTIVE = 1;
                rooms = new BackendAdapter(param).Get<List<V_HIS_ROOM>>("api/HisRoom/GetView", ApiConsumers.MosConsumer, roomFilter, param);

                HisExpMestTypeFilter expMestTypeFilter = new HisExpMestTypeFilter();
                expMestTypes = new BackendAdapter(param).Get<List<HIS_EXP_MEST_TYPE>>("api/HisExpMestType/Get", ApiConsumers.MosConsumer, expMestTypeFilter, param);

                medistock = medistocks.FirstOrDefault(o => o.ROOM_ID == this.roomId && o.ROOM_TYPE_ID == this.roomTypeId);

                if (medistock != null)
                {
                    var mestRoom = BackendDataWorker.Get<V_HIS_MEST_ROOM>().Where(o => o.MEDI_STOCK_ID == medistock.ID).ToList();
                    if (mestRoom != null && mestRoom.Count > 0)
                    {
                        var medi = medistocks.Where(o => mestRoom.Select(p => p.ROOM_ID).Contains(o.ROOM_ID)).ToList();
                        LoadDataComboMediStock(medi, cboImportMediStock);
                    }
                }

                room = rooms.FirstOrDefault(o => o.ID == this.roomId);

                LoadDataComboMediStock(medistocks, cboExportMediStock);


                if (GlobalVariables.AcsAuthorizeSDO != null)
                {
                    controlAcs = GlobalVariables.AcsAuthorizeSDO.ControlInRoles;
                }

                this.SetShowControlByConfig();

                //Gan ngon ngu
                LoadKeysFromlanguage();

                //Gan gia tri mac dinh
                SetDefaultValueControl();

                //Load du lieu
                RefreshData();

                //focus truong du lieu dau tien
                txtExpMestCode.Focus();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        #endregion

        #region Private method

        private void SetShowControlByConfig()
        {
            try
            {
                if (this.controlAcs != null && this.controlAcs.Count > 0)
                {
                    var dataCheck1 = this.controlAcs.FirstOrDefault(p => p.CONTROL_CODE == ControlCode.BtnApprove);
                    if (dataCheck1 != null)
                        this._ShowBtnApprove = true;
                    var dataCheck2 = this.controlAcs.FirstOrDefault(p => p.CONTROL_CODE == ControlCode.BtnApproveImp);
                    if (dataCheck2 != null)
                        this._ShowBtnApproveImp = true;
                    var dataCheck3 = this.controlAcs.FirstOrDefault(p => p.CONTROL_CODE == ControlCode.BtnCancelExport);
                    if (dataCheck3 != null)
                        this._ShowBtnCancelExport = true;
                    var dataCheck4 = this.controlAcs.FirstOrDefault(p => p.CONTROL_CODE == ControlCode.BtnExport);
                    if (dataCheck4 != null)
                        this._ShowBtnExport = true;
                    var dataCheck5 = this.controlAcs.FirstOrDefault(p => p.CONTROL_CODE == ControlCode.BtnImport);
                    if (dataCheck5 != null)
                        this._ShowBtnImport = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void InitControlState()
        {
            try
            {
                this.controlStateWorker = new HIS.Desktop.Library.CacheClient.ControlStateWorker();
                this.currentControlStateRDO = controlStateWorker.GetData(ControlStateConstant.MODULE_LINK);
                if (this.currentControlStateRDO != null && this.currentControlStateRDO.Count > 0)
                {
                    foreach (var item in this.currentControlStateRDO)
                    {
                        if (item.KEY == ControlStateConstant.CHECK_EXP_MEST_BCS)
                        {
                            chkExpMestBCS.Checked = item.VALUE == "1";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadDataComboMediStock(object data, object control)
        {
            try
            {
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("MEDI_STOCK_CODE", "", 100, 1));
                columnInfos.Add(new ColumnInfo("MEDI_STOCK_NAME", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("MEDI_STOCK_NAME", "ID", columnInfos, false, 350);
                ControlEditorLoader.Load(control, data, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void GetImpMest(List<long> chmsId)
        {
            try
            {
                listImpMest = new List<HIS_IMP_MEST>();
                CommonParam param = new CommonParam();
                HisImpMestFilter filter = new HisImpMestFilter();
                filter.CHMS_EXP_MEST_IDs = chmsId;
                listImpMest = new BackendAdapter(param).Get<List<HIS_IMP_MEST>>("api/HisImpMest/Get", ApiConsumer.ApiConsumers.MosConsumer, filter, param);
                if (listImpMest != null && listImpMest.Count > 0)
                {
                    this._ChmsExpMestIds = listImpMest.Select(p => p.CHMS_EXP_MEST_ID ?? 0).Distinct().ToList();
                }
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
                Resources.ResourceLanguageManager.LanguageResource = new ResourceManager("HIS.Desktop.Plugins.HisExportChmsList.Resources.Lang", typeof(HIS.Desktop.Plugins.HisExportChmsList.UCHisExportChmsList).Assembly);

                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.layoutControl1.Text = Inventec.Common.Resource.Get.Value("UCHisExportChmsList.layoutControl1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl2.Text = Inventec.Common.Resource.Get.Value("UCHisExportChmsList.layoutControl2.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.txtExpMestCode.Properties.NullValuePrompt = Inventec.Common.Resource.Get.Value("UCHisExportChmsList.txtExpMestCode.Properties.NullValuePrompt", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnRefresh.Text = Inventec.Common.Resource.Get.Value("UCHisExportChmsList.btnRefresh.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnSearch.Text = Inventec.Common.Resource.Get.Value("UCHisExportChmsList.btnSearch.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                //this.lciCreateTimeFrom.Text = Inventec.Common.Resource.Get.Value("UCHisExportChmsList.lciCreateTimeFrom.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                //this.lciCreateTimeTo.Text = Inventec.Common.Resource.Get.Value("UCHisExportChmsList.lciCreateTimeTo.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.STT.Caption = Inventec.Common.Resource.Get.Value("UCHisExportChmsList.STT.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn1.Caption = Inventec.Common.Resource.Get.Value("UCHisExportChmsList.gridColumn1.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn15.Caption = Inventec.Common.Resource.Get.Value("UCHisExportChmsList.gridColumn15.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn6.Caption = Inventec.Common.Resource.Get.Value("UCHisExportChmsList.gridColumn6.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn3.Caption = Inventec.Common.Resource.Get.Value("UCHisExportChmsList.gridColumn3.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn20.Caption = Inventec.Common.Resource.Get.Value("UCHisExportChmsList.gridColumn20.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn4.Caption = Inventec.Common.Resource.Get.Value("UCHisExportChmsList.gridColumn4.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn7.Caption = Inventec.Common.Resource.Get.Value("UCHisExportChmsList.gridColumn7.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn8.Caption = Inventec.Common.Resource.Get.Value("UCHisExportChmsList.gridColumn8.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn8.ToolTip = Inventec.Common.Resource.Get.Value("UCHisExportChmsList.gridColumn8.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn2.Caption = Inventec.Common.Resource.Get.Value("UCHisExportChmsList.gridColumn2.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn9.Caption = Inventec.Common.Resource.Get.Value("UCHisExportChmsList.gridColumn9.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn5.Caption = Inventec.Common.Resource.Get.Value("UCHisExportChmsList.gridColumn5.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn16.Caption = Inventec.Common.Resource.Get.Value("UCHisExportChmsList.gridColumn16.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.IMP_MEST_STT_NAME.Caption = Inventec.Common.Resource.Get.Value("UCHisExportChmsList.IMP_MEST_STT_NAME.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn10.Caption = Inventec.Common.Resource.Get.Value("UCHisExportChmsList.gridColumn10.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn10.ToolTip = Inventec.Common.Resource.Get.Value("UCHisExportChmsList.gridColumn10.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.GcExpMestCode.Caption = Inventec.Common.Resource.Get.Value("UCHisExportChmsList.GcExpMestCode.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                //this.GcExpMestTypeName.Caption = Inventec.Common.Resource.Get.Value("UCHisExportChmsList.GcExpMestTypeName.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                //this.GcReqName.Caption = Inventec.Common.Resource.Get.Value("UCHisExportChmsList.GcReqName.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.GcMediStockName.Caption = Inventec.Common.Resource.Get.Value("UCHisExportChmsList.GcMediStockName.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                //this.GcApprovalName.Caption = Inventec.Common.Resource.Get.Value("UCHisExportChmsList.GcApprovalName.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                //this.GcApprovalTime.Caption = Inventec.Common.Resource.Get.Value("UCHisExportChmsList.GcApprovalTime.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                //this.GcExpLoginName.Caption = Inventec.Common.Resource.Get.Value("UCHisExportChmsList.GcExpLoginName.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                //this.GcFinishTime.Caption = Inventec.Common.Resource.Get.Value("UCHisExportChmsList.GcFinishTime.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                //this.GcReqDepartmentCode.Caption = Inventec.Common.Resource.Get.Value("UCHisExportChmsList.GcReqDepartmentCode.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                //this.GcReqDepartmentName.Caption = Inventec.Common.Resource.Get.Value("UCHisExportChmsList.GcReqDepartmentName.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.GcCreateTime.Caption = Inventec.Common.Resource.Get.Value("UCHisExportChmsList.GcCreateTime.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.GcCreator.Caption = Inventec.Common.Resource.Get.Value("UCHisExportChmsList.GcCreator.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.GcModifyTime.Caption = Inventec.Common.Resource.Get.Value("UCHisExportChmsList.GcModifyTime.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.GcModifier.Caption = Inventec.Common.Resource.Get.Value("UCHisExportChmsList.GcModifier.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.ButtonAssignTestDisable.Buttons[0].ToolTip = this.ButtonAssignTest.Buttons[0].ToolTip;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetDefaultValueControl()
        {
            try
            {
                txtExpMestCode.Text = "";
                dtCreateTimeFrom.EditValue = DateTime.Now;
                dtCreateTimeTo.EditValue = DateTime.Now;
                txtExpMestCode.Focus();
                txtExpMestCode.SelectAll();
                txtImpMestCode.Text = "";
                dtExpDateFrom.EditValue = null;
                dtExpDateTo.EditValue = null;
                cboExportMediStock.EditValue = null;
                cboImportMediStock.EditValue = null;
                cboExportMediStock.Enabled = true;
                cboImportMediStock.Enabled = true;

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
                ApiResultObject<List<MOS.EFMODEL.DataModels.V_HIS_EXP_MEST>> apiResult = null;
                MOS.Filter.HisExpMestViewFilter filter = new MOS.Filter.HisExpMestViewFilter();
                SetFilter(ref filter);
                gridView.BeginUpdate();
                apiResult = new Inventec.Common.Adapter.BackendAdapter
                    (paramCommon).GetRO<List<MOS.EFMODEL.DataModels.V_HIS_EXP_MEST>>
                    (ApiConsumer.HisRequestUriStore.HIS_EXP_MEST_GETVIEW, ApiConsumer.ApiConsumers.MosConsumer, filter, paramCommon);
                if (apiResult != null)
                {
                    listExpMest = apiResult.Data;
                    if (listExpMest != null && listExpMest.Count > 0)
                    {
                        GetImpMest(listExpMest.Select(o => o.ID).ToList());
                        FillDataImpMestByExpMest(listExpMest.First());
                        gridControl.DataSource = listExpMest;
                        rowCount = (listExpMest == null ? 0 : listExpMest.Count);
                        dataTotal = (apiResult.Param == null ? 0 : apiResult.Param.Count ?? 0);
                    }
                    else
                    {
                        FillDataImpMestByExpMest(null);
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

        private void SetFilter(ref MOS.Filter.HisExpMestViewFilter filter)
        {
            try
            {
                filter.ORDER_FIELD = "MODIFY_TIME";
                filter.ORDER_DIRECTION = "DESC";

                if (!String.IsNullOrEmpty(txtExpMestCode.Text))
                {
                    string code = txtExpMestCode.Text.Trim();
                    if (code.Length < 12 && checkDigit(code))
                    {
                        code = string.Format("{0:000000000000}", Convert.ToInt64(code));
                        txtExpMestCode.Text = code;
                    }
                    filter.EXP_MEST_CODE__EXACT = code;
                    filter.DATA_DOMAIN_FILTER = true;
                    filter.WORKING_ROOM_ID = roomId;
                    filter.EXP_MEST_TYPE_IDs = new List<long>();
                    filter.EXP_MEST_TYPE_IDs.AddRange(new List<long>
                {
                    IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__CK,
                    IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__BL
                });
                    if (chkExpMestBCS.Checked)
                    {
                        filter.EXP_MEST_TYPE_IDs.Add(IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__BCS);
                    }

                    filter.HAS_AGGR = false;
                }
                else if (!String.IsNullOrEmpty(txtImpMestCode.Text))
                {
                    CommonParam param = new CommonParam();
                    HisImpMestViewFilter impMestViewFilter = new HisImpMestViewFilter();

                    string code = txtImpMestCode.Text.Trim();
                    if (code.Length < 12 && checkDigit(code))
                    {
                        code = string.Format("{0:000000000000}", Convert.ToInt64(code));
                        txtImpMestCode.Text = code;
                    }
                    impMestViewFilter.IMP_MEST_TYPE_IDs = new List<long>();
                    impMestViewFilter.IMP_MEST_TYPE_IDs.AddRange(new List<long>
                     {
                    IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__CK,
                    IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__BL
                });
                    if (chkExpMestBCS.Checked)
                    {
                        impMestViewFilter.IMP_MEST_TYPE_IDs.Add(IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__BCS);
                    }
                    impMestViewFilter.IMP_MEST_CODE__EXACT = code;
                    impMestViewFilter.DATA_DOMAIN_FILTER = true;
                    impMestViewFilter.WORKING_ROOM_ID = roomId;

                    var impMestByCode = new BackendAdapter(param).Get<List<V_HIS_IMP_MEST>>("api/HisImpMest/GetView", ApiConsumers.MosConsumer, impMestViewFilter, param);

                    //gridControlImp.BeginUpdate();
                    //gridControlImp.DataSource = impMestByCode;
                    //gridControlImp.EndUpdate();

                    if (impMestByCode != null && impMestByCode.Count > 0)
                    {
                        filter.ID = impMestByCode.FirstOrDefault().CHMS_EXP_MEST_ID;

                    }
                    else
                    {
                        filter.EXP_MEST_CODE__EXACT = "xxxxxxxx";
                    }

                    filter.DATA_DOMAIN_FILTER = true;
                    filter.WORKING_ROOM_ID = roomId;
                    filter.EXP_MEST_TYPE_IDs = new List<long>();
                    filter.EXP_MEST_TYPE_IDs.AddRange(new List<long>
                {
                    IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__CK,
                    IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__BL
                });
                    if (chkExpMestBCS.Checked)
                    {
                        filter.EXP_MEST_TYPE_IDs.Add(IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__BCS);
                    }
                    filter.HAS_AGGR = false;
                }
                else
                {
                    filter.EXP_MEST_TYPE_IDs = new List<long>();
                    filter.EXP_MEST_TYPE_IDs.AddRange(new List<long>
                {
                    IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__CK,
                    IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__BL
                });
                    if (chkExpMestBCS.Checked)
                    {
                        filter.EXP_MEST_TYPE_IDs.Add(IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__BCS);
                    }

                    if (dtCreateTimeFrom.EditValue != null && dtCreateTimeFrom.DateTime != DateTime.MinValue)
                        filter.CREATE_DATE_FROM = Inventec.Common.TypeConvert.Parse.ToInt64(
                            Convert.ToDateTime(dtCreateTimeFrom.EditValue).ToString("yyyyMMdd") + "000000");

                    if (dtCreateTimeTo.EditValue != null && dtCreateTimeTo.DateTime != DateTime.MinValue)
                        filter.CREATE_DATE_TO = Inventec.Common.TypeConvert.Parse.ToInt64(
                            Convert.ToDateTime(dtCreateTimeTo.EditValue).ToString("yyyyMMdd") + "000000");
                    if (dtExpDateFrom.EditValue != null && dtExpDateFrom.DateTime != DateTime.MinValue)
                        filter.FINISH_DATE_FROM = Inventec.Common.TypeConvert.Parse.ToInt64(
                            Convert.ToDateTime(dtExpDateFrom.EditValue).ToString("yyyyMMdd") + "000000");

                    if (dtExpDateTo.EditValue != null && dtExpDateTo.DateTime != DateTime.MinValue)
                        filter.FINISH_DATE_TO = Inventec.Common.TypeConvert.Parse.ToInt64(
                            Convert.ToDateTime(dtExpDateTo.EditValue).ToString("yyyyMMdd") + "000000");

                    if (medistock != null)
                    {
                        filter.IMP_OR_EXP_MEDI_STOCK_ID = medistock.ID;
                    }

                    if (cboExportMediStock.EditValue != null)
                    {
                        filter.MEDI_STOCK_ID = Inventec.Common.TypeConvert.Parse.ToInt64(cboExportMediStock.EditValue.ToString());
                    }

                    if (cboImportMediStock.EditValue != null)
                    {
                        filter.IMP_MEDI_STOCK_ID = Inventec.Common.TypeConvert.Parse.ToInt64(cboImportMediStock.EditValue.ToString());
                        if (medistock != null)
                            filter.MEDI_STOCK_ID = medistock.ID;
                    }

                }
                filter.HAS_CHMS_TYPE_ID = false;
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

        private void SetFilterType(ref MOS.Filter.HisExpMestViewFilter filter)
        {
            try
            {

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

        private void gridView_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)
                {
                    MOS.EFMODEL.DataModels.V_HIS_EXP_MEST data = (MOS.EFMODEL.DataModels.V_HIS_EXP_MEST)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
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
                        else if (e.Column.FieldName == "FINISH_TIME_STR")
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
                        //        Resources.ResourceLanguageManager.LanguageUCHisExportChmsList,
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
                    V_HIS_EXP_MEST pData = (V_HIS_EXP_MEST)gridView.GetRow(e.RowHandle);
                    long statusIdCheckForButtonEdit = Inventec.Common.TypeConvert.Parse.ToInt64((gridView.GetRowCellValue(e.RowHandle, "EXP_MEST_STT_ID") ?? "").ToString());
                    long mediStockId = Inventec.Common.TypeConvert.Parse.ToInt64((gridView.GetRowCellValue(e.RowHandle, "MEDI_STOCK_ID") ?? "").ToString());
                    long impMediStockId = Inventec.Common.TypeConvert.Parse.ToInt64((gridView.GetRowCellValue(e.RowHandle, "IMP_MEDI_STOCK_ID") ?? "").ToString());
                    long expMestTypeId = Inventec.Common.TypeConvert.Parse.ToInt64((gridView.GetRowCellValue(e.RowHandle, "EXP_MEST_TYPE_ID") ?? "").ToString());
                    string creator = (gridView.GetRowCellValue(e.RowHandle, "CREATOR") ?? "").ToString().Trim();
                    if (e.Column.FieldName == "EDIT_DISPLAY") // sửa
                    {
                        if ((creator == LoggingName || CheckLoginAdmin.IsAdmin(LoggingName))
                            &&
                            (statusIdCheckForButtonEdit == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__DRAFT
                            || statusIdCheckForButtonEdit == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__REQUEST)
                            && expMestTypeId != IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__BL
                            )
                        {
                            e.RepositoryItem = ButtonEnableEdit;
                        }
                        else
                            e.RepositoryItem = ButtonDisableEdit;
                    }
                    else if (e.Column.FieldName == "DISCARD_DISPLAY") //hủy
                    {
                        if ((creator == LoggingName || CheckLoginAdmin.IsAdmin(LoggingName))
                            &&
                            (
                            statusIdCheckForButtonEdit == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__REQUEST
                            || statusIdCheckForButtonEdit == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__DRAFT
                            || statusIdCheckForButtonEdit == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__REJECT)
                            )
                        {

                            e.RepositoryItem = ButtonEnableDiscard;
                        }
                        else
                            e.RepositoryItem = ButtonDisableDiscard;
                    }
                    else if (e.Column.FieldName == "APPROVAL_DISPLAY") //duyet
                    {
                        if (this._ShowBtnApproveImp
                            && medistock != null
                            && medistock.ID == mediStockId
                            && pData.IS_NOT_TAKEN != 1
                            && expMestTypeId != IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__BL
                            && statusIdCheckForButtonEdit != IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__DONE
                            && statusIdCheckForButtonEdit != IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__DRAFT
                            && statusIdCheckForButtonEdit != IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__REJECT
                            )
                        {
                            if (expMestTypeId == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__BCS
                            && (statusIdCheckForButtonEdit == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__REQUEST
                                        || statusIdCheckForButtonEdit == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__EXECUTE))
                            {
                                e.RepositoryItem = ButtonEnableApproval;
                            }
                            else if (expMestTypeId == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__CK
                            && statusIdCheckForButtonEdit == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__REQUEST)
                            {
                                e.RepositoryItem = ButtonEnableApproval;
                            }
                            else
                            {
                                e.RepositoryItem = ButtonDisableApproval;
                            }
                        }
                        else
                        {
                            e.RepositoryItem = ButtonDisableApproval;
                        }
                    }
                    else if (e.Column.FieldName == "DIS_APPROVAL")// Từ chối duyệt duyệt
                    {
                        if (medistock != null && medistock.ID == mediStockId
                                    &&
                                    (statusIdCheckForButtonEdit == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__REQUEST
                                    || statusIdCheckForButtonEdit == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__REJECT)
                                    && expMestTypeId != IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__BL
                            && pData.IS_NOT_TAKEN != 1)
                        {

                            if (statusIdCheckForButtonEdit == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__REJECT)
                            {
                                e.RepositoryItem = Btn_HuyTuChoiDuyet_Enable;
                            }
                            else
                            {
                                e.RepositoryItem = ButtonEnableDisApproval;
                            }
                        }
                        else
                        {
                            e.RepositoryItem = ButtonDisableDisApproval;
                        }
                    }
                    else if (e.Column.FieldName == "EXPORT_DISPLAY")// thực xuất
                    {
                        if (medistock != null && medistock.ID == mediStockId &&
                                    statusIdCheckForButtonEdit == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__EXECUTE
                                    && expMestTypeId != IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__BL
                            && pData.IS_NOT_TAKEN != 1
                            && this._ShowBtnExport)
                        {
                            e.RepositoryItem = ButtonEnableActualExport;
                        }
                        else if (medistock != null
                            && medistock.ID == mediStockId
                            && statusIdCheckForButtonEdit == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__DONE
                            && expMestTypeId != IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__BL
                            && this._ShowBtnCancelExport)
                        {
                            e.RepositoryItem = Btn_HuyThucXuat_Enable;
                        }
                        else
                        {
                            e.RepositoryItem = Btn_HuyThucXuat_Disable;
                        }
                    }
                    else if (e.Column.FieldName == "MOBA_IMP_MEST_CREATE")// Tạo yêu cầu nhập thu hồi
                    {
                        if (statusIdCheckForButtonEdit == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__DONE
                            && (expMestTypeId == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__HPKP
                            || expMestTypeId == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__DM))
                        {
                            if (expMestTypeId == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__HPKP)
                                ButtonEnableMobaImpCreate.Buttons[0].ToolTip = "Tạo nhập hao phí trả lại";
                            else if (expMestTypeId == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__DM)
                                ButtonEnableMobaImpCreate.Buttons[0].ToolTip = "Tạo nhập thu hồi máu";
                            e.RepositoryItem = ButtonEnableMobaImpCreate;
                        }
                        else
                            e.RepositoryItem = ButtonDisableMobaImpCreate;
                    }
                    else if (e.Column.FieldName == "REQUEST_DISPLAY")// Hủy duyệt
                    {
                        if (pData.IS_NOT_TAKEN != 1
                            && statusIdCheckForButtonEdit == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__EXECUTE
                            && medistock != null && medistock.ID == mediStockId
                            && expMestTypeId != IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__BL
                        )
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
                        if (pData.IS_EXPORT_EQUAL_APPROVE == 1)
                        {
                            e.RepositoryItem = NotApprove;
                        }
                        else if (statusIdCheckForButtonEdit == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__EXECUTE)
                            e.RepositoryItem = Approve;
                    }
                    else if (e.Column.FieldName == "ASSIGN_TEST")//chỉ định xét nghiệm
                    {
                        if (
                            expMestTypeId == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__DM && (
                            statusIdCheckForButtonEdit == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__DONE
                            || statusIdCheckForButtonEdit == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__EXECUTE))
                        {
                            e.RepositoryItem = ButtonAssignTest;
                        }
                        else
                            e.RepositoryItem = ButtonAssignTestDisable;
                    }

                    else if (e.Column.FieldName == "MobaDepaCreate")//Tạo hoa phí trả lại
                    {
                        if (expMestTypeId == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__HPKP)
                        {
                            e.RepositoryItem = Btn_MobaDepaCreate_Enable;
                        }
                        else
                            e.RepositoryItem = Btn_MobaDepaCreate_Disable;
                    }
                    else if (e.Column.FieldName == "ImpMestCreate")//Tạo phiếu nhập chuyển kho
                    {
                        if (medistock != null && medistock.ID == impMediStockId
                            && (statusIdCheckForButtonEdit == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__DONE))
                        {
                            if (expMestTypeId == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__BCS)
                            {
                                SetToolTipButton(Btn_ImpMestCreate_Enable, expMestTypeId);
                                e.RepositoryItem = Btn_ImpMestCreate_Enable;
                            }
                            else if (expMestTypeId == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__BL)
                            {
                                SetToolTipButton(Btn_ImpMestCreate_Enable, expMestTypeId);
                                e.RepositoryItem = Btn_ImpMestCreate_Enable;
                            }
                            else if (expMestTypeId == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__CK)
                            {
                                if (this._ChmsExpMestIds.Contains(pData.ID))
                                    e.RepositoryItem = Btn_ImpMestCreate_Disable;
                                else
                                    e.RepositoryItem = Btn_ImpMestCreate_Enable;
                            }
                        }
                        else
                        {
                            e.RepositoryItem = Btn_ImpMestCreate_Disable;
                        }
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

        public void FocusImpCode()
        {
            try
            {
                txtImpMestCode.Focus();
                txtImpMestCode.SelectAll();
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
                var row = (V_HIS_EXP_MEST)gridView.GetFocusedRow();
                if (row != null)
                {
                    FillDataImpMestByExpMest(row);
                }
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
                var row = (V_HIS_EXP_MEST)gridView.GetFocusedRow();
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
                var row = (V_HIS_EXP_MEST)gridView.GetFocusedRow();
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

        private void gridViewType_KeyUp(object sender, KeyEventArgs e)
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

        private void Btn_HuyThucXuat_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                if (MessageBox.Show("Bạn có chắc chắn muốn hủy thực xuất không?", "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    MOS.EFMODEL.DataModels.V_HIS_EXP_MEST row = (MOS.EFMODEL.DataModels.V_HIS_EXP_MEST)gridView.GetFocusedRow();
                    if (row != null)
                    {
                        if (row.EXP_MEST_TYPE_ID != IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__DDT)
                        {
                            WaitingManager.Show();
                            bool success = false;
                            CommonParam param = new CommonParam();
                            HisExpMestSDO hisExpMestApproveSDO = new MOS.SDO.HisExpMestSDO();

                            hisExpMestApproveSDO.ExpMestId = row.ID;
                            //hisExpMestApproveSDO.IsFinish = true;
                            hisExpMestApproveSDO.ReqRoomId = this.roomId;

                            var rs = new Inventec.Common.Adapter.BackendAdapter(param).Post<HIS_EXP_MEST>(
                       "api/HisExpMest/Unexport", ApiConsumers.MosConsumer, hisExpMestApproveSDO, param);
                            if (rs != null)
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
                            HisExpMestSDO hisExpMestApproveSDO = new MOS.SDO.HisExpMestSDO();

                            hisExpMestApproveSDO.ExpMestId = row.ID;
                            //hisExpMestApproveSDO.IsFinish = true;
                            hisExpMestApproveSDO.ReqRoomId = this.roomId;

                            var rs = new Inventec.Common.Adapter.BackendAdapter(param).Post<HIS_EXP_MEST>(
                       "api/HisExpMest/InPresUnexport", ApiConsumers.MosConsumer, hisExpMestApproveSDO, param);
                            if (rs != null)
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
                    }
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
                var expMestData = (MOS.EFMODEL.DataModels.V_HIS_EXP_MEST)gridView.GetFocusedRow();
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
                MOS.EFMODEL.DataModels.V_HIS_EXP_MEST row = (MOS.EFMODEL.DataModels.V_HIS_EXP_MEST)gridView.GetFocusedRow();
                if (row != null)
                {
                    WaitingManager.Show();
                    bool success = false;
                    CommonParam param = new CommonParam();
                    HisExpMestSDO hisExpMestApproveSDO = new MOS.SDO.HisExpMestSDO();

                    hisExpMestApproveSDO.ExpMestId = row.ID;
                    //hisExpMestApproveSDO.IsFinish = true;
                    hisExpMestApproveSDO.ReqRoomId = this.roomId;

                    var rs = new Inventec.Common.Adapter.BackendAdapter(param).Post<HIS_EXP_MEST>(
               "api/HisExpMest/Undecline", ApiConsumers.MosConsumer, hisExpMestApproveSDO, param);
                    if (rs != null)
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
                var data = (V_HIS_EXP_MEST)gridView.GetRow(e.RowHandle);
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

        private void FillDataImpMestByExpMest(V_HIS_EXP_MEST data)
        {
            try
            {
                if (data != null)
                {
                    CommonParam param = new CommonParam();
                    HisImpMestViewFilter filter = new HisImpMestViewFilter();
                    filter.CHMS_EXP_MEST_ID = data.ID;

                    var impMestByExpMest = new BackendAdapter(param).Get<List<V_HIS_IMP_MEST>>("api/HisImpMest/GetView", ApiConsumers.MosConsumer, filter, param);

                    gridControlImp.BeginUpdate();
                    gridControlImp.DataSource = impMestByExpMest;
                    gridControlImp.EndUpdate();
                }
                else if (!String.IsNullOrEmpty(txtImpMestCode.Text))
                {
                    CommonParam param = new CommonParam();
                    HisImpMestViewFilter impMestViewFilter = new HisImpMestViewFilter();

                    string code = txtImpMestCode.Text.Trim();
                    if (code.Length < 12 && checkDigit(code))
                    {
                        code = string.Format("{0:000000000000}", Convert.ToInt64(code));
                        txtImpMestCode.Text = code;
                    }
                    impMestViewFilter.IMP_MEST_TYPE_IDs = new List<long>();
                    impMestViewFilter.IMP_MEST_TYPE_IDs.AddRange(new List<long>
                     {
                    IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__CK,
                    IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__BL
                });
                    if (chkExpMestBCS.Checked)
                    {
                        impMestViewFilter.IMP_MEST_TYPE_IDs.Add(IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__BCS);
                    }
                    impMestViewFilter.IMP_MEST_CODE__EXACT = code;
                    impMestViewFilter.DATA_DOMAIN_FILTER = true;
                    impMestViewFilter.WORKING_ROOM_ID = roomId;

                    var impMestByCode = new BackendAdapter(param).Get<List<V_HIS_IMP_MEST>>("api/HisImpMest/GetView", ApiConsumers.MosConsumer, impMestViewFilter, param);

                    gridControlImp.BeginUpdate();
                    gridControlImp.DataSource = impMestByCode;
                    gridControlImp.EndUpdate();
                }
                else
                {
                    gridControlImp.BeginUpdate();
                    gridControlImp.DataSource = null;
                    gridControlImp.EndUpdate();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        #region import

        private void gridViewImp_CustomRowCellEdit(object sender, CustomRowCellEditEventArgs e)
        {
            try
            {
                if (e.RowHandle >= 0)
                {
                    var data = (V_HIS_IMP_MEST)gridViewImp.GetRow(e.RowHandle);
                    long statusIdCheckForButtonEdit = long.Parse((gridViewImp.GetRowCellValue(e.RowHandle, "IMP_MEST_STT_ID") ?? "").ToString());
                    long typeIdCheckForButtonEdit = long.Parse((gridViewImp.GetRowCellValue(e.RowHandle, "IMP_MEST_TYPE_ID") ?? "").ToString());
                    long mediStockId = Inventec.Common.TypeConvert.Parse.ToInt64((gridViewImp.GetRowCellValue(e.RowHandle, "MEDI_STOCK_ID") ?? "").ToString());
                    string creator = (gridViewImp.GetRowCellValue(e.RowHandle, "CREATOR") ?? "").ToString();
                    if (e.Column.FieldName == "APPROVAL_DISPLAY")//duyệt
                    {
                        if (data.IMP_MEST_TYPE_ID != IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__BCT)
                        {
                            if (this._ShowBtnApprove)//controlAcs != null && controlAcs.FirstOrDefault(o => o.CONTROL_CODE == ControlCode.BtnApprove) != null)
                            {
                                if (medistock != null && medistock.ID == mediStockId &&
                                    (statusIdCheckForButtonEdit == IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_STT.ID__REQUEST))
                                {
                                    if (data != null && data.AGGR_IMP_MEST_ID != null && data.IMP_MEST_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__DNTTL && data.IMP_MEST_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__BCT)
                                    {
                                        e.RepositoryItem = repositoryItemButtonApprovalDisable;
                                    }
                                    else
                                    {
                                        e.RepositoryItem = repositoryItemButtonApprovalEnable;
                                    }
                                }
                                else
                                    e.RepositoryItem = repositoryItemButtonApprovalDisable;
                            }
                            else
                                e.RepositoryItem = repositoryItemButtonApprovalDisable;
                        }
                        else
                        {
                            e.RepositoryItem = repositoryItemButtonApprovalDisable;
                        }
                    }
                    else if (e.Column.FieldName == "DISCARD_DISPLAY")//hủy
                    {
                        if (data.IMP_MEST_TYPE_ID != IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__BCT)
                        {
                            if ((creator == LoggingName || CheckLoginAdmin.IsAdmin(LoggingName)) &&
                                (statusIdCheckForButtonEdit == IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_STT.ID__REQUEST || statusIdCheckForButtonEdit == IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_STT.ID__DRAFT || statusIdCheckForButtonEdit == IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_STT.ID__REJECT) && typeIdCheckForButtonEdit != IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__BL
                                && data.IMP_MEST_TYPE_ID != IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__BCT)
                            {
                                if (medistock != null && medistock.ID == mediStockId)
                                {
                                    if (typeIdCheckForButtonEdit == IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__DNTTL
                                        || typeIdCheckForButtonEdit == IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__DTTTL
                                        || typeIdCheckForButtonEdit == IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__DMTL)
                                    {
                                        e.RepositoryItem = repositoryItemButtonDiscardDisable;
                                    }
                                    else
                                    {
                                        e.RepositoryItem = repositoryItemButtonDiscardEnable;
                                    }
                                }
                                else
                                {
                                    if (typeIdCheckForButtonEdit == IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__DNTTL
                                        || typeIdCheckForButtonEdit == IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__DTTTL
                                        || typeIdCheckForButtonEdit == IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__DMTL)
                                    {
                                        e.RepositoryItem = repositoryItemButtonDiscardEnable;
                                    }
                                    else
                                    {
                                        e.RepositoryItem = repositoryItemButtonDiscardDisable;
                                    }
                                }
                                //    {
                                //if (typeIdCheckForButtonEdit != IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__TH)
                                //{
                                //    if (medistock != null && medistock.ID == mediStockId)
                                //    {
                                //        e.RepositoryItem = repositoryItemButtonDiscardEnable;
                                //    }
                                //    else
                                //    {
                                //        e.RepositoryItem = repositoryItemButtonDiscardDisable;
                                //    }
                                //}
                                //else
                                //{
                                //    if (medistock != null && medistock.ID == mediStockId)
                                //    {
                                //        e.RepositoryItem = repositoryItemButtonDiscardEnable;
                                //    }
                                //    else
                                //    {
                                //        e.RepositoryItem = repositoryItemButtonDiscardDisable;
                                //    }
                                //}
                            }
                            else
                                e.RepositoryItem = repositoryItemButtonDiscardDisable;
                        }
                        else
                        {
                            e.RepositoryItem = repositoryItemButtonDiscardDisable;
                        }
                    }

                    else if (e.Column.FieldName == "EditNCC")//Sửa thông tin nhập NCC
                    {
                        if (data.IMP_MEST_TYPE_ID != IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__BCT)
                        {
                            if ((creator == LoggingName || CheckLoginAdmin.IsAdmin(LoggingName)) &&
                                (typeIdCheckForButtonEdit == IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__NCC && data.IMP_MEST_TYPE_ID != IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__BCT))
                            {
                                e.RepositoryItem = Btn_EditInfoImpMestNCC_Enable;
                            }
                            else
                                e.RepositoryItem = Btn_EditInfoImpMestNCC_Disable;
                        }
                        else
                        {
                            e.RepositoryItem = Btn_EditInfoImpMestNCC_Disable;
                        }
                    }

                    else if (e.Column.FieldName == "CreateExpNCC")//Xuất trả ncc
                    {
                        if (data.IMP_MEST_TYPE_ID != IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__BCT)
                        {
                            if (typeIdCheckForButtonEdit == IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__NCC && statusIdCheckForButtonEdit == IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_STT.ID__IMPORT && data.IMP_MEST_TYPE_ID != IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__BCT)
                            {
                                e.RepositoryItem = Btn_ExportNCC_Enable;
                            }
                            else
                                e.RepositoryItem = Btn_ExportNCC_Disable;
                        }
                        else
                        {
                            e.RepositoryItem = Btn_ExportNCC_Disable;
                        }
                    }

                    else if (e.Column.FieldName == "IMPORT_DISPLAY")// thực nhập
                    {
                        if (data.IMP_MEST_TYPE_ID != IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__BCT)
                        {
                            if (medistock != null && medistock.ID == mediStockId &&
                               statusIdCheckForButtonEdit == IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_STT.ID__APPROVAL
                               && data.IMP_MEST_TYPE_ID != IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__BCT)
                            {
                                if (data != null && data.AGGR_IMP_MEST_ID != null && data.IMP_MEST_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__DNTTL)
                                {
                                    if (this._ShowBtnImport)//controlAcs != null && controlAcs.FirstOrDefault(o => o.CONTROL_CODE == ControlCode.BtnImport) != null)
                                    {
                                        e.RepositoryItem = repositoryItemButtonActualImportDisable;
                                    }
                                    else
                                    {
                                        e.RepositoryItem = repositoryItemButtonActualImportEnable;
                                    }
                                }
                                else
                                {
                                    e.RepositoryItem = repositoryItemButtonActualImportEnable;
                                }

                            }
                            else if (
                                statusIdCheckForButtonEdit == IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_STT.ID__IMPORT
                                && data.IMP_MEST_TYPE_ID != IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__BCT)
                            {
                                if (medistock != null && medistock.ID == mediStockId)
                                {
                                    if (data != null && data.AGGR_IMP_MEST_ID != null && data.IMP_MEST_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__DNTTL)
                                    {
                                        e.RepositoryItem = Btn_Cancel_Import_Disable;
                                    }
                                    else
                                    {
                                        e.RepositoryItem = Btn_Cancel_Import_Enable;
                                    }
                                }
                                else
                                    e.RepositoryItem = Btn_Cancel_Import_Disable;
                            }
                            else
                                e.RepositoryItem = repositoryItemButtonActualImportDisable;
                        }
                        else
                        {
                            e.RepositoryItem = repositoryItemButtonActualImportDisable;
                        }
                    }
                    else if (e.Column.FieldName == "EDIT")// sửa
                    {
                        if (data.IMP_MEST_TYPE_ID != IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__BCT)
                        {
                            if ((creator == LoggingName || CheckLoginAdmin.IsAdmin(LoggingName))
                                 && (statusIdCheckForButtonEdit == IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_STT.ID__DRAFT || statusIdCheckForButtonEdit == IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_STT.ID__REQUEST)
                                 &&
                                (typeIdCheckForButtonEdit != IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__TH &&
                                typeIdCheckForButtonEdit != IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__CK && data.IMP_MEST_TYPE_ID != IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__BCT))
                            {
                                if (medistock != null && medistock.ID == mediStockId)
                                {
                                    if (typeIdCheckForButtonEdit == IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__DNTTL
                                        || typeIdCheckForButtonEdit == IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__DTTTL
                                        || typeIdCheckForButtonEdit == IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__DMTL)
                                    {
                                        e.RepositoryItem = repositoryItemButtonEditDisable;
                                    }
                                    else
                                    {
                                        e.RepositoryItem = repositoryItemButtonEditEnable;
                                    }
                                }
                                else
                                {
                                    if (typeIdCheckForButtonEdit == IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__DNTTL
                                        || typeIdCheckForButtonEdit == IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__DTTTL
                                        || typeIdCheckForButtonEdit == IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__DMTL)
                                    {
                                        e.RepositoryItem = repositoryItemButtonEditDisable;
                                    }
                                    else
                                    {
                                        e.RepositoryItem = repositoryItemButtonEditEnable;
                                    }
                                }
                            }
                            else
                                e.RepositoryItem = repositoryItemButtonEditDisable;
                        }
                        else
                        {
                            e.RepositoryItem = repositoryItemButtonEditDisable;
                        }
                    }
                    else if (e.Column.FieldName == "DIS_APPROVAL")// Không duyệt duyệt
                    {
                        if (data.IMP_MEST_TYPE_ID != IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__BCT)
                        {
                            if (medistock != null && medistock.ID == mediStockId &&
                                (statusIdCheckForButtonEdit == IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_STT.ID__REQUEST || statusIdCheckForButtonEdit == IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_STT.ID__REJECT) && data.IMP_MEST_TYPE_ID != IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__BCT)
                            {
                                if (statusIdCheckForButtonEdit == IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_STT.ID__REJECT)
                                {
                                    if (data != null && data.AGGR_IMP_MEST_ID != null && data.IMP_MEST_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__DNTTL)
                                    {
                                        e.RepositoryItem = repositoryItemButtonDisApprovalEnable;
                                    }
                                    else
                                    {
                                        e.RepositoryItem = Btn_HuyTuChoiDuyet_EnableImp;
                                    }
                                }
                                else
                                {
                                    if (data != null && data.AGGR_IMP_MEST_ID != null && data.IMP_MEST_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__DNTTL)
                                    {
                                        e.RepositoryItem = repositoryItemButtonDisApprovalDisable;
                                    }
                                    else
                                    {
                                        e.RepositoryItem = repositoryItemButtonDisApprovalEnable;
                                    }
                                }
                            }
                            else
                            {
                                e.RepositoryItem = repositoryItemButtonDisApprovalDisable;
                            }
                        }
                        else
                        {
                            e.RepositoryItem = repositoryItemButtonDisApprovalDisable;
                        }
                    }
                    else if (e.Column.FieldName == "REQUEST_DISPLAY")// Hủy duyệt
                    {
                        if (data.IMP_MEST_TYPE_ID != IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__BCT)
                        {
                            if (medistock != null && medistock.ID == mediStockId &&
                                (statusIdCheckForButtonEdit == IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_STT.ID__APPROVAL) && data.IMP_MEST_TYPE_ID != IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__BCT)
                            {
                                if (data != null && data.AGGR_IMP_MEST_ID != null && data.IMP_MEST_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__DNTTL)
                                {
                                    e.RepositoryItem = repositoryItemButtonRequestDisable;
                                }
                                else
                                {
                                    e.RepositoryItem = repositoryItemButtonRequest;
                                }
                            }
                            else
                            {
                                e.RepositoryItem = repositoryItemButtonRequestDisable;
                            }
                        }
                        else
                        {
                            e.RepositoryItem = repositoryItemButtonRequestDisable;
                        }
                    }
                    //else if (e.Column.FieldName == "DONE")// Hủy duyệt
                    //{
                    //    if (medistock != null && medistock.ID == mediStockId &&
                    //        (statusIdCheckForButtonEdit == IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_STT.ID__APPROVAL)&&
                    //       (typeIdCheckForButtonEdit == IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__CK||
                    //        typeIdCheckForButtonEdit == IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__BCS ||
                    //        typeIdCheckForButtonEdit == IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__HCS
                    //        ))
                    //    {
                    //        e.RepositoryItem = Btn_Done_Enable;
                    //    }
                    //    else
                    //    {
                    //        e.RepositoryItem = repositoryItemButtonRequestDisable;
                    //    }
                    //}
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewImp_CustomUnboundColumnData(object sender, CustomColumnDataEventArgs e)
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
                            else if (data.IMP_MEST_STT_ID == IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_STT.ID__REQUEST)//yêu cầu
                            {
                                e.Value = imageListStatus.Images[1];
                            }
                            else if (data.IMP_MEST_STT_ID == IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_STT.ID__REJECT)//// tu choi
                            {
                                e.Value = imageListStatus.Images[2];
                            }
                            else if (data.IMP_MEST_STT_ID == IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_STT.ID__APPROVAL) // duyet
                            {
                                e.Value = imageListStatus.Images[3];
                            }
                            else if (data.IMP_MEST_STT_ID == IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_STT.ID__IMPORT) // da nhap
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
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void Btn_EditInfoImpMestNCC_EnableImp_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                var ViewImportMest = (MOS.EFMODEL.DataModels.V_HIS_IMP_MEST)gridViewImp.GetFocusedRow();
                if (ViewImportMest != null)
                {
                    //hien thi popup chi tiet
                    WaitingManager.Show();

                    List<object> listArgs = new List<object>();
                    listArgs.Add(ViewImportMest);
                    CallModule callModule = new CallModule(CallModule.ManuImpMestEdit, this.roomId, this.roomTypeId, listArgs);

                    WaitingManager.Hide();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void ManuExpMestCreateClick(V_HIS_IMP_MEST impMest)
        {
            try
            {
                WaitingManager.Show();

                List<object> listArgs = new List<object>();
                Inventec.Desktop.Common.Modules.Module currentModule = new Inventec.Desktop.Common.Modules.Module();
                listArgs.Add(impMest);
                CallModule callModule = new CallModule(CallModule.ManuExpMestCreate, this.roomId, this.roomTypeId, listArgs);

                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void EditInfoImportNCC(V_HIS_IMP_MEST impMest)
        {
            try
            {
                //hien thi popup chi tiet
                WaitingManager.Show();
                List<object> listArgs = new List<object>();
                listArgs.Add(impMest);
                CallModule callModule = new CallModule(CallModule.ManuImpMestEdit, this.roomId, this.roomTypeId, listArgs);

                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void Btn_ExportNCC_EnableImp_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                var impMest = (V_HIS_IMP_MEST)gridViewImp.GetFocusedRow();
                if (impMest != null)
                {
                    ManuExpMestCreateClick(impMest);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void Btn_Done_EnableImp_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void Btn_Cancel_Import_EnableImp_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                if (MessageBox.Show("Bạn có chắc chắn muốn hủy thực nhập không?", "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    WaitingManager.Show();
                    bool success = false;
                    CommonParam param = new CommonParam();
                    MOS.EFMODEL.DataModels.V_HIS_IMP_MEST VImportMest = (MOS.EFMODEL.DataModels.V_HIS_IMP_MEST)gridViewImp.GetFocusedRow();
                    MOS.EFMODEL.DataModels.HIS_IMP_MEST EVImportMest = new MOS.EFMODEL.DataModels.HIS_IMP_MEST();
                    Inventec.Common.Mapper.DataObjectMapper.Map
                        <MOS.EFMODEL.DataModels.HIS_IMP_MEST>
                        (EVImportMest, VImportMest);

                    EVImportMest.IMP_MEST_STT_ID = IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_STT.ID__APPROVAL;
                    var apiresul = new Inventec.Common.Adapter.BackendAdapter
                        (param).Post<MOS.EFMODEL.DataModels.HIS_IMP_MEST>
                        ("api/HisImpMest/CancelImport", ApiConsumer.ApiConsumers.MosConsumer, EVImportMest, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, param);
                    if (apiresul != null)
                    {
                        success = true;
                        RefreshData();
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
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void Btn_EvenLogImp_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                var impMest = (MOS.EFMODEL.DataModels.V_HIS_IMP_MEST)gridViewImp.GetFocusedRow();
                if (impMest != null)
                {
                    //hien thi popup chi tiet
                    WaitingManager.Show();
                    List<object> listArgs = new List<object>();
                    Inventec.UC.EventLogControl.Data.DataInit3 dataInit3 = new Inventec.UC.EventLogControl.Data.DataInit3(ConfigSystems.URI_API_SDA, GlobalVariables.APPLICATION_CODE, ConfigApplications.NumPageSize, "IMP_MEST_CODE: " + impMest.IMP_MEST_CODE);
                    listArgs.Add(dataInit3);
                    listArgs.Add(Inventec.Desktop.Common.Modules.Module.MODULE_TYPE_ID__FORM);
                    CallModule callModule = new CallModule(CallModule.EventLog, this.roomId, this.roomTypeId, listArgs);

                    WaitingManager.Hide();
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void Btn_HuyTuChoiDuyet_EnableImp_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                WaitingManager.Show();
                bool success = false;
                CommonParam param = new CommonParam();
                MOS.EFMODEL.DataModels.V_HIS_IMP_MEST VImportMest = (MOS.EFMODEL.DataModels.V_HIS_IMP_MEST)gridViewImp.GetFocusedRow();
                MOS.EFMODEL.DataModels.HIS_IMP_MEST EVImportMest = new MOS.EFMODEL.DataModels.HIS_IMP_MEST();
                Inventec.Common.Mapper.DataObjectMapper.Map
                    <MOS.EFMODEL.DataModels.HIS_IMP_MEST>
                    (EVImportMest, VImportMest);

                EVImportMest.IMP_MEST_STT_ID = IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_STT.ID__REQUEST;
                var apiresul = new Inventec.Common.Adapter.BackendAdapter
                    (param).Post<MOS.EFMODEL.DataModels.HIS_IMP_MEST>
                    ("api/HisImpMest/UpdateStatus", ApiConsumer.ApiConsumers.MosConsumer, EVImportMest, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, param);
                if (apiresul != null)
                {
                    success = true;
                    RefreshData();
                }
                WaitingManager.Hide();
                #region Show message
                MessageManager.Show(this.ParentForm, param, success);
                #endregion

                #region Process has exception
                HIS.Desktop.Controls.Session.SessionManager.ProcessTokenLost(param);
                #endregion
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void repositoryItemButtonViewDetailImp_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                var ViewImportMest = (MOS.EFMODEL.DataModels.V_HIS_IMP_MEST)gridViewImp.GetFocusedRow();
                //hien thi popup chi tiet
                WaitingManager.Show();

                if (ViewImportMest.IMP_MEST_TYPE_ID != IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__TH)
                {
                    ImpMestViewDetailADO impMestView = new ImpMestViewDetailADO(ViewImportMest.ID, ViewImportMest.IMP_MEST_TYPE_ID, ViewImportMest.IMP_MEST_STT_ID);
                    List<object> listArgs = new List<object>();
                    listArgs.Add(impMestView);
                    //listArgs.Add((HIS.Desktop.Common.DelegateSelectData)FillDataApterSave);
                    CallModule callModule = new CallModule(CallModule.ImpMestViewDetail, this.roomId, this.roomTypeId, listArgs);

                    WaitingManager.Hide();
                }
                else
                {
                    List<object> listArgs = new List<object>();
                    listArgs.Add(ViewImportMest.ID);
                    CallModule callModule = new CallModule(CallModule.ApproveAggrImpMest, this.roomId, this.roomTypeId, listArgs);

                    WaitingManager.Hide();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        //private void FillDataApterSave(object data)
        //{
        //    try
        //    {
        //        if (data != null)
        //        {
        //            //FillDataImportMestList();
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Inventec.Common.Logging.LogSystem.Error(ex);
        //    }
        //}

        private void repositoryItemButtonEditEnableImp_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                var ViewImportMest = (MOS.EFMODEL.DataModels.V_HIS_IMP_MEST)gridViewImp.GetFocusedRow();
                WaitingManager.Show();
                V_HIS_IMP_MEST_1 impMest1View = null;
                CommonParam param = new CommonParam();
                HisImpMestView1Filter ipmMestView1Filter = new HisImpMestView1Filter();
                ipmMestView1Filter.ID = ViewImportMest.ID;
                var listImpMestView1 = new BackendAdapter(param).Get<List<V_HIS_IMP_MEST_1>>("api/HisImpMest/GetView1", ApiConsumer.ApiConsumers.MosConsumer, ipmMestView1Filter, param);
                if (listImpMestView1 != null && listImpMestView1.Count > 0)
                {
                    impMest1View = listImpMestView1.FirstOrDefault();
                }

                if (impMest1View != null && impMest1View.IS_BLOOD != 1)
                {
                    if (ViewImportMest.IMP_MEST_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__NCC || ViewImportMest.IMP_MEST_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__DK || ViewImportMest.IMP_MEST_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__KK || ViewImportMest.IMP_MEST_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__KHAC)
                    {
                        List<object> listArgs = new List<object>();
                        listArgs.Add(ViewImportMest.ID);
                        //listArgs.Add((HIS.Desktop.Common.RefeshReference)FillDataImportMestList);
                        CallModule callModule = new CallModule(CallModule.ManuImpMestUpdate, this.roomId, this.roomTypeId, listArgs);
                        RefreshData();

                        WaitingManager.Hide();
                    }
                    else
                    {
                        WaitingManager.Hide();
                        MessageManager.Show(Resources.ResourceMessage.ChucNangDangPhatTrienVuiLongThuLaiSau);
                    }
                }
                else
                {
                    WaitingManager.Hide();
                    MessageManager.Show(Resources.ResourceMessage.ChucNangDangPhatTrienVuiLongThuLaiSau);
                }

            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void repositoryItemButtonDiscardEnableImp_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            CommonParam param = new CommonParam();
            try
            {
                bool success = false;
                if (DevExpress.XtraEditors.XtraMessageBox.Show(
                    "Bạn có muốn xóa dữ liệu không?",
                    Resources.ResourceMessage.ThongBao,
                    MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    MOS.EFMODEL.DataModels.V_HIS_IMP_MEST row = (MOS.EFMODEL.DataModels.V_HIS_IMP_MEST)gridViewImp.GetFocusedRow();
                    if (row != null)
                    {
                        WaitingManager.Show();
                        MOS.EFMODEL.DataModels.HIS_IMP_MEST data = new MOS.EFMODEL.DataModels.HIS_IMP_MEST();
                        Inventec.Common.Mapper.DataObjectMapper.Map<MOS.EFMODEL.DataModels.HIS_IMP_MEST>(data, row);

                        var apiresult = new Inventec.Common.Adapter.BackendAdapter
                            (param).Post<bool>
                            ("api/HisImpMest/Delete", ApiConsumer.ApiConsumers.MosConsumer, data, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, param);
                        if (apiresult)
                        {
                            success = true;
                            RefreshData();
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
                Inventec.Common.Logging.LogSystem.Fatal(ex);
                WaitingManager.Hide();
            }
        }

        private void repositoryItemButtonApprovalEnableImp_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                WaitingManager.Show();
                bool success = false;
                CommonParam param = new CommonParam();
                MOS.EFMODEL.DataModels.V_HIS_IMP_MEST VImportMest = (MOS.EFMODEL.DataModels.V_HIS_IMP_MEST)gridViewImp.GetFocusedRow();
                MOS.EFMODEL.DataModels.HIS_IMP_MEST EVImportMest = new MOS.EFMODEL.DataModels.HIS_IMP_MEST();
                Inventec.Common.Mapper.DataObjectMapper.Map
                    <MOS.EFMODEL.DataModels.HIS_IMP_MEST>
                    (EVImportMest, VImportMest);

                EVImportMest.IMP_MEST_STT_ID = IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_STT.ID__APPROVAL;
                var apiresul = new Inventec.Common.Adapter.BackendAdapter
                    (param).Post<MOS.EFMODEL.DataModels.HIS_IMP_MEST>
                    ("api/HisImpMest/UpdateStatus", ApiConsumer.ApiConsumers.MosConsumer, EVImportMest, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, param);
                if (apiresul != null)
                {
                    success = true;
                    RefreshData();
                }
                WaitingManager.Hide();
                #region Show message
                MessageManager.Show(this.ParentForm, param, success);
                #endregion

                #region Process has exception
                HIS.Desktop.Controls.Session.SessionManager.ProcessTokenLost(param);
                #endregion
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void repositoryItemButtonDisApprovalEnableImp_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                WaitingManager.Show();
                bool success = false;
                CommonParam param = new CommonParam();
                MOS.EFMODEL.DataModels.V_HIS_IMP_MEST VImportMest = (MOS.EFMODEL.DataModels.V_HIS_IMP_MEST)gridViewImp.GetFocusedRow();
                MOS.EFMODEL.DataModels.HIS_IMP_MEST EVImportMest = new MOS.EFMODEL.DataModels.HIS_IMP_MEST();
                Inventec.Common.Mapper.DataObjectMapper.Map
                    <MOS.EFMODEL.DataModels.HIS_IMP_MEST>
                    (EVImportMest, VImportMest);
                EVImportMest.IMP_MEST_STT_ID = IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_STT.ID__REJECT;
                var apiresul = new Inventec.Common.Adapter.BackendAdapter
                    (param).Post<MOS.EFMODEL.DataModels.HIS_IMP_MEST>
                    ("api/HisImpMest/UpdateStatus", ApiConsumer.ApiConsumers.MosConsumer, EVImportMest, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, param);
                if (apiresul != null)
                {
                    success = true;
                    RefreshData();
                }
                WaitingManager.Hide();
                #region Show message
                MessageManager.Show(this.ParentForm, param, success);
                #endregion

                #region Process has exception
                HIS.Desktop.Controls.Session.SessionManager.ProcessTokenLost(param);
                #endregion
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                WaitingManager.Hide();
            }
        }

        private void repositoryItemButtonActualImportEnableImp_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            CommonParam param = new CommonParam();
            try
            {
                bool success = false;
                if (DevExpress.XtraEditors.XtraMessageBox.Show(
                    "Bạn có muốn thực nhập dữ liệu không?",
                    Resources.ResourceMessage.ThongBao,
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    MOS.EFMODEL.DataModels.V_HIS_IMP_MEST row = (MOS.EFMODEL.DataModels.V_HIS_IMP_MEST)gridViewImp.GetFocusedRow();
                    if (row != null)
                    {
                        WaitingManager.Show();
                        MOS.EFMODEL.DataModels.HIS_IMP_MEST data = new MOS.EFMODEL.DataModels.HIS_IMP_MEST();
                        Inventec.Common.Mapper.DataObjectMapper.Map
                            <MOS.EFMODEL.DataModels.HIS_IMP_MEST>
                            (data, row);
                        data.IMP_MEST_STT_ID = IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_STT.ID__IMPORT;
                        var apiresult = new Inventec.Common.Adapter.BackendAdapter
                            (param).Post<MOS.EFMODEL.DataModels.HIS_IMP_MEST>
                            ("api/HisImpMest/Import", ApiConsumer.ApiConsumers.MosConsumer, data, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, param);
                        if (apiresult != null)
                        {
                            success = true;
                            RefreshData();
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
                Inventec.Common.Logging.LogSystem.Warn(ex);
                WaitingManager.Hide();
            }
        }

        private void repositoryItemButtonRequestImp_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                if (MessageBox.Show("Bạn có chắc chắn muốn hủy duyệt không?", "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    WaitingManager.Show();
                    bool success = false;
                    CommonParam param = new CommonParam();
                    MOS.EFMODEL.DataModels.V_HIS_IMP_MEST VImportMest = (MOS.EFMODEL.DataModels.V_HIS_IMP_MEST)gridViewImp.GetFocusedRow();
                    MOS.EFMODEL.DataModels.HIS_IMP_MEST EVImportMest = new MOS.EFMODEL.DataModels.HIS_IMP_MEST();
                    Inventec.Common.Mapper.DataObjectMapper.Map
                        <MOS.EFMODEL.DataModels.HIS_IMP_MEST>
                        (EVImportMest, VImportMest);

                    EVImportMest.IMP_MEST_STT_ID = IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_STT.ID__REQUEST;
                    var apiresul = new Inventec.Common.Adapter.BackendAdapter
                        (param).Post<MOS.EFMODEL.DataModels.HIS_IMP_MEST>
                        ("api/HisImpMest/UpdateStatus", ApiConsumer.ApiConsumers.MosConsumer, EVImportMest, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, param);
                    if (apiresul != null)
                    {
                        success = true;
                        RefreshData();
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
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }


        private void toolTipController1_GetActiveObjectInfo(object sender, ToolTipControllerGetActiveObjectInfoEventArgs e)
        {
            try
            {
                if (e.Info == null && e.SelectedControl == gridControlImp)
                {
                    DevExpress.XtraGrid.Views.Grid.GridView view = gridControlImp.FocusedView as DevExpress.XtraGrid.Views.Grid.GridView;
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

        #endregion

        private void cboImportMediStock_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (cboImportMediStock.EditValue != null)
                    cboExportMediStock.Enabled = false;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void cboExportMediStock_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (cboExportMediStock.EditValue != null)
                    cboImportMediStock.Enabled = false;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void txtExpMestCode_KeyUp(object sender, KeyEventArgs e)
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
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void txtImpMestCode_KeyUp(object sender, KeyEventArgs e)
        {

        }

        private void gridView_LeftCoordChanged(object sender, EventArgs e)
        {
            //try
            //{
            //    GridView view = sender as GridView;
            //    view.GridControl.Update();
            //}
            //catch (Exception ex)
            //{
            //    Inventec.Common.Logging.LogSystem.Warn(ex);
            //}

        }

        private void DoubleBuffered(DevExpress.XtraGrid.GridControl grc)
        {
            //Type dgvType = dgv.GetType();
            //PropertyInfo pi = dgvType.GetProperty("DoubleBuffered",
            //    BindingFlags.Instance | BindingFlags.NonPublic);
            //pi.SetValue(dgv, setting, null);

            MethodInfo getStyle = typeof(Control).GetMethod("GetStyle",
    BindingFlags.Instance | BindingFlags.NonPublic);
            bool dbGrid = (bool)getStyle.Invoke(grc, new object[] { ControlStyles.DoubleBuffer });
        }

        private void cboImportMediStock_Properties_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    cboImportMediStock.EditValue = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void cboExportMediStock_Properties_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    cboExportMediStock.EditValue = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void chkExpMestBCS_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                WaitingManager.Show();
                HIS.Desktop.Library.CacheClient.ControlStateRDO csAddOrUpdate = (this.currentControlStateRDO != null && this.currentControlStateRDO.Count > 0) ? this.currentControlStateRDO.Where(o => o.KEY == ControlStateConstant.CHECK_EXP_MEST_BCS && o.MODULE_LINK == ControlStateConstant.MODULE_LINK).FirstOrDefault() : null;
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => csAddOrUpdate), csAddOrUpdate));
                if (csAddOrUpdate != null)
                {
                    csAddOrUpdate.VALUE = (chkExpMestBCS.Checked ? "1" : "");
                }
                else
                {
                    csAddOrUpdate = new HIS.Desktop.Library.CacheClient.ControlStateRDO();
                    csAddOrUpdate.KEY = ControlStateConstant.CHECK_EXP_MEST_BCS;
                    csAddOrUpdate.VALUE = (chkExpMestBCS.Checked ? "1" : "");
                    csAddOrUpdate.MODULE_LINK = ControlStateConstant.MODULE_LINK;
                    if (this.currentControlStateRDO == null)
                        this.currentControlStateRDO = new List<HIS.Desktop.Library.CacheClient.ControlStateRDO>();
                    this.currentControlStateRDO.Add(csAddOrUpdate);
                }
                this.controlStateWorker.SetData(this.currentControlStateRDO);
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtImpMestCode_KeyDown(object sender, KeyEventArgs e)
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
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }

}
