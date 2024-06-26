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
using Inventec.Desktop.Common.Message;
using Inventec.Core;
using DevExpress.XtraGrid.Columns;
using EMR.Desktop.Plugins.EmrSignerFlow.entity;
using Inventec.Common.Controls.EditorLoader;
using AutoMapper;
using HIS.Desktop.LocalStorage.BackendData;
using Inventec.Common.Adapter;
using HIS.Desktop.LocalStorage.ConfigApplication;
using DevExpress.XtraGrid.Views.Base;
using HIS.Desktop.Controls.Session;
using Inventec.Common.RichEditor.Base;
using EMR.Desktop.Plugins.EmrSignerFlow.Properties;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using System.Resources;
using Inventec.Desktop.Common.LanguageManager;
using DevExpress.XtraBars;
using EMR.UC.EmrFlow;
using EMR.UC.EmrSign;
using EMR.EFMODEL.DataModels;
using EMR.Filter;
using EMR.UC.EmrFlow.ADO;
using EMR.UC.EmrSign.ADO;

namespace EMR.Desktop.Plugins.EmrSignerFlow
{
    public partial class UC_EmrSignerFlow : UserControl
    {

        #region Declare
        internal Inventec.Desktop.Common.Modules.Module currentModule;
        List<EmrFlowADO> _EmrFlowDeleteADOs { get; set; }
        List<EmrFlowADO> _EmrFlowInsertADOs { get; set; }
        List<V_EMR_FLOW> EmrFlows { get; set; }

        UCEmrFlowProcessor EmrFlowProcessor;
        EmrSignProcessor EmrSignProcessor;
        UserControl ucGridControlFlow;
        UserControl ucGridControlSign;
        int start = 0;
        int limit = 0;
        int rowCount = 0;
        int dataTotal = 0;
        int start1 = 0;
        int limit1 = 0;
        int rowCount1 = 0;
        int dataTotal1 = 0;
        bool isCheckAll;
        internal List<EmrSignADO> _EmrSignADOs { get; set; }
        internal List<EmrFlowADO> _EmrFlowADOs { get; set; }
        List<EMR_SIGNER> listEmrSign;
        List<V_EMR_FLOW> listEmrFlow;
        long ERIdCheckByER = 0;
        long isChoseFlow;
        long isChoseSign;
        long signIdCheckBySign = 0;
        List<EMR_SIGNER_FLOW> _EmrSignerFlows { get; set; }
        List<EMR_SIGNER_FLOW> EmrSignerFlow1 { get; set; }
        bool checkRa = false;

        #endregion


        #region Constructor

        public UC_EmrSignerFlow()
        {
            InitializeComponent();
        }

