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
using HIS.Desktop.Plugins.RestRetrType.entity;
using HIS.UC.Service;
using HIS.UC.Service.ADO;
using Inventec.Core;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using HIS.UC.RehaTrainType;
using HIS.UC.RehaTrainType.ADO;
using Inventec.Common.Controls.EditorLoader;
using Inventec.Common.Adapter;
using HIS.Desktop.ApiConsumer;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using DevExpress.XtraEditors;
using HIS.Desktop.LocalStorage.ConfigApplication;
using System.Resources;
using Inventec.Desktop.Common.LanguageManager;

namespace HIS.Desktop.Plugins.RestRetrType
{
    public partial class UCRestRetrType : HIS.Desktop.Utility.UserControlBase
    {
        List<HIS_SERVICE> Service { get; set; }
        UCServiceProcessor ServiceProcessor;
        UCRehaTrainTypeProcessor RehaTrainTypeProcessor;
        UserControl ucGridControlService;
        UserControl ucGridControlRehaTrainType;
        int start = 0;
        int limit = 0;
        int rowCount = 0;
        int dataTotal = 0;
        int start1 = 0;
        int limit1 = 0;
        int rowCount1 = 0;
        int dataTotal1 = 0;
        bool checkRa = false;
        List<ServiceADO> listCheckServiceAdos = new List<ServiceADO>();
        List<RehaTrainTypeADO> listCheckRehaTrainTypeAdos = new List<RehaTrainTypeADO>();
        internal List<HIS.UC.Service.ServiceADO> lstServiceADOs { get; set; }
        internal List<HIS.UC.RehaTrainType.RehaTrainTypeADO> lstRehaTrainTypeADOs { get; set; }
        List<V_HIS_SERVICE> listService;
        List<V_HIS_REHA_TRAIN_TYPE> listRehaTrainType;
        long ServiceIdCheckByService = 0;
        long ServiceUnitId = 0;
        long RehaTrainTypeIdCheck = 0;
        long isChooseService;
        long isChooseRehaTrainType;
        bool isCheckAll;
        bool statecheckColumn;
        List<V_HIS_REST_RETR_TYPE> matyServices { get; set; }
        List<V_HIS_REST_RETR_TYPE> matyRehaTrainTypes { get; set; }

