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
using MOS.EFMODEL.DataModels;
using Inventec.Desktop.Common.Message;
using Inventec.Core;
using MOS.Filter;
using DevExpress.XtraGrid.Columns;
using HIS.UC.ExecuteRole;
using HIS.UC.ExecuteRole.ADO;
using HIS.Desktop.Plugins.Remuneration.entity;
using Inventec.Common.Controls.EditorLoader;
using AutoMapper;
using HIS.Desktop.LocalStorage.BackendData;
using Inventec.Common.Adapter;
using HIS.Desktop.LocalStorage.ConfigApplication;
using DevExpress.XtraGrid.Views.Base;
using HIS.Desktop.Controls.Session;
using Inventec.Common.RichEditor.Base;
using HIS.Desktop.Plugins.Remuneration.Properties;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using System.Resources;
using Inventec.Desktop.Common.LanguageManager;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.ViewInfo;
using System.Dynamic;
using DevExpress.XtraGrid;

namespace HIS.Desktop.Plugins.Remuneration
{
    public partial class UC_Remuneration : HIS.Desktop.Utility.UserControlBase
    {
        #region Declare
        List<ServiceADO> listCheckServiceAdos = new List<ServiceADO>();
        List<ExecuteRoleADO> listCheckERAdos = new List<ExecuteRoleADO>();
        ServiceADO checkServiceAdo;
        dynamic car = new ExpandoObject();
        ExecuteRoleADO checkERAdo;
        List<ExecuteRoleADO> listExecuteRoleDeleteADO { get; set; }
        List<ExecuteRoleADO> listExecuteRoleInsertADO { get; set; }
        List<HIS_EXECUTE_ROLE> executeRole { get; set; }
        ExecuteRoleADO erADORa;
        internal Inventec.Desktop.Common.Modules.Module currentModule;
        ExecuteRoleProcessor ERProcessor;
        UserControl ucGridControlER;
        private int positionHandleControlEditor = -1;
        int start = 0;
        int limit = 0;
        int rowCount = 0;
        int dataTotal = 0;
        int start1 = 0;
        int limit1 = 0;
        int rowCount1 = 0;
        int dataTotal1 = 0;
        bool isCheckAll;
        bool checkRemu = false;
        internal List<ServiceADO> lstServiceADOs { get; set; }
        internal List<HIS.UC.ExecuteRole.ExecuteRoleADO> lstExecuteRoleADOs { get; set; }
        ServiceADO serviceADORa;
        List<V_HIS_SERVICE> listService;
        List<HIS_EXECUTE_ROLE> listExecuteRole;
        long ERIdCheckByER = 0;
        long ServiceIdCheckByService = 0;
        long isChoseER;
        long isChoseService;
        bool checkRa = false;
        List<HIS_REMUNERATION> remuneration { get; set; }
        List<HIS_REMUNERATION> remuneration1 { get; set; }

        #endregion

        #region Contructor

        public UC_Remuneration(Inventec.Desktop.Common.Modules.Module _moduleData)
            : base(_moduleData)
        {
            InitializeComponent();
        }