        private void UC_EmrSignerFlow_Load(object sender, EventArgs e)
        {
            try
            {
                SetCaptionByLanguageKey();
                WaitingManager.Show();
                txtSearchER.Focus();
                txtSearchER.SelectAll();
                LoadDataToCombo();
                InitUcgrid1();
                InitUcgrid2();
                FillDataToGrid1(this);
                FillDataToGrid2(this);
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
                Resources.ResourceLanguageManager.LanguageResource = new ResourceManager("EMR.Desktop.Plugins.EmrSignerFlow.Resources.Lang", typeof(EMR.Desktop.Plugins.EmrSignerFlow.UC_EmrSignerFlow).Assembly);

                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.layoutControl1.Text = Inventec.Common.Resource.Get.Value("UC_EmrSignerFlow.layoutControl1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboChooseBy.Properties.NullText = Inventec.Common.Resource.Get.Value("UC_EmrSignerFlow.cboChooseBy.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnSave.Text = Inventec.Common.Resource.Get.Value("UC_EmrSignerFlow.btnSave.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl3.Text = Inventec.Common.Resource.Get.Value("UC_EmrSignerFlow.layoutControl3.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnSearchA.Text = Inventec.Common.Resource.Get.Value("UC_EmrSignerFlow.btnSearchA.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.txtSearchA.Properties.NullValuePrompt = Inventec.Common.Resource.Get.Value("UC_EmrSignerFlow.txtSearchA.Properties.NullValuePrompt", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl2.Text = Inventec.Common.Resource.Get.Value("UC_EmrSignerFlow.layoutControl2.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnSeachER.Text = Inventec.Common.Resource.Get.Value("UC_EmrSignerFlow.btnSeachER.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.txtSearchER.Properties.NullValuePrompt = Inventec.Common.Resource.Get.Value("UC_EmrSignerFlow.txtSearchER.Properties.NullValuePrompt", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem12.Text = Inventec.Common.Resource.Get.Value("UC_EmrSignerFlow.layoutControlItem12.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }


        #endregion


        #region Load column

        private void InitUcgrid1()
        {
            try
            {
                EmrFlowProcessor = new UCEmrFlowProcessor();
                EmrFlowInitADO ado = new EmrFlowInitADO();
                ado.ListEmrFlowColumn = new List<EmrFlowColumn>();
                ado.btn_Radio_Enable_Click = btn_Radio_Enable_Click;
                // ado.gridViewRoom_MouseDownRoom = RoomGrid_CellValueChanged;
                ado.gridViewEmrFlow_MouseDownEmrFlow = Room_MouseDown;
                object image = Properties.Resources.ResourceManager.GetObject("check1");

                EmrFlowColumn colRadio1 = new EmrFlowColumn("   ", "radio1", 30, true);
                colRadio1.VisibleIndex = 0;
                colRadio1.Visible = false;
                colRadio1.UnboundColumnType = DevExpress.Data.UnboundColumnType.Object;
                ado.ListEmrFlowColumn.Add(colRadio1);

                EmrFlowColumn colCheck1 = new EmrFlowColumn("   ", "check1", 30, true);
                colCheck1.VisibleIndex = 1;
                colCheck1.Visible = false;
                colCheck1.image = imageCollection.Images[0];
                colCheck1.ToolTip = "Chọn tất cả";
                colCheck1.UnboundColumnType = DevExpress.Data.UnboundColumnType.Object;
                ado.ListEmrFlowColumn.Add(colCheck1);

                EmrFlowColumn colMaTheoDoi = new EmrFlowColumn("Mã", "FLOW_CODE", 120, false);
                colMaTheoDoi.VisibleIndex = 2;
                ado.ListEmrFlowColumn.Add(colMaTheoDoi);

                EmrFlowColumn colTenTheoDoi = new EmrFlowColumn("Tên", "FLOW_NAME", 120, false);
                colTenTheoDoi.VisibleIndex = 3;
                ado.ListEmrFlowColumn.Add(colTenTheoDoi);

                this.ucGridControlFlow = (UserControl)EmrFlowProcessor.Run(ado);
                if (ucGridControlFlow != null)
                {
                    this.panelControl1.Controls.Add(this.ucGridControlFlow);
                    this.ucGridControlFlow.Dock = DockStyle.Fill;
                }
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void InitUcgrid2()
        {
            try
            {
                EmrSignProcessor = new EmrSignProcessor();
                EmrSignInitADO ado = new EmrSignInitADO();
                ado.ListEmrSignColumn = new List<UC.EmrSign.EmrSignColumn>();
                ado.btn_Radio_Enable_Click1 = btn_Radio_Enable_Click1;
                ado.EmrSignGrid_MouseDown = EmrSign_MouseDown;
                object image = Properties.Resources.ResourceManager.GetObject("check1");

                EmrSignColumn colRadio2 = new EmrSignColumn("   ", "radio2", 30, true);
                colRadio2.VisibleIndex = 0;
                colRadio2.Visible = false;
                colRadio2.UnboundColumnType = DevExpress.Data.UnboundColumnType.Object;
                ado.ListEmrSignColumn.Add(colRadio2);

                EmrSignColumn colCheck2 = new EmrSignColumn("   ", "check2", 30, true);
                colCheck2.VisibleIndex = 1;
                colCheck2.Visible = false;
                colCheck2.image = imageCollection.Images[0];
                //colCheck2.image = StringAlignment.Center;
                colCheck2.UnboundColumnType = DevExpress.Data.UnboundColumnType.Object;
                ado.ListEmrSignColumn.Add(colCheck2);

                EmrSignColumn colMaNguoiKy = new EmrSignColumn("Mã người ký", "LOGINNAME", 120, false);
                colMaNguoiKy.VisibleIndex = 2;
                ado.ListEmrSignColumn.Add(colMaNguoiKy);

                EmrSignColumn colTenNguoiKy = new EmrSignColumn("Tên người ký", "USERNAME", 120, false);
                colTenNguoiKy.VisibleIndex = 3;
                ado.ListEmrSignColumn.Add(colTenNguoiKy);

                EmrSignColumn colChucDanh = new EmrSignColumn("Chức danh", "TITLE", 120, false);
                colChucDanh.VisibleIndex = 4;
                ado.ListEmrSignColumn.Add(colChucDanh);

                EmrSignColumn colKhoa = new EmrSignColumn("Khoa", "DEPARTMENT_NAME", 120, false);
                colKhoa.VisibleIndex = 5;
                ado.ListEmrSignColumn.Add(colKhoa);

                this.ucGridControlSign = (UserControl)EmrSignProcessor.Run(ado);
                if (ucGridControlSign != null)
                {
                    this.panelControl2.Controls.Add(this.ucGridControlSign);
                    this.ucGridControlSign.Dock = DockStyle.Fill;
                }
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        #endregion


        #region Load Combo

        private void LoadDataToCombo()
        {
            try
            {
                LoadComboStatus();
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadComboStatus()
        {
            try
            {
                List<Status> status = new List<Status>();
                status.Add(new Status(1, "Theo dõi"));
                status.Add(new Status(2, "Người ký"));

                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("statusName", "", 300, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("statusName", "id", columnInfos, false, 350);
                ControlEditorLoader.Load(cboChooseBy, status, controlEditorADO);
                cboChooseBy.EditValue = status[0].id;
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        #endregion


        #region Method


        private void FillDataToGrid2(UC_EmrSignerFlow uCEmrSignerFlow)
        {
            try
            {
                int numPageSize;
                if (ucPaging1.pagingGrid != null)
                {
                    numPageSize = ucPaging1.pagingGrid.PageSize;
                }
                else
                {
                    numPageSize = ConfigApplicationWorker.Get<int>("CONFIG_KEY__NUM_PAGESIZE");
                }
                FillDataToGridEmrSign(new CommonParam(0, numPageSize));
                CommonParam param = new CommonParam();
                param.Limit = rowCount1;
                param.Count = dataTotal1;
                ucPaging2.Init(FillDataToGridEmrSign, param, numPageSize);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void FillDataToGridEmrSign(object data)
        {
            try
            {
                WaitingManager.Show();
                listEmrSign = new List<EMR_SIGNER>();
                int start1 = ((CommonParam)data).Start ?? 0;
                int limit1 = ((CommonParam)data).Limit ?? 0;
                CommonParam param = new CommonParam(start1, limit1);
                EmrSignerFilter EmrSignFilter = new EmrSignerFilter();
                EmrSignFilter.ORDER_FIELD = "MODIFY_TIME";
                EmrSignFilter.ORDER_DIRECTION = "DESC";
                EmrSignFilter.KEY_WORD = txtSearchA.Text;

                if ((long)cboChooseBy.EditValue == 2)
                {
                    isChoseSign = (long)cboChooseBy.EditValue;
                }

                var sar = new Inventec.Common.Adapter.BackendAdapter(param).GetRO<List<EMR_SIGNER>>(
                   "api/EmrSigner/Get",
                    HIS.Desktop.ApiConsumer.ApiConsumers.EmrConsumer,
                    EmrSignFilter,
                    param);

                _EmrSignADOs = new List<EmrSignADO>();

                if (sar != null && sar.Data.Count > 0)
                {
                    //List<SAMPLE_ROOM> dataByEmrSign = new List<SAMPLE_ROOM>();
                    listEmrSign = sar.Data;
                    foreach (var item in listEmrSign)
                    {
                        EmrSignADO EmrSignADO = new EmrSignADO(item);
                        if (isChoseSign == 2)
                        {
                            EmrSignADO.isKeyChoose1 = true;
                        }

                        _EmrSignADOs.Add(EmrSignADO);
                    }
                }
                if (_EmrSignerFlows != null && _EmrSignerFlows.Count > 0)
                {
                    foreach (var item in _EmrSignerFlows)
                    {
                        var check = _EmrSignADOs.FirstOrDefault(o => o.ID == item.SIGNER_ID);
                        if (check != null)
                        {
                            check.check2 = true;
                        }

                    }
                }

                _EmrSignADOs = _EmrSignADOs.OrderByDescending(p => p.check2).ToList();
                if (ucGridControlSign != null)
                {
                    EmrSignProcessor.Reload(ucGridControlSign, _EmrSignADOs);
                }
                rowCount1 = (data == null ? 0 : _EmrSignADOs.Count);
                dataTotal1 = (sar.Param == null ? 0 : sar.Param.Count ?? 0);
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void FillDataToGrid1(UC_EmrSignerFlow uCEmrSignerFlow)
        {
            try
            {
                int numPageSize;
                if (ucPaging1.pagingGrid != null)
                {
                    numPageSize = ucPaging1.pagingGrid.PageSize;
                }
                else
                {
                    numPageSize = ConfigApplicationWorker.Get<int>("CONFIG_KEY__NUM_PAGESIZE");
                }
                FillDataToGridRoom(new CommonParam(0, numPageSize));

                CommonParam param = new CommonParam();
                param.Limit = rowCount;
                param.Count = dataTotal;
                ucPaging1.Init(FillDataToGridRoom, param, numPageSize);
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void FillDataToGridRoom(object data)
        {
            try
            {
                WaitingManager.Show();
                listEmrFlow = new List<V_EMR_FLOW>();
                int start = ((CommonParam)data).Start ?? 0;
                int limit = ((CommonParam)data).Limit ?? 0;
                CommonParam param = new CommonParam(start, limit);
                EMR.Filter.EmrFlowViewFilter ERFillter = new EmrFlowViewFilter();
                ERFillter.ORDER_FIELD = "MODIFY_TIME";
                ERFillter.ORDER_DIRECTION = "DESC";
                ERFillter.KEY_WORD = txtSearchER.Text;

                if ((long)cboChooseBy.EditValue == 1)
                {
                    isChoseFlow = (long)cboChooseBy.EditValue;
                }

                var rs = new Inventec.Common.Adapter.BackendAdapter(param).GetRO<List<V_EMR_FLOW>>(
                     "api/EmrFlow/GetView",
                     HIS.Desktop.ApiConsumer.ApiConsumers.EmrConsumer,
                     ERFillter,
                     param);

                _EmrFlowADOs = new List<EmrFlowADO>();

                if (rs != null && rs.Data.Count > 0)
                {
                    //List<V_EMR_FLOW> dataByRoom = new List<V_EMR_FLOW>();
                    listEmrFlow = rs.Data;
                    foreach (var item in listEmrFlow)
                    {
                        EmrFlowADO RoomADO = new EmrFlowADO(item);
                        if (isChoseFlow == 1)
                        {
                            RoomADO.isKeyChoose = true;
                        }
                        _EmrFlowADOs.Add(RoomADO);
                    }
                }

                if (EmrSignerFlow1 != null && EmrSignerFlow1.Count > 0)
                {
                    foreach (var item in EmrSignerFlow1)
                    {
                        var check = _EmrFlowADOs.FirstOrDefault(o => o.ID == item.FLOW_ID);
                        if (check != null)
                        {
                            check.check1 = true;
                        }
                    }
                }

                _EmrFlowADOs = _EmrFlowADOs.OrderByDescending(p => p.check1).ToList();
                if (ucGridControlFlow != null)
                {
                    EmrFlowProcessor.Reload(ucGridControlFlow, _EmrFlowADOs);
                }
                rowCount = (data == null ? 0 : _EmrFlowADOs.Count);
                dataTotal = (rs.Param == null ? 0 : rs.Param.Count ?? 0);
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public void FindShortcut1()
        {
            try
            {
                btnFind1_Click(null, null);
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public void FindShortcut2()
        {
            try
            {
                btnFind2_Click(null, null);
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public void SaveShortcut()
        {
            try
            {
                btnSave.Focus();
                btnSave_Click(null, null);
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        #endregion


        #region Event

        private void btnFind1_Click(object sender, EventArgs e)
        {
            try
            {
                WaitingManager.Show();
                FillDataToGrid1(this);
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnFind2_Click(object sender, EventArgs e)
        {
            try
            {
                WaitingManager.Show();
                FillDataToGrid2(this);
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboChoose_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                checkRa = false;
                isChoseSign = 0;
                isChoseFlow = 0;
                _EmrSignerFlows = null;
                EmrSignerFlow1 = null;
                FillDataToGrid1(this);
                FillDataToGrid2(this);
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                bool resultSuccess = false;
                CommonParam param = new CommonParam();
                WaitingManager.Show();
                if (ucGridControlSign != null && ucGridControlFlow != null)
                {
                    object EmrSign = EmrSignProcessor.GetDataGridView(ucGridControlSign);
                    object ExecuteR = EmrFlowProcessor.GetDataGridView(ucGridControlFlow);
                    if (isChoseFlow == 1)
                    {
                        if (EmrSign is List<EmrSignADO>)
                        {
                            var data = (List<EmrSignADO>)EmrSign;
                            if (data != null && data.Count > 0)
                            {
                                if (checkRa == true)
                                {
                                    //Danh sach cac user duoc check
                                    EMR.Filter.EmrSignerFlowFilter filter = new EmrSignerFlowFilter();
                                    filter.FLOW_ID = ERIdCheckByER;

                                    var EmrSignerFlow = new BackendAdapter(param).Get<List<EMR_SIGNER_FLOW>>(
                                       "api/EmrSignerFlow/Get",
                                       HIS.Desktop.ApiConsumer.ApiConsumers.EmrConsumer,
                                       filter,
                                       param);

                                    List<long> listSaroid = EmrSignerFlow.Select(p => p.SIGNER_ID).ToList();

                                    var dataCheckeds = data.Where(p => p.check2 == true).ToList();

                                    //List xoa

                                    var dataDeletes = data.Where(o => EmrSignerFlow.Select(p => p.SIGNER_ID)
                                        .Contains(o.ID) && o.check2 == false).ToList();

                                    //list them
                                    var dataCreates = dataCheckeds.Where(o => !EmrSignerFlow.Select(p => p.SIGNER_ID)
                                        .Contains(o.ID)).ToList();
                                    if (dataDeletes != null && dataDeletes.Count > 0)
                                    {
                                        List<long> deleteIds = EmrSignerFlow.Where(o => dataDeletes.Select(p => p.ID)
                                            .Contains(o.SIGNER_ID)).Select(o => o.ID).ToList();
                                        bool deleteResult = new BackendAdapter(param).Post<bool>(
                                                  "/api/EmrSignerFlow/DeleteList",
                                                  HIS.Desktop.ApiConsumer.ApiConsumers.EmrConsumer,
                                                  deleteIds,
                                                  param);
                                        if (deleteResult)
                                            resultSuccess = true;

                                    }

                                    if (dataCreates != null && dataCreates.Count > 0)
                                    {
                                        List<EMR_SIGNER_FLOW> EmrSignerFlowCreates = new List<EMR_SIGNER_FLOW>();
                                        foreach (var item in dataCreates)
                                        {
                                            EMR_SIGNER_FLOW EmrSignerFlowCreate = new EMR_SIGNER_FLOW();
                                            EmrSignerFlowCreate.SIGNER_ID = item.ID;
                                            EmrSignerFlowCreate.FLOW_ID = ERIdCheckByER;
                                            EmrSignerFlowCreates.Add(EmrSignerFlowCreate);
                                        }

                                        var createResult = new BackendAdapter(param).Post<List<EMR_SIGNER_FLOW>>(
                                                   "/api/EmrSignerFlow/CreateList",
                                                   HIS.Desktop.ApiConsumer.ApiConsumers.EmrConsumer,
                                                   EmrSignerFlowCreates,
                                                   param);
                                        if (createResult != null && createResult.Count > 0)
                                            resultSuccess = true;
                                    }
                                    WaitingManager.Hide();
                                    #region Show message
                                    MessageManager.ShowAlert(this.ParentForm, param, resultSuccess);
                                    #endregion

                                    #region Process has exception
                                    SessionManager.ProcessTokenLost(param);
                                    #endregion

                                    data = data.OrderByDescending(p => p.check2).ToList();
                                    if (ucGridControlSign != null)
                                    {
                                        EmrSignProcessor.Reload(ucGridControlSign, data);
                                    }
                                    WaitingManager.Hide();
                                }
                                else
                                {
                                    DevExpress.XtraEditors.XtraMessageBox.Show("Chưa chọn phòng chỉ định");
                                    WaitingManager.Hide();
                                }
                            }
                        }
                    }
                    if (isChoseSign == 2)
                    {
                        if (ExecuteR is List<EmrFlowADO>)
                        {
                            var data = (List<EmrFlowADO>)ExecuteR;

                            if (data != null && data.Count > 0)
                            {
                                if (checkRa == true)
                                {
                                    //Danh sach cac user duoc check
                                    EMR.Filter.EmrSignerFlowFilter filter = new EmrSignerFlowFilter();
                                    filter.SIGNER_ID = signIdCheckBySign;
                                    var erUser = new BackendAdapter(param).Get<List<EMR_SIGNER_FLOW>>(
                                       "api/EmrSignerFlow/Get",
                                       HIS.Desktop.ApiConsumer.ApiConsumers.EmrConsumer,
                                       filter,
                                       param);

                                    var listERID = erUser.Select(p => p.FLOW_ID).ToList();

                                    var dataChecked = data.Where(p => p.check1 == true).ToList();

                                    //List xoa

                                    var dataDelete = data.Where(o => erUser.Select(p => p.FLOW_ID)
                                        .Contains(o.ID) && o.check1 == false).ToList();

                                    //list them
                                    var dataCreate = dataChecked.Where(o => !erUser.Select(p => p.FLOW_ID)
                                        .Contains(o.ID)).ToList();
                                    if (dataDelete != null && dataDelete.Count > 0)
                                    {
                                        List<long> deleteId = erUser.Where(o => dataDelete.Select(p => p.ID)

                                            .Contains(o.FLOW_ID)).Select(o => o.ID).ToList();
                                        bool deleteResult = new BackendAdapter(param).Post<bool>(
                                                  "/api/EmrSignerFlow/DeleteList",
                                                  HIS.Desktop.ApiConsumer.ApiConsumers.EmrConsumer,
                                                  deleteId,
                                                  param);
                                        if (deleteResult)
                                            resultSuccess = true;

                                    }

                                    if (dataCreate != null && dataCreate.Count > 0)
                                    {
                                        List<EMR_SIGNER_FLOW> erUserCreate = new List<EMR_SIGNER_FLOW>();
                                        foreach (var item in dataCreate)
                                        {
                                            EMR_SIGNER_FLOW erUserID = new EMR_SIGNER_FLOW();
                                            erUserID.FLOW_ID = item.ID;
                                            erUserID.SIGNER_ID = signIdCheckBySign;
                                            erUserCreate.Add(erUserID);
                                        }

                                        var createResult = new BackendAdapter(param).Post<List<EMR_SIGNER_FLOW>>(
                                                   "/api/EmrSignerFlow/CreateList",
                                                   HIS.Desktop.ApiConsumer.ApiConsumers.EmrConsumer,
                                                   erUserCreate,
                                                   param);
                                        if (createResult != null && createResult.Count > 0)
                                            resultSuccess = true;
                                    }
                                    WaitingManager.Hide();
                                    #region Show message
                                    MessageManager.ShowAlert(this.ParentForm, param, resultSuccess);
                                    #endregion

                                    #region Process has exception
                                    SessionManager.ProcessTokenLost(param);
                                    #endregion

                                    data = data.OrderByDescending(p => p.check1).ToList();
                                    if (ucGridControlFlow != null)
                                    {
                                        EmrFlowProcessor.Reload(ucGridControlFlow, data);
                                    }
                                    WaitingManager.Hide();
                                }
                                else
                                {
                                    DevExpress.XtraEditors.XtraMessageBox.Show("Chưa chọn phòng lấy mẫu");
                                    WaitingManager.Hide();
                                }

                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void RoomGrid_CellValueChanged(EmrFlowADO data, CellValueChangedEventArgs e)
        {
            try
            {
                //if (data != null)
                //{
                //    if (data.check1 == true)
                //    {

                //        listRoomInsertADO.Add(data);
                //    }
                //    else
                //    {

                //        listRoomDeleteADO.Add(data);
                //    }
                //}
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void Room_MouseDown(object sender, MouseEventArgs e)
        {
            try
            {
                if (isChoseFlow == 1)
                {
                    return;
                }

                WaitingManager.Show();
                if ((Control.ModifierKeys & Keys.Control) != Keys.Control)
                {
                    GridView view = sender as GridView;
                    GridViewInfo viewInfo = view.GetViewInfo() as GridViewInfo;
                    GridHitInfo hi = view.CalcHitInfo(e.Location);

                    if (hi.HitTest == GridHitTest.Column)
                    {
                        if (hi.Column.FieldName == "check1")
                        {
                            var lstCheckAll = _EmrFlowADOs;
                            List<EmrFlowADO> lstChecks = new List<EmrFlowADO>();

                            if (lstCheckAll != null && lstCheckAll.Count > 0)
                            {
                                if (isCheckAll)
                                {
                                    foreach (var item in lstCheckAll)
                                    {
                                        if (item.ID != null)
                                        {
                                            item.check1 = true;
                                            lstChecks.Add(item);
                                        }
                                        else
                                        {
                                            lstChecks.Add(item);
                                        }
                                    }
                                    isCheckAll = false;
                                    hi.Column.Image = imageCollection.Images[1];
                                }
                                else
                                {
                                    foreach (var item in lstCheckAll)
                                    {
                                        if (item.ID != null)
                                        {
                                            item.check1 = false;
                                            lstChecks.Add(item);
                                        }
                                        else
                                        {
                                            lstChecks.Add(item);
                                        }
                                    }
                                    isCheckAll = true;
                                    hi.Column.Image = imageCollection.Images[0];
                                }

                                EmrFlowProcessor.Reload(ucGridControlFlow, lstChecks);
                                //??

                            }
                        }
                    }
                }
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void EmrSign_MouseDown(object sender, MouseEventArgs e)
        {
            try
            {
                if (isChoseSign == 2)
                {
                    return;
                }
                WaitingManager.Show();
                if ((Control.ModifierKeys & Keys.Control) != Keys.Control)
                {
                    GridView view = sender as GridView;
                    GridViewInfo viewInfo = view.GetViewInfo() as GridViewInfo;
                    GridHitInfo hi = view.CalcHitInfo(e.Location);

                    if (hi.HitTest == GridHitTest.Column)
                    {
                        if (hi.Column.FieldName == "check2")
                        {
                            var lstCheckAll = _EmrSignADOs;
                            List<EmrSignADO> lstChecks = new List<EmrSignADO>();

                            if (lstCheckAll != null && lstCheckAll.Count > 0)
                            {
                                if (isCheckAll)
                                {
                                    foreach (var item in lstCheckAll)
                                    {
                                        if (item.ID != null)
                                        {
                                            item.check2 = true;
                                            lstChecks.Add(item);
                                        }
                                        else
                                        {
                                            lstChecks.Add(item);
                                        }
                                    }
                                    isCheckAll = false;
                                    hi.Column.Image = imageCollection.Images[1];
                                }
                                else
                                {
                                    foreach (var item in lstCheckAll)
                                    {
                                        if (item.ID != null)
                                        {
                                            item.check2 = false;
                                            lstChecks.Add(item);
                                        }
                                        else
                                        {
                                            lstChecks.Add(item);
                                        }
                                    }
                                    isCheckAll = true;
                                    hi.Column.Image = imageCollection.Images[0];
                                }

                                //ReloadData
                                EmrSignProcessor.Reload(ucGridControlSign, lstChecks);
                                //??

                            }
                        }
                    }
                }
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btn_Radio_Enable_Click(V_EMR_FLOW data)
        {
            try
            {
                WaitingManager.Show();
                CommonParam param = new CommonParam();
                EMR.Filter.EmrSignerFlowFilter filter = new EmrSignerFlowFilter();
                filter.FLOW_ID = data.ID;
                ERIdCheckByER = data.ID;
                _EmrSignerFlows = new BackendAdapter(param).Get<List<EMR_SIGNER_FLOW>>(
                                "api/EmrSignerFlow/Get",
                                HIS.Desktop.ApiConsumer.ApiConsumers.EmrConsumer,
                                filter,
                                param);
                List<EmrSignADO> dataNew = new List<EmrSignADO>();
                dataNew = (from r in listEmrSign select new EmrSignADO(r)).ToList();
                if (_EmrSignerFlows != null && _EmrSignerFlows.Count > 0)
                {
                    foreach (var item in _EmrSignerFlows)
                    {
                        var check = dataNew.FirstOrDefault(o => o.ID == item.SIGNER_ID);
                        if (check != null)
                        {
                            check.check2 = true;

                        }

                    }
                }

                dataNew = dataNew.OrderByDescending(p => p.check2).ToList();
                if (ucGridControlSign != null)
                {
                    EmrSignProcessor.Reload(ucGridControlSign, dataNew);
                }
                WaitingManager.Hide();
                checkRa = true;
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btn_Radio_Enable_Click1(EMR_SIGNER data)
        {
            try
            {
                WaitingManager.Show();
                CommonParam param = new CommonParam();
                EMR.Filter.EmrSignerFlowFilter filter = new EmrSignerFlowFilter();
                filter.SIGNER_ID = data.ID;
                signIdCheckBySign = data.ID;
                EmrSignerFlow1 = new BackendAdapter(param).Get<List<EMR_SIGNER_FLOW>>(
                                "api/EmrSignerFlow/Get",
                                HIS.Desktop.ApiConsumer.ApiConsumers.EmrConsumer,
                                filter,
                                param);
                List<EmrFlowADO> dataNew = new List<EmrFlowADO>();
                dataNew = (from r in listEmrFlow select new EmrFlowADO(r)).ToList();
                if (EmrSignerFlow1 != null && EmrSignerFlow1.Count > 0)
                {

                    foreach (var itemUsername in EmrSignerFlow1)
                    {
                        var check = dataNew.FirstOrDefault(o => o.ID == itemUsername.FLOW_ID);
                        if (check != null)
                        {
                            check.check1 = true;
                        }
                    }

                    dataNew = dataNew.OrderByDescending(p => p.check1).ToList();
                    if (ucGridControlFlow != null)
                    {
                        EmrFlowProcessor.Reload(ucGridControlFlow, dataNew);
                    }
                }
                else
                {
                    FillDataToGrid1(this);
                }
                WaitingManager.Hide();
                checkRa = true;
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtSearchER_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                WaitingManager.Show();
                if (e.KeyCode == Keys.Enter)
                {
                    FillDataToGrid1(this);
                }
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtSearchA_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                WaitingManager.Show();
                if (e.KeyCode == Keys.Enter)
                {
                    FillDataToGrid2(this);
                }
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }


        #endregion


    }

}