        public UCRestRetrType(Inventec.Desktop.Common.Modules.Module _moduleData)
            : base(_moduleData)
        {
            InitializeComponent();
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

        private void UCRestRetrType_Load(object sender, EventArgs e)
        {
            try
            {
                WaitingManager.Show();
                SetCaptionByLanguageKey();
                LoadComboStatus();
                InitUCgridService();
                InitUCgridRehaTrainType();
                FillDataToGridService(this);
                FillDataToGridRehaTrainType(this);
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadComboStatus()
        {
            try
            {
                List<Status> status = new List<Status>();
                status.Add(new Status(1, "Dịch vụ"));
                status.Add(new Status(2, "Kỹ thuật tập"));

                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("statusName", "", 300, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("statusName", "id", columnInfos, false, 350);
                ControlEditorLoader.Load(cboChoose, status, controlEditorADO);
                cboChoose.EditValue = status[1].id;
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void FillDataToGridRehaTrainType(UCRestRetrType uCRestRetrType)
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
                    numPageSize = (int)ConfigApplications.NumPageSize;
                }

                FillDataToGridRehaTrainTypePage(new CommonParam(0, numPageSize));

                CommonParam param = new CommonParam();
                param.Limit = rowCount1;
                param.Count = dataTotal1;
                ucPaging2.Init(FillDataToGridRehaTrainTypePage, param, numPageSize);
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void FillDataToGridRehaTrainTypePage(object data)
        {
            try
            {
                WaitingManager.Show();
                listRehaTrainType = new List<V_HIS_REHA_TRAIN_TYPE>();
                int start1 = ((CommonParam)data).Start ?? 0;
                int limit1 = ((CommonParam)data).Limit ?? 0;
                CommonParam param = new CommonParam(start1, limit1);
                MOS.Filter.HisRehaTrainTypeFilter RehaTrainTypeFilter = new HisRehaTrainTypeFilter();
                RehaTrainTypeFilter.ORDER_FIELD = "MODIFY_TIME";
                RehaTrainTypeFilter.ORDER_DIRECTION = "ASC";
                RehaTrainTypeFilter.KEY_WORD = txtKeyword2.Text;

                if ((long)cboChoose.EditValue == 2)
                {
                    isChooseRehaTrainType = (long)cboChoose.EditValue;
                }

                var mest = new Inventec.Common.Adapter.BackendAdapter(param).GetRO<List<MOS.EFMODEL.DataModels.V_HIS_REHA_TRAIN_TYPE>>(
                    "api/HisRehaTrainType/GetView",
                    HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer,
                    RehaTrainTypeFilter,
                    param);

                lstRehaTrainTypeADOs = new List<RehaTrainTypeADO>();
                if (mest != null && mest.Data.Count > 0)
                {
                    listRehaTrainType = mest.Data;
                    foreach (var item in listRehaTrainType)
                    {
                        RehaTrainTypeADO RehaTrainTypeADO = new RehaTrainTypeADO(item);
                        if (isChooseRehaTrainType == 2)
                        {
                            RehaTrainTypeADO.isKeyChooseRehaTrainType = true;
                        }
                        lstRehaTrainTypeADOs.Add(RehaTrainTypeADO);
                    }
                }

                if (matyServices != null && matyServices.Count > 0)
                {
                    foreach (var itemUsername in matyServices)
                    {
                        var check = lstRehaTrainTypeADOs.FirstOrDefault(o => o.ID == itemUsername.REHA_TRAIN_TYPE_ID);
                        if (check != null)
                        {
                            check.checkRehaTrainType = true;
                            //check.EXPEND_AMOUNT_STR = itemUsername.EXPEND_AMOUNT;
                            //check.EXPEND_PRICE_STR = itemUsername.EXPEND_PRICE;
                        }
                    }
                }
                lstRehaTrainTypeADOs = lstRehaTrainTypeADOs.OrderByDescending(p => p.checkRehaTrainType).ToList();

                if (RehaTrainTypeIdCheck != 0 && isChooseRehaTrainType == 2)
                {
                    var radioRehaTrainType = lstRehaTrainTypeADOs.Where(o => o.ID == RehaTrainTypeIdCheck).FirstOrDefault();
                    if (radioRehaTrainType != null)
                    {
                        radioRehaTrainType.radioRehaTrainType = true;
                    }
                }
                lstRehaTrainTypeADOs = lstRehaTrainTypeADOs.OrderByDescending(p => p.radioRehaTrainType).ToList();

                if (listCheckRehaTrainTypeAdos != null && listCheckRehaTrainTypeAdos.Count > 0)
                {
                    foreach (var item in listCheckRehaTrainTypeAdos)
                    {
                        var check = lstRehaTrainTypeADOs.FirstOrDefault(o => o.ID == item.ID);
                        if (check != null)
                        {
                            lstRehaTrainTypeADOs.FirstOrDefault(o => o.ID == item.ID).checkRehaTrainType = item.checkRehaTrainType;
                            //lstRehaTrainTypeADOs.FirstOrDefault(o => o.ID == item.ID).EXPEND_AMOUNT_STR = item.EXPEND_AMOUNT_STR;
                            //lstRehaTrainTypeADOs.FirstOrDefault(o => o.ID == item.ID).EXPEND_PRICE_STR = item.EXPEND_PRICE_STR;
                        }
                    }
                }

                if (ucGridControlRehaTrainType != null)
                {
                    RehaTrainTypeProcessor.Reload(ucGridControlRehaTrainType, lstRehaTrainTypeADOs);
                }
                rowCount1 = (data == null ? 0 : lstRehaTrainTypeADOs.Count);
                dataTotal1 = (mest.Param == null ? 0 : mest.Param.Count ?? 0);
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void FillDataToGridService(UCRestRetrType uCRestRetrType)
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
                    numPageSize = (int)ConfigApplications.NumPageSize;
                }

                FillDataToGridServicePage(new CommonParam(0, numPageSize));

                CommonParam param = new CommonParam();
                param.Limit = rowCount;
                param.Count = dataTotal;
                ucPaging1.Init(FillDataToGridServicePage, param, numPageSize);
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void FillDataToGridServicePage(object data)
        {
            try
            {
                WaitingManager.Show();
                listService = new List<V_HIS_SERVICE>();
                int start = ((CommonParam)data).Start ?? 0;
                int limit = ((CommonParam)data).Limit ?? 0;
                CommonParam param = new CommonParam(start, limit);
                MOS.Filter.HisServiceViewFilter ServiceFilter = new HisServiceViewFilter();
                ServiceFilter.SERVICE_TYPE_ID = IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__PHCN;
                ServiceFilter.ORDER_FIELD = "MODIFY_TIME";
                ServiceFilter.ORDER_DIRECTION = "ASC";
                ServiceFilter.KEY_WORD = txtKeyword1.Text;

                if ((long)cboChoose.EditValue == 1)
                {
                    isChooseService = (long)cboChoose.EditValue;
                }

                var rs = new Inventec.Common.Adapter.BackendAdapter(param).GetRO<List<MOS.EFMODEL.DataModels.V_HIS_SERVICE>>(
                    HIS.Desktop.ApiConsumer.HisRequestUriStore.HIS_SERVICE_GETVIEW,
                    HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer,
                    ServiceFilter,
                    param);

                lstServiceADOs = new List<ServiceADO>();
                if (rs != null && rs.Data.Count > 0)
                {
                    listService = rs.Data;
                    foreach (var item in listService)
                    {
                        ServiceADO ServiceAccountADO = new ServiceADO(item);
                        if (isChooseService == 1)
                        {
                            ServiceAccountADO.isKeyChooseService = true;
                        }
                        lstServiceADOs.Add(ServiceAccountADO);
                    }
                }

                if (matyRehaTrainTypes != null && matyRehaTrainTypes.Count > 0)
                {
                    foreach (var itemUsername in matyRehaTrainTypes)
                    {
                        var check = lstServiceADOs.FirstOrDefault(o => o.ID == itemUsername.REHA_SERVICE_TYPE_ID);
                        if (check != null)
                        {
                            check.checkService = true;
                        }
                    }
                }
                lstServiceADOs = lstServiceADOs.OrderByDescending(p => p.checkService).ToList();

                if (ServiceIdCheckByService != 0 && isChooseService == 1)
                {
                    var checkSevice = lstServiceADOs.Where(o => o.ID == ServiceIdCheckByService).FirstOrDefault();
                    if (checkSevice != null)
                    {
                        checkSevice.radioService = true;
                    }
                    lstServiceADOs = lstServiceADOs.OrderByDescending(p => p.radioService).ToList();
                }

                if (listCheckServiceAdos != null && listCheckServiceAdos.Count > 0)
                {
                    foreach (var item in listCheckServiceAdos)
                    {
                        var checks = lstServiceADOs.FirstOrDefault(o => o.ID == item.ID);
                        if (checks != null)
                        {
                            lstServiceADOs.FirstOrDefault(o => o.ID == item.ID).checkService = item.checkService;
                        }
                    }
                }

                if (ucGridControlService != null)
                {
                    ServiceProcessor.Reload(ucGridControlService, lstServiceADOs);
                }
                rowCount = (data == null ? 0 : lstServiceADOs.Count);
                dataTotal = (rs.Param == null ? 0 : rs.Param.Count ?? 0);
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void InitUCgridRehaTrainType()
        {
            try
            {
                var culture = Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture();
                var langManager = Resources.ResourceLanguageManager.LanguageResource;

                RehaTrainTypeProcessor = new UCRehaTrainTypeProcessor();
                RehaTrainTypeInitADO ado = new RehaTrainTypeInitADO();
                ado.ListRehaTrainTypeColumn = new List<RehaTrainTypeColumn>();
                ado.gridViewRehaTrainType_MouseDownRehaTrainType = gridViewRehaTrainType_MouseDownMate;
                ado.btn_Radio_Enable_Click = btn_Radio_Enable_Click_Mate;
                ado.Check__Enable_CheckedChanged = RehaTrainTypeCheckedChanged;

                RehaTrainTypeColumn colRadio2 = new RehaTrainTypeColumn("   ", "radioRehaTrainType", 30, true);
                colRadio2.VisibleIndex = 0;
                colRadio2.Visible = false;
                colRadio2.UnboundColumnType = DevExpress.Data.UnboundColumnType.Object;
                ado.ListRehaTrainTypeColumn.Add(colRadio2);

                RehaTrainTypeColumn colCheck2 = new RehaTrainTypeColumn("   ", "checkRehaTrainType", 30, true);
                colCheck2.VisibleIndex = 1;
                colCheck2.Visible = false;
                colCheck2.image = imageCollectionRehaTrainType.Images[0];
                colCheck2.UnboundColumnType = DevExpress.Data.UnboundColumnType.Object;
                ado.ListRehaTrainTypeColumn.Add(colCheck2);

                RehaTrainTypeColumn colMaLoaiVatTu = new RehaTrainTypeColumn(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_SERVICE_MATY__COLUMN_REHA_TRAIN_TYPE_TYPE_CODE", langManager, culture), "REHA_TRAIN_TYPE_CODE", 100, false);
                colMaLoaiVatTu.VisibleIndex = 2;
                ado.ListRehaTrainTypeColumn.Add(colMaLoaiVatTu);

                RehaTrainTypeColumn colTenLoaiVatTu = new RehaTrainTypeColumn(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_SERVICE_MATY__COLUMN_REHA_TRAIN_TYPE_TYPE_NAME", langManager, culture), "REHA_TRAIN_TYPE_NAME", 150, false);
                colTenLoaiVatTu.VisibleIndex = 3;
                ado.ListRehaTrainTypeColumn.Add(colTenLoaiVatTu);

                //RehaTrainTypeColumn colSoLuong = new RehaTrainTypeColumn(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_SERVICE_MATY__COLUMN_EXPEND_AMOUNT_STR", langManager, culture), "EXPEND_AMOUNT_STR", 100, true);
                //colSoLuong.VisibleIndex = 4;
                //colSoLuong.Visible = false;
                //colSoLuong.UnboundColumnType = DevExpress.Data.UnboundColumnType.Object;
                //ado.ListRehaTrainTypeColumn.Add(colSoLuong);

                //RehaTrainTypeColumn colGiaTien = new RehaTrainTypeColumn(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_SERVICE_MATY__COLUMN_EXPEND_PRICE_STR", langManager, culture), "EXPEND_PRICE_STR", 100, true);
                //colGiaTien.VisibleIndex = 5;
                //colGiaTien.Visible = false;
                //colGiaTien.UnboundColumnType = DevExpress.Data.UnboundColumnType.Object;
                //ado.ListRehaTrainTypeColumn.Add(colGiaTien);

                this.ucGridControlRehaTrainType = (UserControl)RehaTrainTypeProcessor.Run(ado);

                if (ucGridControlRehaTrainType != null)
                {
                    this.panelControlTrain.Controls.Add(this.ucGridControlRehaTrainType);
                    this.ucGridControlRehaTrainType.Dock = DockStyle.Fill;
                }

            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void RehaTrainTypeCheckedChanged(RehaTrainTypeADO data)
        {
            try
            {
                bool success = false;
                var sources = (List<RehaTrainTypeADO>)RehaTrainTypeProcessor.GetDataGridView(ucGridControlRehaTrainType);
                var itemSources = sources.FirstOrDefault(o => o.ID == data.ID);
                if (listCheckRehaTrainTypeAdos != null && listCheckRehaTrainTypeAdos.Count > 0)
                {
                    foreach (var item in listCheckRehaTrainTypeAdos)
                    {
                        if (data.ID == item.ID)
                        {
                            listCheckRehaTrainTypeAdos.FirstOrDefault(o => o.ID == itemSources.ID).checkRehaTrainType = itemSources.checkRehaTrainType;
                            success = true;
                            break;
                        }
                    }
                    if (!success)
                    {
                        listCheckRehaTrainTypeAdos.Add(itemSources);
                    }
                }
                else
                {
                    listCheckRehaTrainTypeAdos.Add(itemSources);
                }
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewRehaTrainType_MouseDownMate(object sender, MouseEventArgs e)
        {
            try
            {
                if (isChooseRehaTrainType == 2)
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
                        if (hi.Column.FieldName == "checkRehaTrainType")
                        {
                            var lstCheckAll = lstRehaTrainTypeADOs;
                            List<HIS.UC.RehaTrainType.RehaTrainTypeADO> lstChecks = new List<HIS.UC.RehaTrainType.RehaTrainTypeADO>();

                            if (lstCheckAll != null && lstCheckAll.Count > 0)
                            {
                                var RehaTrainTypeCheckedNum = lstRehaTrainTypeADOs.Where(o => o.checkRehaTrainType == true).Count();
                                var RehaTrainTypeNum = lstRehaTrainTypeADOs.Count();
                                if ((RehaTrainTypeCheckedNum > 0 && RehaTrainTypeCheckedNum < RehaTrainTypeNum) || RehaTrainTypeCheckedNum == 0)
                                {
                                    isCheckAll = true;
                                    hi.Column.Image = imageCollectionRehaTrainType.Images[1];
                                }

                                if (RehaTrainTypeCheckedNum == RehaTrainTypeNum)
                                {
                                    isCheckAll = false;
                                    hi.Column.Image = imageCollectionRehaTrainType.Images[0];
                                }
                                if (isCheckAll)
                                {
                                    foreach (var item in lstCheckAll)
                                    {
                                        if (item.ID != null)
                                        {
                                            item.checkRehaTrainType = true;
                                            lstChecks.Add(item);
                                        }
                                        else
                                        {
                                            lstChecks.Add(item);
                                        }
                                    }
                                    isCheckAll = false;
                                }
                                else
                                {
                                    foreach (var item in lstCheckAll)
                                    {
                                        if (item.ID != null)
                                        {
                                            item.checkRehaTrainType = false;
                                            lstChecks.Add(item);
                                        }
                                        else
                                        {
                                            lstChecks.Add(item);
                                        }
                                    }
                                    isCheckAll = true;
                                }

                                //ReloadData
                                RehaTrainTypeProcessor.Reload(ucGridControlRehaTrainType, lstChecks);
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

        private void btn_Radio_Enable_Click_Mate(V_HIS_REHA_TRAIN_TYPE data)
        {
            try
            {
                WaitingManager.Show();
                CommonParam param = new CommonParam();
                MOS.Filter.HisRestRetrTypeViewFilter matyServiceFilter = new HisRestRetrTypeViewFilter();
                matyServiceFilter.REHA_TRAIN_TYPE_ID = data.ID;
                RehaTrainTypeIdCheck = data.ID;

                matyRehaTrainTypes = new BackendAdapter(param).Get<List<V_HIS_REST_RETR_TYPE>>(
                    "api/HisRestRetrType/GetView",
                   ApiConsumers.MosConsumer,
                   matyServiceFilter,
                   param);
                lstServiceADOs = new List<HIS.UC.Service.ServiceADO>();

                lstServiceADOs = (from r in listService select new ServiceADO(r)).ToList();
                if (matyRehaTrainTypes != null && matyRehaTrainTypes.Count > 0)
                {
                    foreach (var itemUsername in matyRehaTrainTypes)
                    {
                        var check = lstServiceADOs.FirstOrDefault(o => o.ID == itemUsername.REHA_SERVICE_TYPE_ID);
                        if (check != null)
                        {
                            check.checkService = true;
                        }
                    }
                }

                lstServiceADOs = lstServiceADOs.OrderByDescending(p => p.checkService).ToList();
                if (ucGridControlService != null)
                {
                    ServiceProcessor.Reload(ucGridControlService, lstServiceADOs);
                }
                checkRa = true;
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void InitUCgridService()
        {
            try
            {
                var culture = Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture();
                var langManager = Resources.ResourceLanguageManager.LanguageResource;
                ServiceProcessor = new UCServiceProcessor();
                ServiceInitADO ado = new ServiceInitADO();
                ado.ListServiceColumn = new List<ServiceColumn>();
                ado.gridViewService_MouseDownMest = gridViewService_MouseDownMest;
                ado.btn_Radio_Enable_Click1 = btn_Radio_Enable_Click1;
                ado.Check__Enable_CheckedChanged = serviceCheckedChanged;

                ServiceColumn colRadio1 = new ServiceColumn("   ", "radioService", 30, true);
                colRadio1.VisibleIndex = 0;
                colRadio1.Visible = false;
                colRadio1.UnboundColumnType = DevExpress.Data.UnboundColumnType.Object;
                ado.ListServiceColumn.Add(colRadio1);

                ServiceColumn colCheck1 = new ServiceColumn("   ", "checkService", 30, true);
                colCheck1.VisibleIndex = 1;
                colCheck1.image = imageCollectionRehaServiceType.Images[0];
                colCheck1.Visible = false;
                colCheck1.UnboundColumnType = DevExpress.Data.UnboundColumnType.Object;
                ado.ListServiceColumn.Add(colCheck1);

                ServiceColumn colMaDichVu = new ServiceColumn(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_SERVICE_MATY__COLUMN_SERVICE_CODE", langManager, culture), "SERVICE_CODE", 100, false);
                colMaDichVu.VisibleIndex = 2;
                ado.ListServiceColumn.Add(colMaDichVu);

                ServiceColumn colTenDichVu = new ServiceColumn(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_SERVICE_MATY__COLUMN_SERVICE_NAME", langManager, culture), "SERVICE_NAME", 150, false);
                colTenDichVu.VisibleIndex = 3;
                ado.ListServiceColumn.Add(colTenDichVu);

                //ServiceColumn colLoaiDichVu = new ServiceColumn("Loại dịch vụ", "SERVICE_TYPE_NAME", 100, false);
                //colLoaiDichVu.VisibleIndex = 4;
                //ado.ListServiceColumn.Add(colLoaiDichVu);

                this.ucGridControlService = (UserControl)ServiceProcessor.Run(ado);
                if (ucGridControlService != null)
                {
                    this.panelControlService.Controls.Add(this.ucGridControlService);
                    this.ucGridControlService.Dock = DockStyle.Fill;
                }
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void serviceCheckedChanged(ServiceADO data)
        {
            try
            {
                bool success = false;
                var sources = (List<ServiceADO>)ServiceProcessor.GetDataGridView(ucGridControlService);
                var itemSources = sources.FirstOrDefault(o => o.ID == data.ID);
                if (listCheckServiceAdos != null && listCheckServiceAdos.Count > 0)
                {
                    foreach (var item in listCheckServiceAdos)
                    {
                        if (data.ID == item.ID)
                        {
                            listCheckServiceAdos.FirstOrDefault(o => o.ID == itemSources.ID).checkService = itemSources.checkService;
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

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewService_MouseDownMest(object sender, MouseEventArgs e)
        {
            try
            {
                if (isChooseService == 1)
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
                        if (hi.Column.FieldName == "checkService")
                        {
                            var lstCheckAll = lstServiceADOs;
                            List<HIS.UC.Service.ServiceADO> lstChecks = new List<HIS.UC.Service.ServiceADO>();

                            if (lstCheckAll != null && lstCheckAll.Count > 0)
                            {
                                var ServiceCheckedNum = lstServiceADOs.Where(o => o.checkService == true).Count();
                                var ServiceNum = lstServiceADOs.Count();
                                if ((ServiceCheckedNum > 0 && ServiceCheckedNum < ServiceNum) || ServiceCheckedNum == 0)
                                {
                                    isCheckAll = true;
                                    hi.Column.Image = imageCollectionRehaServiceType.Images[1];
                                }

                                if (ServiceCheckedNum == ServiceNum)
                                {
                                    isCheckAll = false;
                                    hi.Column.Image = imageCollectionRehaServiceType.Images[0];
                                }

                                if (isCheckAll)
                                {
                                    foreach (var item in lstCheckAll)
                                    {
                                        if (item.ID != null)
                                        {
                                            item.checkService = true;
                                            lstChecks.Add(item);
                                        }
                                        else
                                        {
                                            lstChecks.Add(item);
                                        }
                                    }
                                    isCheckAll = false;
                                }
                                else
                                {
                                    foreach (var item in lstCheckAll)
                                    {
                                        if (item.ID != null)
                                        {
                                            item.checkService = false;
                                            lstChecks.Add(item);
                                        }
                                        else
                                        {
                                            lstChecks.Add(item);
                                        }
                                    }
                                    isCheckAll = true;
                                }

                                ServiceProcessor.Reload(ucGridControlService, lstChecks);
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

        private void btn_Radio_Enable_Click1(V_HIS_SERVICE data)
        {
            try
            {
                WaitingManager.Show();
                CommonParam param = new CommonParam();
                MOS.Filter.HisRestRetrTypeViewFilter matyServiceFilter = new HisRestRetrTypeViewFilter();
                matyServiceFilter.SERVICE_ID = data.ID;
                ServiceIdCheckByService = data.ID;
                ServiceUnitId = data.SERVICE_UNIT_ID;

                matyServices = new BackendAdapter(param).Get<List<V_HIS_REST_RETR_TYPE>>(
                                "api/HisRestRetrType/GetView",
                                HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer,
                                matyServiceFilter,
                                param);
                lstRehaTrainTypeADOs = new List<HIS.UC.RehaTrainType.RehaTrainTypeADO>();
                lstRehaTrainTypeADOs = (from r in listRehaTrainType select new RehaTrainTypeADO(r)).ToList();
                if (matyServices != null && matyServices.Count > 0)
                {
                    foreach (var itemUsername in matyServices)
                    {
                        var check = lstRehaTrainTypeADOs.FirstOrDefault(o => o.ID == itemUsername.REHA_TRAIN_TYPE_ID);
                        if (check != null)
                        {
                            check.checkRehaTrainType = true;
                            //check.EXPEND_PRICE_STR = itemUsername.EXPEND_PRICE;
                            //check.EXPEND_AMOUNT_STR = itemUsername.EXPEND_AMOUNT;
                        }
                    }
                }

                lstRehaTrainTypeADOs = lstRehaTrainTypeADOs.OrderByDescending(p => p.checkRehaTrainType).ToList();
                if (ucGridControlRehaTrainType != null)
                {
                    RehaTrainTypeProcessor.Reload(ucGridControlRehaTrainType, lstRehaTrainTypeADOs);
                }
                checkRa = true;
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnFind2_Click(object sender, EventArgs e)
        {
            try
            {
                WaitingManager.Show();
                FillDataToGridRehaTrainType(this);
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnFind1_Click(object sender, EventArgs e)
        {
            try
            {
                WaitingManager.Show();
                FillDataToGridService(this);
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboChoose_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                WaitingManager.Show();
                checkRa = false;
                isChooseRehaTrainType = 0;
                isChooseService = 0;
                txtKeyword1.Text = null;
                txtKeyword2.Text = null;
                listCheckServiceAdos = new List<ServiceADO>();
                listCheckRehaTrainTypeAdos = new List<RehaTrainTypeADO>();
                matyServices = new List<V_HIS_REST_RETR_TYPE>();
                matyRehaTrainTypes = new List<V_HIS_REST_RETR_TYPE>();
                FillDataToGridRehaTrainType(this);
                FillDataToGridService(this);
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                WaitingManager.Show();
                if (ucGridControlRehaTrainType != null && ucGridControlService != null)
                {
                    object RehaTrainType = RehaTrainTypeProcessor.GetDataGridView(ucGridControlRehaTrainType);
                    object Service = ServiceProcessor.GetDataGridView(ucGridControlService);
                    bool success = false;
                    CommonParam param = new CommonParam();
                    if (isChooseService == 1)
                    {
                        if (RehaTrainType is List<HIS.UC.RehaTrainType.RehaTrainTypeADO>)
                        {
                            this.lstRehaTrainTypeADOs = (List<HIS.UC.RehaTrainType.RehaTrainTypeADO>)RehaTrainType;

                            if (this.lstRehaTrainTypeADOs != null && this.lstRehaTrainTypeADOs.Count > 0 && checkRa == true)
                            {
                                //Danh sach cac user duoc check

                                var dataCheckeds = this.lstRehaTrainTypeADOs.Where(p => p.checkRehaTrainType == true).ToList();

                                //List xoa

                                var dataDeletes = this.lstRehaTrainTypeADOs.Where(o => matyServices.Select(p => p.REHA_TRAIN_TYPE_ID)
                                    .Contains(o.ID) && o.checkRehaTrainType == false).ToList();

                                //list them
                                var dataCreates = dataCheckeds.Where(o => !matyServices.Select(p => p.REHA_TRAIN_TYPE_ID)
                                    .Contains(o.ID)).ToList();
                                //List update
                                //var dataUpdate = dataCheckeds.Where(o => matyServices.Select(p => p.REHA_TRAIN_TYPE_ID)
                                //   .Contains(o.ID)).ToList();

                                if (dataCheckeds.Count == 0 && dataDeletes.Count == 0)
                                {
                                    DevExpress.XtraEditors.XtraMessageBox.Show("Chưa chọn loại vật tư", "Thông báo");
                                    return;
                                }
                                if (dataCheckeds != null)
                                {
                                    success = true;
                                }

                                //xử lý update
                                // success = UpdateRehaTrainTypeProcess(dataUpdate, param);

                                //xử lý delete
                                if (dataDeletes != null && dataDeletes.Count > 0)
                                {
                                    success = DeleteRehaTrainTypeProcess(dataDeletes, param);
                                }

                                //xử lý thêm
                                if (dataCreates != null && dataCreates.Count > 0)
                                {
                                    success = CreateRehaTrainTypeProcess(dataCreates, param);
                                }

                                this.lstRehaTrainTypeADOs = this.lstRehaTrainTypeADOs.OrderByDescending(p => p.checkRehaTrainType).ToList();
                                if (ucGridControlRehaTrainType != null)
                                {
                                    RehaTrainTypeProcessor.Reload(ucGridControlRehaTrainType, this.lstRehaTrainTypeADOs);
                                }
                            }
                        }
                    }
                    if (isChooseRehaTrainType == 2)
                    {
                        if (Service is List<HIS.UC.Service.ServiceADO>)
                        {
                            this.lstServiceADOs = (List<HIS.UC.Service.ServiceADO>)Service;

                            if (this.lstServiceADOs != null && this.lstServiceADOs.Count > 0)
                            {
                                //bool success = false;
                                HIS.UC.RehaTrainType.RehaTrainTypeADO RehaTrainTypeType = this.lstRehaTrainTypeADOs.FirstOrDefault(o => o.ID == RehaTrainTypeIdCheck);
                                //Danh sach cac user duoc check

                                var dataChecked = this.lstServiceADOs.Where(p => p.checkService == true).ToList();


                                //List xoa

                                var dataDelete = this.lstServiceADOs.Where(o => matyRehaTrainTypes.Select(p => p.REHA_SERVICE_TYPE_ID)
                                    .Contains(o.ID) && o.checkService == false).ToList();

                                //list them
                                var dataCreate = dataChecked.Where(o => !matyRehaTrainTypes.Select(p => p.REHA_SERVICE_TYPE_ID)
                                    .Contains(o.ID)).ToList();
                                //list update
                                //var dataUpdate = dataChecked.Where(o => matyRehaTrainTypes.Select(p => p.REHA_SERVICE_TYPE_ID)
                                //.Contains(o.ID)).ToList();

                                if (dataChecked.Count == 0 && dataDelete.Count == 0)
                                {
                                    DevExpress.XtraEditors.XtraMessageBox.Show("Chưa dịch vụ nào", "Thông báo");
                                    return;
                                }
                                //if (dataChecked != null)
                                //{
                                //    success = true;
                                //}

                                //xử lý update
                                //UpdateServiceProcess(dataUpdate, RehaTrainTypeType, param);

                                //xử lý delete
                                if (dataDelete != null && dataDelete.Count > 0)
                                {
                                    success = DeleteServiceProcess(dataDelete, param);
                                }

                                //xử lý Create
                                if (dataCreate != null && dataCreate.Count > 0)
                                {
                                    success = CreateServiceProcess(dataCreate, RehaTrainTypeType, param);
                                }

                                this.lstServiceADOs = this.lstServiceADOs.OrderByDescending(p => p.checkService).ToList();
                                if (ucGridControlService != null)
                                {
                                    ServiceProcessor.Reload(ucGridControlService, this.lstServiceADOs);
                                }
                            }
                        }
                    }
                    MessageManager.Show(this.ParentForm, param, success);
                }
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private bool CreateServiceProcess(List<ServiceADO> dataCreate, RehaTrainTypeADO RehaTrainTypeType, CommonParam param)
        {
            bool success = false;
            if (dataCreate != null && dataCreate.Count > 0 && RehaTrainTypeType != null)
            {
                //if (RehaTrainTypeType.EXPEND_AMOUNT_STR <= 0)
                //{
                //    DevExpress.XtraEditors.XtraMessageBox.Show("Số lượng phải lớn hơn không", "Thông báo");
                //    success = false;
                //    return success;
                //}
                List<HIS_REST_RETR_TYPE> mestServiceCreate = new List<HIS_REST_RETR_TYPE>();
                foreach (var item in dataCreate)
                {
                    HIS_REST_RETR_TYPE mestService = new HIS_REST_RETR_TYPE();
                    mestService.REHA_SERVICE_TYPE_ID = item.ID;
                    mestService.REHA_TRAIN_TYPE_ID = RehaTrainTypeIdCheck;
                    //mestService.EXPEND_AMOUNT = RehaTrainTypeType.EXPEND_AMOUNT_STR;
                    //mestService.EXPEND_PRICE = RehaTrainTypeType.EXPEND_PRICE_STR;
                    //mestService.SERVICE_UNIT_ID = item.SERVICE_UNIT_ID;
                    mestServiceCreate.Add(mestService);
                }

                var createResult = new BackendAdapter(param).Post<List<HIS_REST_RETR_TYPE>>(
                           "api/HisRestRetrType/CreateList",
                           HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer,
                           mestServiceCreate,
                           param);
                if (createResult != null && createResult.Count > 0)
                {
                    success = true;
                    AutoMapper.Mapper.CreateMap<HIS_REST_RETR_TYPE, V_HIS_REST_RETR_TYPE>();
                    var vCreateResults = AutoMapper.Mapper.Map<List<HIS_REST_RETR_TYPE>, List<V_HIS_REST_RETR_TYPE>>(createResult);
                    matyRehaTrainTypes.AddRange(vCreateResults);
                }
            }
            return success;
        }

        //Hàm xử lý xóa service
        private bool DeleteServiceProcess(List<ServiceADO> dataDelete, CommonParam param)
        {
            bool success = false;
            if (dataDelete != null && dataDelete.Count > 0)
            {
                List<long> deleteIds = matyRehaTrainTypes.Where(o => dataDelete.Select(p => p.ID)
                    .Contains(o.REHA_SERVICE_TYPE_ID)).Select(o => o.ID).ToList();
                bool deleteResult = new BackendAdapter(param).Post<bool>(
                          "api/HisRestRetrType/DeleteList",
                          HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer,
                          deleteIds,
                          param);
                if (deleteResult)
                {
                    success = true;
                    matyRehaTrainTypes = matyRehaTrainTypes.Where(o => !deleteIds.Contains(o.ID)).ToList();
                }
            }
            return success;
        }

        //Hàm xử lý update Service
        private bool UpdateServiceProcess(List<ServiceADO> dataUpdate, RehaTrainTypeADO RehaTrainTypeType, CommonParam param)
        {
            bool success = false;
            if (dataUpdate != null && dataUpdate.Count > 0 && RehaTrainTypeType != null)
            {
                var RehaTrainTypeMetyUpdates = new List<V_HIS_REST_RETR_TYPE>();
                foreach (var item in dataUpdate)
                {
                    //if (RehaTrainTypeType.EXPEND_AMOUNT_STR <= 0)
                    //{
                    //    DevExpress.XtraEditors.XtraMessageBox.Show("Số lượng phải lớn hơn không", "Thông báo");
                    //    success = false;
                    //    return success;
                    //}
                    var RehaTrainTypeMaty = matyRehaTrainTypes.FirstOrDefault(o => o.REHA_TRAIN_TYPE_ID == RehaTrainTypeIdCheck && o.REHA_SERVICE_TYPE_ID == item.ID);
                    if (RehaTrainTypeMaty != null)
                    {
                        // RehaTrainTypeMaty.EXPEND_AMOUNT = RehaTrainTypeType.EXPEND_AMOUNT_STR;
                        // RehaTrainTypeMaty.EXPEND_PRICE = RehaTrainTypeType.EXPEND_PRICE_STR;
                        //MedicineMety.SERVICE_UNIT_ID = item.SERVICE_UNIT_ID;
                        //MedicineMety.MEDICINE_TYPE_ID = MedicineIdCheck;
                        RehaTrainTypeMetyUpdates.Add(RehaTrainTypeMaty);
                    }
                }
                if (RehaTrainTypeMetyUpdates != null && RehaTrainTypeMetyUpdates.Count > 0)
                {
                    var updateResult = new BackendAdapter(param).Post<List<HIS_REST_RETR_TYPE>>(
                               "/api/HisRestRetrType/UpdateList",
                               HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer,
                               RehaTrainTypeMetyUpdates,
                               param);
                    if (updateResult != null && updateResult.Count > 0)
                    {
                        //listMediStockMety.AddRange(updateResult);
                        success = true;
                    }
                }
            }
            return success;
        }

        //xử lý thêm vật tư
        private bool CreateRehaTrainTypeProcess(List<RehaTrainTypeADO> dataCreates, CommonParam param)
        {
            bool success = false;
            if (dataCreates != null && dataCreates.Count > 0)
            {
                List<V_HIS_REST_RETR_TYPE> MestServiceCreates = new List<V_HIS_REST_RETR_TYPE>();
                foreach (var item in dataCreates)
                {
                    //if (item.EXPEND_AMOUNT_STR <= 0)
                    //{
                    //    DevExpress.XtraEditors.XtraMessageBox.Show("Số lượng phải lớn hơn không", "Thông báo");
                    //    success = false;
                    //    return success;
                    //}
                    V_HIS_REST_RETR_TYPE mestService = new V_HIS_REST_RETR_TYPE();
                    mestService.REHA_TRAIN_TYPE_ID = item.ID;
                    //mestService.SERVICE_UNIT_ID = ServiceUnitId;
                    mestService.REHA_SERVICE_TYPE_ID = ServiceIdCheckByService;
                    //mestService.EXPEND_AMOUNT = item.EXPEND_AMOUNT_STR;
                    //mestService.EXPEND_PRICE = item.EXPEND_PRICE_STR;
                    MestServiceCreates.Add(mestService);
                }

                var createResult = new BackendAdapter(param).Post<List<HIS_REST_RETR_TYPE>>(
                           "api/HisRestRetrType/CreateList",
                           HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer,
                           MestServiceCreates,
                           param);
                if (createResult != null && createResult.Count > 0)
                {
                    success = true;
                    AutoMapper.Mapper.CreateMap<HIS_REST_RETR_TYPE, V_HIS_REST_RETR_TYPE>();
                    var vCreateResults = AutoMapper.Mapper.Map<List<HIS_REST_RETR_TYPE>, List<V_HIS_REST_RETR_TYPE>>(createResult);
                    matyServices.AddRange(vCreateResults);
                }
            }
            return success;
        }

        //Hàm xóa RehaTrainType
        private bool DeleteRehaTrainTypeProcess(List<RehaTrainTypeADO> dataDeletes, CommonParam param)
        {
            bool success = false;
            if (dataDeletes != null && dataDeletes.Count > 0)
            {
                List<long> deleteIds = matyServices.Where(o => dataDeletes.Select(p => p.ID)
                    .Contains(o.REHA_TRAIN_TYPE_ID)).Select(o => o.ID).ToList();
                bool deleteResult = new BackendAdapter(param).Post<bool>(
                          "api/HisRestRetrType/DeleteList",
                          HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer,
                          deleteIds,
                          param);
                if (deleteResult)
                {
                    success = true;
                    matyServices = matyServices.Where(o => !deleteIds.Contains(o.ID)).ToList();
                }
            }
            return success;
        }

        //Hàm update RehaTrainType
        private bool UpdateRehaTrainTypeProcess(List<RehaTrainTypeADO> dataUpdate, CommonParam param)
        {

            bool success = false;
            if (dataUpdate != null && dataUpdate.Count > 0)
            {
                var RestRetrTypeUpdates = new List<V_HIS_REST_RETR_TYPE>();
                foreach (var item in dataUpdate)
                {
                    //if (item.EXPEND_AMOUNT_STR <= 0)
                    //{
                    //    DevExpress.XtraEditors.XtraMessageBox.Show("Số lượng phải lớn hơn không", "Thông báo");
                    //    success = false;
                    //    return success;
                    //}
                    var RestRetrType = matyServices.FirstOrDefault(o => o.REHA_TRAIN_TYPE_ID == item.ID && o.REHA_SERVICE_TYPE_ID == ServiceIdCheckByService);
                    if (RestRetrType != null)
                    {
                        //RestRetrType.EXPEND_AMOUNT = item.EXPEND_AMOUNT_STR;
                        // RestRetrType.EXPEND_PRICE = item.EXPEND_PRICE_STR;
                        RestRetrTypeUpdates.Add(RestRetrType);
                    }
                }
                if (RestRetrTypeUpdates != null && RestRetrTypeUpdates.Count > 0)
                {
                    var updateResult = new BackendAdapter(param).Post<List<HIS_REST_RETR_TYPE>>(
                               "/api/HisRestRetrType/UpdateList",
                               HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer,
                               RestRetrTypeUpdates,
                               param);
                    if (updateResult != null && updateResult.Count > 0)
                    {
                        //listMediStockMety.AddRange(updateResult);
                        success = true;
                    }
                }
            }
            return success;
        }

        //xử lý tìm kiếm dịch vụ
        private void txtKeyword1_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                WaitingManager.Show();
                if (e.KeyCode == Keys.Enter)
                {
                    FillDataToGridService(this);
                }
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        //xử lý tìm kiếm vật tư
        private void txtKeyword2_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                WaitingManager.Show();
                if (e.KeyCode == Keys.Enter)
                {
                    FillDataToGridRehaTrainType(this);
                }
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetCaptionByLanguageKey()
        {
            try
            {
                //Khởi tạo đối tượng resources
                Resources.ResourceLanguageManager.LanguageResource = new System.Resources.ResourceManager("HIS.Desktop.Plugins.RestRetrType.Resources.Lang", typeof(HIS.Desktop.Plugins.RestRetrType.UCRestRetrType).Assembly);
                //Gán giá trị cho các control
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