        private void UC_Remuneration_Load(object sender, EventArgs e)
        {
            try
            {
                WaitingManager.Show();
                TEst();
                SetCaptionByLanguageKey();
                txtSearchExecuteRole.Focus();
                txtSearchExecuteRole.SelectAll();
                LoadDataToCombo();
                InitUcgrid1();
                FillDataToGrid1(this);
                gridViewService.Columns.Where(o => o.FieldName == "check2").FirstOrDefault().Image = imageCollection.Images[0];
                gridViewService.Columns.Where(o => o.FieldName == "check2").FirstOrDefault().ImageAlignment = StringAlignment.Center;
                FillDataToGrid2(this);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void TEst()
        {
            try
            {

                car.Type = "Huyndai";
                car.Name = "X100";
                car.Price = "1000";
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        #endregion

        #region Load Column

        private void InitUcgrid1()
        {
            try
            {
                ERProcessor = new ExecuteRoleProcessor();
                ExecuteRoleInitADO ado = new ExecuteRoleInitADO();
                ado.ListExecuteRoleColumn = new List<ExecuteRoleColumn>();
                ado.btn_Radio_Enable_Click = btn_Radio_Enable_Click;
                //ado.ExecuteRoleGrid_CellValueChanged = ExecuteRoleGrid_CellValueChanged;
                ado.ExecuteRoleGrid_MouseDown = ExecuteRole_MouseDown;
                ado.checkEnable_CheckedChanged = Check_CheckedChanged;
                ado.spin_EditValueChanged = Spin_EditValueChanged;



                ExecuteRoleColumn colRadio1 = new ExecuteRoleColumn("   ", "radio1", 30, true);
                colRadio1.VisibleIndex = 0;
                colRadio1.Visible = false;
                colRadio1.UnboundColumnType = DevExpress.Data.UnboundColumnType.Object;
                ado.ListExecuteRoleColumn.Add(colRadio1);

                ExecuteRoleColumn colCheck1 = new ExecuteRoleColumn("   ", "check1", 30, true);
                colCheck1.VisibleIndex = 1;
                colCheck1.Visible = false;
                colCheck1.image = imageCollection.Images[0];
                colCheck1.ToolTip = "Chọn tất cả";
                colCheck1.imageAlignment = StringAlignment.Center;
                colCheck1.UnboundColumnType = DevExpress.Data.UnboundColumnType.Object;
                ado.ListExecuteRoleColumn.Add(colCheck1);

                ExecuteRoleColumn colMaVaiTro = new ExecuteRoleColumn("Mã vai trò", "EXECUTE_ROLE_CODE", 120, false);
                colMaVaiTro.VisibleIndex = 2;
                ado.ListExecuteRoleColumn.Add(colMaVaiTro);

                ExecuteRoleColumn colTenVaiTro = new ExecuteRoleColumn("Tên vai trò", "EXECUTE_ROLE_NAME", 120, false);
                colTenVaiTro.VisibleIndex = 3;
                ado.ListExecuteRoleColumn.Add(colTenVaiTro);

                ExecuteRoleColumn colPrice = new ExecuteRoleColumn("Giá", "PRICE", 120, true);
                colPrice.VisibleIndex = 4;
                ado.ListExecuteRoleColumn.Add(colPrice);

                this.ucGridControlER = (UserControl)ERProcessor.Run(ado);
                if (ucGridControlER != null)
                {
                    this.panelControl1.Controls.Add(this.ucGridControlER);
                    this.ucGridControlER.Dock = DockStyle.Fill;
                }
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void FillDataToGrid1(UC_Remuneration uCExecuteRoleUser)
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
                FillDataToGridExecuteRole(new CommonParam(0, numPageSize));

                CommonParam param = new CommonParam();
                param.Limit = rowCount;
                param.Count = dataTotal;
                ucPaging1.Init(FillDataToGridExecuteRole, param, numPageSize, (GridControl)ERProcessor.GetGridControl(ucGridControlER));
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void FillDataToGridExecuteRole(object data)
        {
            try
            {
                WaitingManager.Show();
                //var listERSource = (List<ExecuteRoleADO>)ERProcessor.GetDataGridView(ucGridControlER);
                listExecuteRole = new List<HIS_EXECUTE_ROLE>();
                int start = ((CommonParam)data).Start ?? 0;
                int limit = ((CommonParam)data).Limit ?? 0;
                CommonParam param = new CommonParam(start, limit);
                MOS.Filter.HisExecuteRoleFilter ERFillter = new HisExecuteRoleFilter();
                ERFillter.ORDER_FIELD = "MODIFY_TIME";
                ERFillter.ORDER_DIRECTION = "DESC";
                ERFillter.KEY_WORD = txtSearchExecuteRole.Text;

                if ((long)cboChooseBy.EditValue == 1)
                {
                    isChoseER = (long)cboChooseBy.EditValue;
                }

                var rs = new Inventec.Common.Adapter.BackendAdapter(param).GetRO<List<MOS.EFMODEL.DataModels.HIS_EXECUTE_ROLE>>(
                     "api/HisExecuteRole/Get",
                     HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer,
                     ERFillter,
                     param);

                lstExecuteRoleADOs = new List<HIS.UC.ExecuteRole.ExecuteRoleADO>();

                if (rs != null && rs.Data.Count > 0)
                {
                    //List<V_HIS_ROOM> dataByRoom = new List<V_HIS_ROOM>();
                    listExecuteRole = rs.Data;
                    foreach (var item in listExecuteRole)
                    {
                        HIS.UC.ExecuteRole.ExecuteRoleADO ExecuteRoleADO = new HIS.UC.ExecuteRole.ExecuteRoleADO(item);
                        if (isChoseER == 1)
                        {
                            ExecuteRoleADO.isKeyChoose = true;
                        }
                        lstExecuteRoleADOs.Add(ExecuteRoleADO);
                    }
                }

                //FILTER TEST CẦN SỬA LẠI 

                if (remuneration1 != null && remuneration1.Count > 0)
                {
                    foreach (var item in remuneration1)
                    {
                        var check = lstExecuteRoleADOs.FirstOrDefault(o => o.ID == item.EXECUTE_ROLE_ID);
                        if (check != null)
                        {
                            check.check1 = true;
                            check.PRICE = item.PRICE;
                        }
                    }
                }


                lstExecuteRoleADOs = lstExecuteRoleADOs.OrderByDescending(p => p.check1).ToList();

                if (erADORa != null && isChoseER == 1)
                {
                    var erADO = lstExecuteRoleADOs.Where(o => o.ID == erADORa.ID).FirstOrDefault();
                    if (erADO != null)
                    {
                        erADO.radio1 = true;
                        lstExecuteRoleADOs = lstExecuteRoleADOs.OrderByDescending(p => p.radio1).ToList();
                    }
                }

                if (listCheckERAdos != null && listCheckERAdos.Count > 0)
                {
                    foreach (var item in listCheckERAdos)
                    {
                        var kq = lstExecuteRoleADOs.FirstOrDefault(o => o.ID == item.ID);
                        if (kq != null)
                        {
                            lstExecuteRoleADOs.FirstOrDefault(o => o.ID == item.ID).check1 = item.check1;
                            lstExecuteRoleADOs.FirstOrDefault(o => o.ID == item.ID).PRICE = item.PRICE;
                        }
                    }
                }


                if (ucGridControlER != null)
                {
                    ERProcessor.Reload(ucGridControlER, lstExecuteRoleADOs);
                }
                rowCount = (data == null ? 0 : lstExecuteRoleADOs.Count);
                dataTotal = (rs.Param == null ? 0 : rs.Param.Count ?? 0);
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void FillDataToGrid2(UC_Remuneration uCExecuteRoleUser)
        {
            try
            {
                int numPageSize;
                if (ucPaging2.pagingGrid != null)
                {
                    numPageSize = ucPaging2.pagingGrid.PageSize;
                }
                else
                {
                    numPageSize = ConfigApplicationWorker.Get<int>("CONFIG_KEY__NUM_PAGESIZE");
                }
                FillDataToGridService(new CommonParam(0, numPageSize));
                CommonParam param = new CommonParam();
                param.Limit = rowCount1;
                param.Count = dataTotal1;
                ucPaging2.Init(FillDataToGridService, param, numPageSize, gridControlService);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void FillDataToGridService(object data)
        {
            try
            {
                WaitingManager.Show();
                //var listServiceSource = (List<ServiceADO>)gridControlService.DataSource;

                listService = new List<V_HIS_SERVICE>();
                int start1 = ((CommonParam)data).Start ?? 0;
                int limit1 = ((CommonParam)data).Limit ?? 0;
                CommonParam param = new CommonParam(start1, limit1);
                HisServiceViewFilter serviceFilter = new HisServiceViewFilter();
                serviceFilter.ORDER_FIELD = "MODIFY_TIME";
                serviceFilter.ORDER_DIRECTION = "DESC";
                serviceFilter.KEY_WORD = txtSearchService.Text;

                if ((long)cboChooseBy.EditValue == 2)
                {
                    isChoseService = (long)cboChooseBy.EditValue;
                }

                var service = new Inventec.Common.Adapter.BackendAdapter(param).GetRO<List<V_HIS_SERVICE>>(
                    "api/HisService/GetView",
                    HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer,
                    serviceFilter,
                    param);
                //var checkService = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<V_HIS_SERVICE>();

                lstServiceADOs = new List<ServiceADO>();

                if (service != null && service.Data.Count > 0)
                {
                    //List<ACS_USER> dataByAccount = new List<ACS_USER>();
                    listService = service.Data;
                    foreach (var item in listService)
                    {
                        ServiceADO serviceADO = new ServiceADO(item);
                        if (isChoseService == 2)
                        {
                            serviceADO.isKeyChoose1 = true;
                        }

                        lstServiceADOs.Add(serviceADO);
                    }
                }

                //FILTER TEST CẦN SỬA LẠI 

                if (remuneration != null && remuneration.Count > 0)
                {
                    foreach (var item in remuneration)
                    {
                        var check = lstServiceADOs.FirstOrDefault(o => o.ID == item.SERVICE_ID);
                        if (check != null)
                        {
                            check.check2 = true;
                            check.PRICE = item.PRICE;
                        }
                    }
                }

                lstServiceADOs = lstServiceADOs.OrderByDescending(p => p.check2).ToList();


                if (serviceADORa != null && isChoseService == 2)
                {
                    var svADO = lstServiceADOs.Where(o => o.ID == serviceADORa.ID).FirstOrDefault();
                    if (svADO != null)
                    {
                        svADO.radio2 = true;
                        lstServiceADOs = lstServiceADOs.OrderByDescending(p => p.radio2).ToList();
                    }
                }

                if (listCheckServiceAdos != null && listCheckServiceAdos.Count > 0)
                {
                    foreach (var item in listCheckServiceAdos)
                    {
                        var kq = lstServiceADOs.FirstOrDefault(o => o.ID == item.ID);
                        if (kq != null)
                        {
                            lstServiceADOs.FirstOrDefault(o => o.ID == item.ID).check2 = item.check2;
                            lstServiceADOs.FirstOrDefault(o => o.ID == item.ID).PRICE = item.PRICE;
                        }
                    }
                }

                gridControlService.BeginUpdate();
                gridControlService.DataSource = lstServiceADOs;
                gridControlService.EndUpdate();
                rowCount1 = (data == null ? 0 : lstServiceADOs.Count);
                dataTotal1 = (service.Param == null ? 0 : service.Param.Count ?? 0);
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
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
                status.Add(new Status(1, "Vai trò thực hiện"));
                status.Add(new Status(2, "Dịch vụ"));

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

        private void SetCaptionByLanguageKey()
        {
            try
            {
                ////Khoi tao doi tuong resource
                Resources.ResourceLanguageManager.LanguageResource = new ResourceManager("HIS.Desktop.Plugins.Remuneration.Resources.Lang", typeof(HIS.Desktop.Plugins.Remuneration.UC_Remuneration).Assembly);

                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.layoutControl1.Text = Inventec.Common.Resource.Get.Value("UC_Remuneration.layoutControl1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnSave.Text = Inventec.Common.Resource.Get.Value("UC_Remuneration.btnSave.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn1.Caption = Inventec.Common.Resource.Get.Value("UC_Remuneration.gridColumn1.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn2.Caption = Inventec.Common.Resource.Get.Value("UC_Remuneration.gridColumn2.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn3.Caption = Inventec.Common.Resource.Get.Value("UC_Remuneration.gridColumn3.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn4.Caption = Inventec.Common.Resource.Get.Value("UC_Remuneration.gridColumn4.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn5.Caption = Inventec.Common.Resource.Get.Value("UC_Remuneration.gridColumn5.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnSearchService.Text = Inventec.Common.Resource.Get.Value("UC_Remuneration.btnSearchService.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.txtSearchService.Properties.NullValuePrompt = Inventec.Common.Resource.Get.Value("UC_Remuneration.txtSearchService.Properties.NullValuePrompt", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnSearchExecuteRole.Text = Inventec.Common.Resource.Get.Value("UC_Remuneration.btnSearchExecuteRole.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.txtSearchExecuteRole.Properties.NullValuePrompt = Inventec.Common.Resource.Get.Value("UC_Remuneration.txtSearchExecuteRole.Properties.NullValuePrompt", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboChooseBy.Properties.NullText = Inventec.Common.Resource.Get.Value("UC_Remuneration.cboChooseBy.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem5.Text = Inventec.Common.Resource.Get.Value("UC_Remuneration.layoutControlItem5.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        #endregion

        #region Key Enter



        #endregion

        #region Event

        private void ExecuteRole_MouseDown(object sender, MouseEventArgs e)
        {
            try
            {
                if (isChoseER == 1)
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
                            var lstCheckAll = lstExecuteRoleADOs;
                            List<HIS.UC.ExecuteRole.ExecuteRoleADO> lstChecks = new List<HIS.UC.ExecuteRole.ExecuteRoleADO>();

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

                                ERProcessor.Reload(ucGridControlER, lstChecks);
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

        private void btn_Radio_Enable_Click(HIS_EXECUTE_ROLE data, ExecuteRoleADO ERado)
        {
            try
            {
                WaitingManager.Show();
                CommonParam param = new CommonParam();

                erADORa = new ExecuteRoleADO();
                erADORa = ERado;
                //FILTER TEST CẦN SỬA LẠI


                HisRemunerationFilter filter = new HisRemunerationFilter();
                filter.EXECUTE_ROLE_ID = data.ID;
                ERIdCheckByER = data.ID;
                remuneration = new BackendAdapter(param).Get<List<HIS_REMUNERATION>>(
                                "api/HisRemuneration/Get",
                                HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer,
                                filter,
                                param);
                List<ServiceADO> dataNew = new List<ServiceADO>();
                dataNew = (from r in listService select new ServiceADO(r)).ToList();
                if (remuneration != null && remuneration.Count > 0)
                {
                    foreach (var item in remuneration)
                    {
                        var check = dataNew.FirstOrDefault(o => o.ID == item.SERVICE_ID);
                        if (check != null)
                        {
                            check.check2 = true;
                            check.PRICE = item.PRICE;
                        }

                    }
                }

                dataNew = dataNew.OrderByDescending(p => p.check2).ToList();
                gridControlService.BeginUpdate();
                gridControlService.DataSource = dataNew;
                gridControlService.EndUpdate();
                WaitingManager.Hide();
                checkRa = true;
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btn_Radio_Enable_Click1(V_HIS_SERVICE data)
        {
            try
            {

                WaitingManager.Show();
                CommonParam param = new CommonParam();

                //FILTER TEST CẦN SỬA LẠI

                HisRemunerationFilter filter = new HisRemunerationFilter();
                filter.SERVICE_ID = data.ID;
                ServiceIdCheckByService = data.ID;
                remuneration1 = new BackendAdapter(param).Get<List<HIS_REMUNERATION>>(
                                "api/HisRemuneration/Get",
                                HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer,
                                filter,
                                param);
                List<HIS.UC.ExecuteRole.ExecuteRoleADO> dataNew = new List<HIS.UC.ExecuteRole.ExecuteRoleADO>();
                dataNew = (from r in listExecuteRole select new HIS.UC.ExecuteRole.ExecuteRoleADO(r)).ToList();
                if (remuneration1 != null && remuneration1.Count > 0)
                {

                    foreach (var item in remuneration1)
                    {
                        var check = dataNew.FirstOrDefault(o => o.ID == item.EXECUTE_ROLE_ID);
                        if (check != null)
                        {
                            check.check1 = true;
                            check.PRICE = item.PRICE;
                        }
                    }

                    dataNew = dataNew.OrderByDescending(p => p.check1).ToList();
                    if (ucGridControlER != null)
                    {
                        ERProcessor.Reload(ucGridControlER, dataNew);
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

        private void txtSearchExecuteRole_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    FillDataToGrid1(this);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnSearchExecuteRole_Click(object sender, EventArgs e)
        {
            try
            {
                FillDataToGrid1(this);
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
                isChoseService = 0;
                isChoseER = 0;
                ServiceIdCheckByService = 0;
                ERIdCheckByER = 0;
                listCheckERAdos = new List<ExecuteRoleADO>();
                listCheckServiceAdos = new List<ServiceADO>();
                erADORa = null;
                serviceADORa = null;
                remuneration = null;
                remuneration1 = null;
                FillDataToGrid1(this);
                FillDataToGrid2(this);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtSearchService_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    FillDataToGrid2(this);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnSearchService_Click(object sender, EventArgs e)
        {
            try
            {
                FillDataToGrid2(this);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewService_CustomRowCellEdit(object sender, CustomRowCellEditEventArgs e)
        {
            GridView View = sender as GridView;
            if (e.RowHandle >= 0)
            {
                var data = (ServiceADO)gridViewService.GetRow(e.RowHandle);
                if (data != null)
                {
                    //if (data.isKeyChoose)
                    //{
                    //    CheckAll1.Enabled = false;
                    //}
                    //else
                    //{
                    //    CheckAll1.Enabled = true;
                    //}
                    if (e.Column.FieldName == "check2")
                    {
                        if (data.isKeyChoose1)
                        {
                            e.RepositoryItem = CheckD;
                        }
                        else
                        {
                            e.RepositoryItem = CheckE;

                        }
                    }
                    if (e.Column.FieldName == "radio2")
                    {
                        if (data.isKeyChoose1)
                        {
                            e.RepositoryItem = RadioE;
                        }
                        else
                        {
                            e.RepositoryItem = RadioD;
                        }

                    }

                    if (e.Column.FieldName == "PRICE")
                    {
                        if (data.isKeyChoose1)
                        {
                            e.RepositoryItem = SpinPriceD;
                        }
                        else
                        {
                            e.RepositoryItem = SpinPriceE;
                        }

                    }
                }
            }
        }

        private void gridViewService_CustomUnboundColumnData(object sender, CustomColumnDataEventArgs e)
        {

        }

        private void gridViewService_MouseDown(object sender, MouseEventArgs e)
        {
            try
            {
                if (isChoseService == 2)
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
                            var lstCheckAll = lstServiceADOs;
                            List<ServiceADO> lstChecks = new List<ServiceADO>();

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
                                gridControlService.BeginUpdate();
                                gridControlService.DataSource = lstChecks;
                                gridControlService.EndUpdate();
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

        private void RadioE_Click(object sender, EventArgs e)
        {
            try
            {
                serviceADORa = new ServiceADO();
                serviceADORa = (ServiceADO)gridViewService.GetFocusedRow();
                var row = (V_HIS_SERVICE)gridViewService.GetFocusedRow();
                foreach (var item in lstServiceADOs)
                {
                    if (item.ID == row.ID)
                    {
                        item.radio2 = true;
                    }
                    else
                    {
                        item.radio2 = false;
                    }
                }

                gridControlService.RefreshDataSource();

                if (row != null)
                {
                    this.btn_Radio_Enable_Click1(row);
                }
                gridViewService.LayoutChanged();
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
                bool validate = true;
                CommonParam param = new CommonParam();
                WaitingManager.Show();

                if (ucGridControlER != null)
                {
                    object controlService = gridControlService.DataSource;
                    object ExecuteR = ERProcessor.GetDataGridView(ucGridControlER);
                    if (isChoseER == 1)
                    {
                        //checkRa = false;
                        if (controlService is List<ServiceADO>)
                        {
                            var data = (List<ServiceADO>)controlService;
                            if (data != null && data.Count > 0)
                            {
                                if (checkRa == true)
                                {
                                    //Danh sach cac user duoc check
                                    HisRemunerationFilter filter = new HisRemunerationFilter();

                                    filter.EXECUTE_ROLE_ID = ERIdCheckByER;

                                    var remunerationGet = new BackendAdapter(param).Get<List<HIS_REMUNERATION>>(
                                      "api/HisRemuneration/Get",
                                       HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer,
                                       filter,
                                       param);

                                    List<long> listServiceID = remunerationGet.Select(p => p.SERVICE_ID).ToList();

                                    var dataCheckeds = data.Where(p => p.check2 == true).ToList();

                                    var dataUpdate = dataCheckeds;
                                    //List xoa

                                    var dataDeletes = data.Where(o => remunerationGet.Select(p => p.SERVICE_ID)
                                        .Contains(o.ID) && o.check2 == false).ToList();

                                    //list them
                                    var dataCreates = dataCheckeds.Where(o => !remunerationGet.Select(p => p.SERVICE_ID)
                                        .Contains(o.ID)).ToList();


                                    foreach (var item in dataCreates)
                                    {
                                        dataUpdate.Remove(item);
                                    }

                                    bool checkDelete = false;
                                    bool checkCreate = false;
                                    bool checkUpdate = false;

                                    //if (dataCheckeds.Count != erUser.Select(p => p.LOGINNAME).Count())
                                    //{

                                    erADORa = new ExecuteRoleADO();
                                    erADORa = ((List<ExecuteRoleADO>)ERProcessor.GetDataGridView(ucGridControlER)).FirstOrDefault(o => o.radio1 == true);

                                    if (dataUpdate != null && dataUpdate.Count > 0)
                                    {
                                        List<HIS_REMUNERATION> Updates = new List<HIS_REMUNERATION>();
                                        foreach (var item in dataUpdate)
                                        {
                                            HIS_REMUNERATION remuID = new HIS_REMUNERATION();
                                            remuID.ID = remunerationGet.Where(o => o.SERVICE_ID == item.ID && o.EXECUTE_ROLE_ID == ERIdCheckByER).FirstOrDefault().ID;
                                            remuID.EXECUTE_ROLE_ID = ERIdCheckByER;
                                            remuID.SERVICE_ID = item.ID;
                                            remuID.PRICE = item.PRICE;
                                            if (item.PRICE < 0)
                                                validate = false;
                                            Updates.Add(remuID);
                                        }
                                        if (validate)
                                        {
                                            var updateResult = new BackendAdapter(param).Post<List<HIS_REMUNERATION>>(
                                                       "/api/HisRemuneration/UpdateList",
                                                       HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer,
                                                       Updates,
                                                       param);
                                            if (updateResult != null && updateResult.Count > 0)
                                            {
                                                resultSuccess = true;
                                                checkUpdate = true;
                                                foreach (var item in updateResult)
                                                {
                                                    remuneration.FirstOrDefault(o => o.ID == item.ID).PRICE = item.PRICE;
                                                }
                                            }
                                        }
                                    }

                                    if (dataDeletes != null && dataDeletes.Count > 0)
                                    {
                                        List<long> deleteIds = remunerationGet.Where(o => dataDeletes.Select(p => p.ID)
                                            .Contains(o.SERVICE_ID)).Select(o => o.ID).ToList();
                                        bool deleteResult = new BackendAdapter(param).Post<bool>(
                                                  "/api/HisRemuneration/DeleteList",
                                                  HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer,
                                                  deleteIds,
                                                  param);
                                        if (deleteResult)
                                        {
                                            resultSuccess = true;
                                            checkDelete = true;
                                            remuneration = remuneration.Where(o => !deleteIds.Contains(o.ID)).ToList();
                                        }
                                    }

                                    if (dataCreates != null && dataCreates.Count > 0)
                                    {
                                        List<HIS_REMUNERATION> remuCreates = new List<HIS_REMUNERATION>();
                                        foreach (var item in dataCreates)
                                        {
                                            HIS_REMUNERATION remuCreate = new HIS_REMUNERATION();
                                            remuCreate.SERVICE_ID = item.ID;
                                            remuCreate.PRICE = item.PRICE;
                                            if (item.PRICE < 0)
                                            {
                                                validate = false;
                                            }
                                            remuCreate.EXECUTE_ROLE_ID = ERIdCheckByER;
                                            remuCreates.Add(remuCreate);
                                        }

                                        if (validate)
                                        {
                                            var createResult = new BackendAdapter(param).Post<List<HIS_REMUNERATION>>(
                                                       "/api/HisRemuneration/CreateList",
                                                       HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer,
                                                       remuCreates,
                                                       param);
                                            if (createResult != null && createResult.Count > 0)
                                            {
                                                resultSuccess = true;
                                                checkCreate = true;
                                                remuneration.AddRange(createResult);
                                            }
                                        }
                                    }
                                    WaitingManager.Hide();
                                    if (validate)
                                    {
                                        if (checkCreate == true || checkDelete == true || checkUpdate == true)
                                        {
                                            #region Show message
                                            MessageManager.Show(this.ParentForm, param, resultSuccess);
                                            #endregion

                                            #region Process has exception
                                            SessionManager.ProcessTokenLost(param);
                                            #endregion
                                            //}
                                            listCheckERAdos = new List<ExecuteRoleADO>();
                                            listCheckServiceAdos = new List<ServiceADO>();
                                            data = data.OrderByDescending(p => p.check2).ToList();
                                            gridControlService.BeginUpdate();
                                            gridControlService.DataSource = data;
                                            gridControlService.EndUpdate();
                                        }
                                    }
                                    else
                                    {
                                        DevExpress.XtraEditors.XtraMessageBox.Show("Giá không được âm", "Thông báo");
                                    }
                                    WaitingManager.Hide();
                                }
                                else
                                {
                                    DevExpress.XtraEditors.XtraMessageBox.Show("Chưa chọn vai trò thực hiện");
                                    WaitingManager.Hide();
                                }

                            }

                        }
                    }
                    if (isChoseService == 2)
                    {
                        //checkRa = false;
                        if (ExecuteR is List<HIS.UC.ExecuteRole.ExecuteRoleADO>)
                        {
                            var data = (List<HIS.UC.ExecuteRole.ExecuteRoleADO>)ExecuteR;

                            if (data != null && data.Count > 0)
                            {
                                if (checkRa == true)
                                {

                                    //Danh sach cac user duoc check
                                    MOS.Filter.HisRemunerationFilter filter = new HisRemunerationFilter();
                                    filter.SERVICE_ID = ServiceIdCheckByService;
                                    var remunerationGet1 = new BackendAdapter(param).Get<List<HIS_REMUNERATION>>(
                                       "api/HisRemuneration/Get",
                                       HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer,
                                       filter,
                                       param);

                                    var listERID = remunerationGet1.Select(p => p.EXECUTE_ROLE_ID).ToList();

                                    var dataChecked = data.Where(p => p.check1 == true).ToList();
                                    var dataUpdate = dataChecked;
                                    //List xoa

                                    var dataDelete = data.Where(o => remunerationGet1.Select(p => p.EXECUTE_ROLE_ID)
                                        .Contains(o.ID) && o.check1 == false).ToList();

                                    //list them
                                    var dataCreate = dataChecked.Where(o => !remunerationGet1.Select(p => p.EXECUTE_ROLE_ID)
                                        .Contains(o.ID)).ToList();
                                    //if (dataChecked.Count != erUser.Select(p => p.EXECUTE_ROLE_ID).Count())
                                    //{
                                    bool checkDelete = false;
                                    bool checkCreate = false;
                                    bool checkUpdate = false;

                                    foreach (var item in dataCreate)
                                    {
                                        dataUpdate.Remove(item);
                                    }

                                    serviceADORa = new ServiceADO();
                                    serviceADORa = ((List<ServiceADO>)gridControlService.DataSource).FirstOrDefault(o => o.radio2 == true);
                                    if (dataUpdate != null && dataUpdate.Count > 0)
                                    {
                                        List<HIS_REMUNERATION> Updates = new List<HIS_REMUNERATION>();
                                        foreach (var item in dataUpdate)
                                        {
                                            HIS_REMUNERATION remuID = new HIS_REMUNERATION();
                                            if (remunerationGet1 != null && remunerationGet1.Count > 0)
                                            {
                                                remuID.ID = remunerationGet1.Where(o => o.SERVICE_ID == ServiceIdCheckByService && o.EXECUTE_ROLE_ID == item.ID).FirstOrDefault().ID;
                                            }
                                            remuID.EXECUTE_ROLE_ID = item.ID;
                                            remuID.SERVICE_ID = ServiceIdCheckByService;
                                            remuID.PRICE = item.PRICE;
                                            if (item.PRICE < 0)
                                                validate = false;
                                            Updates.Add(remuID);
                                        }
                                        if (validate)
                                        {
                                            var updateResult = new BackendAdapter(param).Post<List<HIS_REMUNERATION>>(
                                                       "/api/HisRemuneration/UpdateList",
                                                       HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer,
                                                       Updates,
                                                       param);
                                            if (updateResult != null && updateResult.Count > 0)
                                            {
                                                resultSuccess = true;
                                                checkUpdate = true;
                                                foreach (var item in updateResult)
                                                {
                                                    remuneration1.FirstOrDefault(o => o.ID == item.ID).PRICE = item.PRICE;
                                                }
                                            }
                                        }
                                    }

                                    if (dataDelete != null && dataDelete.Count > 0)
                                    {
                                        List<long> deleteId = remunerationGet1.Where(o => dataDelete.Select(p => p.ID)

                                            .Contains(o.EXECUTE_ROLE_ID)).Select(o => o.ID).ToList();
                                        bool deleteResult = new BackendAdapter(param).Post<bool>(
                                                  "/api/HisRemuneration/DeleteList",
                                                  HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer,
                                                  deleteId,
                                                  param);
                                        if (deleteResult)
                                        {
                                            checkDelete = true;
                                            resultSuccess = true;
                                            remuneration1 = remuneration1.Where(o => !deleteId.Contains(o.ID)).ToList();
                                        }

                                    }

                                    if (dataCreate != null && dataCreate.Count > 0)
                                    {
                                        List<HIS_REMUNERATION> Creates = new List<HIS_REMUNERATION>();
                                        foreach (var item in dataCreate)
                                        {
                                            HIS_REMUNERATION remunerationID = new HIS_REMUNERATION();
                                            remunerationID.EXECUTE_ROLE_ID = item.ID;
                                            remunerationID.SERVICE_ID = ServiceIdCheckByService;
                                            remunerationID.PRICE = item.PRICE;
                                            if (item.PRICE < 0)
                                            {
                                                validate = false;
                                            }
                                            Creates.Add(remunerationID);
                                        }
                                        if (validate)
                                        {
                                            var createResult = new BackendAdapter(param).Post<List<HIS_REMUNERATION>>(
                                                       "/api/HisRemuneration/CreateList",
                                                       HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer,
                                                       Creates,
                                                       param);
                                            if (createResult != null && createResult.Count > 0)
                                            {
                                                checkCreate = true;
                                                resultSuccess = true;
                                                remuneration1.AddRange(createResult);
                                            }
                                        }
                                    }


                                    WaitingManager.Hide();
                                    if (validate)
                                    {
                                        if (checkCreate == true || checkDelete == true || checkUpdate == true)
                                        {
                                            #region Show message
                                            MessageManager.Show(this.ParentForm, param, resultSuccess);
                                            #endregion

                                            #region Process has exception
                                            SessionManager.ProcessTokenLost(param);
                                            #endregion
                                            //}
                                            listCheckERAdos = new List<ExecuteRoleADO>();
                                            listCheckServiceAdos = new List<ServiceADO>();
                                            data = data.OrderByDescending(p => p.check1).ToList();
                                            if (ucGridControlER != null)
                                            {
                                                ERProcessor.Reload(ucGridControlER, data);
                                            }

                                        }
                                    }
                                    else
                                    {
                                        DevExpress.XtraEditors.XtraMessageBox.Show("Giá không được âm", "Thông báo");
                                    }
                                }
                                else
                                {
                                    DevExpress.XtraEditors.XtraMessageBox.Show("Chưa chọn dịch vụ");
                                }
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

        #endregion

        #region Shotcut

        public void FindShortcut1()
        {
            try
            {
                FillDataToGrid1(this);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public void FindShortcut2()
        {
            try
            {
                FillDataToGrid2(this);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
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
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        #endregion

        private void dxValidationProvider1_ValidationFailed(object sender, DevExpress.XtraEditors.DXErrorProvider.ValidationFailedEventArgs e)
        {
            try
            {
                BaseEdit edit = e.InvalidControl as BaseEdit;
                if (edit == null)
                    return;

                BaseEditViewInfo viewInfo = edit.GetViewInfo() as BaseEditViewInfo;
                if (viewInfo == null)
                    return;

                if (positionHandleControlEditor == -1)
                {
                    positionHandleControlEditor = edit.TabIndex;
                    if (edit.Visible)
                    {
                        edit.SelectAll();
                        edit.Focus();
                    }
                }
                if (positionHandleControlEditor > edit.TabIndex)
                {
                    positionHandleControlEditor = edit.TabIndex;
                    if (edit.Visible)
                    {
                        edit.SelectAll();
                        edit.Focus();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void CheckE_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                bool success = false;
                checkServiceAdo = new ServiceADO();
                checkServiceAdo = (ServiceADO)gridViewService.GetFocusedRow();
                var sources = (List<ServiceADO>)gridControlService.DataSource;
                var itemSources = sources.FirstOrDefault(o => o.ID == checkServiceAdo.ID);
                if (listCheckServiceAdos != null && listCheckServiceAdos.Count > 0)
                {
                    foreach (var item in listCheckServiceAdos)
                    {
                        if (item.ID == itemSources.ID)
                        {
                            listCheckServiceAdos.FirstOrDefault(o => o.ID == itemSources.ID).check2 = itemSources.check2;
                            listCheckServiceAdos.FirstOrDefault(o => o.ID == itemSources.ID).PRICE = itemSources.PRICE;

                            success = true;
                            break;
                        }
                    }
                    if (!success)
                    {
                        listCheckServiceAdos.Add(itemSources);
                    }
                }
                else
                {
                    listCheckServiceAdos.Add(itemSources);
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void Check_CheckedChanged(ExecuteRoleADO data)
        {
            try
            {
                bool success = false;
                var sources = (List<ExecuteRoleADO>)ERProcessor.GetDataGridView(ucGridControlER);
                var itemSources = sources.FirstOrDefault(o => o.ID == data.ID);
                if (listCheckERAdos != null && listCheckERAdos.Count > 0)
                {
                    foreach (var item in listCheckERAdos)
                    {
                        if (data.ID == item.ID)
                        {
                            listCheckERAdos.FirstOrDefault(o => o.ID == itemSources.ID).check1 = itemSources.check1;
                            listCheckERAdos.FirstOrDefault(o => o.ID == itemSources.ID).PRICE = itemSources.PRICE;
                            success = true;
                            break;
                        }
                    }
                    if (!success)
                    {
                        listCheckERAdos.Add(itemSources);
                    }
                }
                else
                {
                    listCheckERAdos.Add(itemSources);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void SpinPriceE_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                bool success = false;
                checkServiceAdo = new ServiceADO();
                checkServiceAdo = (ServiceADO)gridViewService.GetFocusedRow();
                var sources = (List<ServiceADO>)gridControlService.DataSource;
                var itemSources = sources.FirstOrDefault(o => o.ID == checkServiceAdo.ID);
                if (listCheckServiceAdos != null && listCheckServiceAdos.Count > 0)
                {
                    foreach (var item in listCheckServiceAdos)
                    {
                        if (item.ID == itemSources.ID)
                        {
                            listCheckServiceAdos.FirstOrDefault(o => o.ID == itemSources.ID).PRICE = itemSources.PRICE;

                            success = true;
                            break;
                        }
                    }
                    if (!success)
                    {
                        listCheckServiceAdos.Add(itemSources);
                    }
                }
                else
                {
                    listCheckServiceAdos.Add(itemSources);
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void Spin_EditValueChanged(ExecuteRoleADO data)
        {
            try
            {
                bool success = false;
                var sources = (List<ExecuteRoleADO>)ERProcessor.GetDataGridView(ucGridControlER);
                var itemSources = sources.FirstOrDefault(o => o.ID == data.ID);
                if (listCheckERAdos != null && listCheckERAdos.Count > 0)
                {
                    foreach (var item in listCheckERAdos)
                    {
                        if (data.ID == item.ID)
                        {
                            //listCheckERAdos.FirstOrDefault(o => o.ID == itemSources.ID).check1 = itemSources.check1;
                            listCheckERAdos.FirstOrDefault(o => o.ID == itemSources.ID).PRICE = itemSources.PRICE;
                            success = true;
                            break;
                        }
                    }
                    if (!success)
                    {
                        listCheckERAdos.Add(itemSources);
                    }
                }
                else
                {
                    listCheckERAdos.Add(itemSources);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

    }
}
