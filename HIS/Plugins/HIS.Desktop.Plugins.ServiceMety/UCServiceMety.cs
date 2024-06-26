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
using HIS.Desktop.Plugins.ServiceMety.entity;
using HIS.UC.Service;
using HIS.UC.Service.ADO;
using Inventec.Core;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using HIS.UC.Medicine;
using HIS.UC.Medicine.ADO;
using Inventec.Common.Controls.EditorLoader;
using Inventec.Common.Adapter;
using HIS.Desktop.ApiConsumer;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using DevExpress.XtraEditors;
using HIS.Desktop.LocalStorage.ConfigApplication;
using System.Resources;
using Inventec.Desktop.Common.LanguageManager;

namespace HIS.Desktop.Plugins.ServiceMety
{
    public partial class UCServiceMety : HIS.Desktop.Utility.UserControlBase
    {
        List<HIS_SERVICE> Service { get; set; }
        UCServiceProcessor ServiceProcessor;
        UCMedicineProcessor MedicineProcessor;
        UserControl ucGridControlService;
        UserControl ucGridControlMedicine;
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
        List<MedicineADO> listCheckMedicineAdos = new List<MedicineADO>();
        internal List<HIS.UC.Service.ServiceADO> lstServiceADOs { get; set; }
        internal List<HIS.UC.Medicine.MedicineADO> lstMedicineADOs { get; set; }
        List<V_HIS_SERVICE> listService;
        List<V_HIS_MEDICINE_TYPE> listMedicine;
        long ServiceIdCheckByService = 0;
        long ServiceUnitId = 0;
        long MedicineIdCheck = 0;
        long isChooseService;
        long isChooseMedicine;
        bool isCheckAll;
        bool statecheckColumn;
        List<V_HIS_SERVICE_METY> matyServices { get; set; }
        List<V_HIS_SERVICE_METY> matyMedicines { get; set; }

        public UCServiceMety(Inventec.Desktop.Common.Modules.Module _currentModule)
            : base(_currentModule)
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

        private void UCServiceMety_Load(object sender, EventArgs e)
        {
            try
            {
                WaitingManager.Show();
                SetCaptionByLanguageKey();
                LoadComboStatus();
                InitUCgridService();
                InitUCgridMedicine();
                FillDataToGridService(this);
                FillDataToGridMedicine(this);
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
                status.Add(new Status(2, "Thuốc"));

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

        private void FillDataToGridMedicine(UCServiceMety uCServiceMety)
        {
            try
            {
                MedicineIdCheck = 0;
                int numPageSize;
                if (ucPaging2.pagingGrid != null)
                {
                    numPageSize = ucPaging2.pagingGrid.PageSize;
                }
                else
                {
                    numPageSize = (int)ConfigApplications.NumPageSize;
                }

                FillDataToGridMedicinePage(new CommonParam(0, numPageSize));

                CommonParam param = new CommonParam();
                param.Limit = rowCount1;
                param.Count = dataTotal1;
                ucPaging2.Init(FillDataToGridMedicinePage, param, numPageSize);
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void FillDataToGridMedicinePage(object data)
        {
            try
            {
                WaitingManager.Show();
                listMedicine = new List<V_HIS_MEDICINE_TYPE>();
                int start1 = ((CommonParam)data).Start ?? 0;
                int limit1 = ((CommonParam)data).Limit ?? 0;
                CommonParam param = new CommonParam(start1, limit1);
                MOS.Filter.HisMedicineTypeFilter MedicineFilter = new HisMedicineTypeFilter();
                MedicineFilter.ORDER_FIELD = "CREATE_TIME";
                MedicineFilter.ORDER_DIRECTION = "DESC";
                MedicineFilter.KEY_WORD = txtKeyword2.Text;

                if ((long)cboChoose.EditValue == 2)
                {
                    isChooseMedicine = (long)cboChoose.EditValue;
                }

                var mest = new Inventec.Common.Adapter.BackendAdapter(param).GetRO<List<MOS.EFMODEL.DataModels.V_HIS_MEDICINE_TYPE>>(
                    HIS.Desktop.ApiConsumer.HisRequestUriStore.HIS_MEDICINE_TYPE_GETVIEW,
                    HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer,
                    MedicineFilter,
                    param);

                lstMedicineADOs = new List<MedicineADO>();
                if (mest != null && mest.Data.Count > 0)
                {
                    listMedicine = mest.Data;
                    foreach (var item in listMedicine)
                    {
                        MedicineADO MedicineADO = new MedicineADO(item);
                        if (isChooseMedicine == 2)
                        {
                            MedicineADO.isKeyChooseMedi = true;
                        }
                        lstMedicineADOs.Add(MedicineADO);
                    }
                }

                if (matyServices != null && matyServices.Count > 0)
                {
                    foreach (var itemUsername in matyServices)
                    {
                        var check = lstMedicineADOs.FirstOrDefault(o => o.ID == itemUsername.MEDICINE_TYPE_ID);
                        if (check != null)
                        {
                            check.checkMedi = true;
                            check.checkExpend = itemUsername.IS_NOT_EXPEND == 1;
                            check.AMOUNT_BHYT_STR = itemUsername.AMOUNT_BHYT;
                            check.EXPEND_AMOUNT_STR = itemUsername.EXPEND_AMOUNT;
                            check.EXPEND_PRICE_STR = itemUsername.EXPEND_PRICE;
                        }
                    }
                }
                lstMedicineADOs = lstMedicineADOs.OrderByDescending(p => p.checkMedi).ToList();

                if (MedicineIdCheck != 0 && isChooseMedicine == 2)
                {
                    var radioMedicine = lstMedicineADOs.Where(o => o.ID == MedicineIdCheck).FirstOrDefault();
                    if (radioMedicine != null)
                    {
                        radioMedicine.radioMedi = true;
                    }
                }
                lstMedicineADOs = lstMedicineADOs.OrderByDescending(p => p.radioMedi).ToList();

                if (listCheckMedicineAdos != null && listCheckMedicineAdos.Count > 0)
                {
                    foreach (var item in listCheckMedicineAdos)
                    {
                        var check = lstMedicineADOs.FirstOrDefault(o => o.ID == item.ID);
                        if (check != null)
                        {
                            lstMedicineADOs.FirstOrDefault(o => o.ID == item.ID).checkMedi = item.checkMedi;
                            lstMedicineADOs.FirstOrDefault(o => o.ID == item.ID).AMOUNT_BHYT_STR = item.AMOUNT_BHYT_STR;
                            lstMedicineADOs.FirstOrDefault(o => o.ID == item.ID).checkExpend = item.checkExpend;
                            lstMedicineADOs.FirstOrDefault(o => o.ID == item.ID).EXPEND_AMOUNT_STR = item.EXPEND_AMOUNT_STR;
                            lstMedicineADOs.FirstOrDefault(o => o.ID == item.ID).EXPEND_PRICE_STR = item.EXPEND_PRICE_STR;
                        }
                    }
                }

                if (ucGridControlMedicine != null)
                {
                    MedicineProcessor.Reload(ucGridControlMedicine, lstMedicineADOs);
                }
                rowCount1 = (data == null ? 0 : lstMedicineADOs.Count);
                dataTotal1 = (mest.Param == null ? 0 : mest.Param.Count ?? 0);
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void FillDataToGridService(UCServiceMety uCServiceMety)
        {
            try
            {
                ServiceIdCheckByService = 0;
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
                ServiceFilter.ORDER_FIELD = "CREATE_TIME";
                ServiceFilter.ORDER_DIRECTION = "DESC";
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

                if (matyMedicines != null && matyMedicines.Count > 0)
                {
                    foreach (var itemUsername in matyMedicines)
                    {
                        var check = lstServiceADOs.FirstOrDefault(o => o.ID == itemUsername.SERVICE_ID);
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

        private void InitUCgridMedicine()
        {
            try
            {
                var culture = Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture();
                var langManager = Resources.ResourceLanguageManager.LanguageResource;

                MedicineProcessor = new UCMedicineProcessor();
                MedicineInitADO ado = new MedicineInitADO();
                ado.ListMedicineColumn = new List<MedicineColumn>();
                ado.gridViewMedicine_MouseDownMedi = gridViewMedicine_MouseDownMate;
                ado.btn_Radio_Enable_Click_Medi = btn_Radio_Enable_Click_Mate;
                ado.Check__Enable_CheckedChanged = MedicineCheckedChanged;

                MedicineColumn colRadio2 = new MedicineColumn("   ", "radioMedi", 30, true);
                colRadio2.VisibleIndex = 0;
                colRadio2.Visible = false;
                colRadio2.UnboundColumnType = DevExpress.Data.UnboundColumnType.Object;
                ado.ListMedicineColumn.Add(colRadio2);

                MedicineColumn colCheck2 = new MedicineColumn("   ", "checkMedi", 30, true);
                colCheck2.VisibleIndex = 1;
                colCheck2.Visible = false;
                colCheck2.image = imageCollectionMedicineType.Images[0];
                colCheck2.UnboundColumnType = DevExpress.Data.UnboundColumnType.Object;
                ado.ListMedicineColumn.Add(colCheck2);

                MedicineColumn colMaLoaiVatTu = new MedicineColumn(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_SERVICE_MATY__COLUMN_MEDICINE_TYPE_CODE", langManager, culture), "MEDICINE_TYPE_CODE", 60, false);
                colMaLoaiVatTu.VisibleIndex = 2;
                ado.ListMedicineColumn.Add(colMaLoaiVatTu);

                MedicineColumn colTenLoaiVatTu = new MedicineColumn(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_SERVICE_MATY__COLUMN_MEDICINE_TYPE_NAME", langManager, culture), "MEDICINE_TYPE_NAME", 100, false);
                colTenLoaiVatTu.VisibleIndex = 3;
                ado.ListMedicineColumn.Add(colTenLoaiVatTu);

                MedicineColumn colSoLuong = new MedicineColumn(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_SERVICE_MATY__COLUMN_EXPEND_AMOUNT_STR", langManager, culture), "EXPEND_AMOUNT_STR", 100, true);
                colSoLuong.VisibleIndex = 4;
                colSoLuong.Visible = false;
                colSoLuong.UnboundColumnType = DevExpress.Data.UnboundColumnType.Object;
                ado.ListMedicineColumn.Add(colSoLuong);

                MedicineColumn colSoLuongToiDa = new MedicineColumn("Số lượng tối đa BHYT chi trả", "AMOUNT_BHYT_STR", 100, true);
                colSoLuongToiDa.VisibleIndex = 5;
                colSoLuongToiDa.Visible = false;
                colSoLuongToiDa.UnboundColumnType = DevExpress.Data.UnboundColumnType.Object;
                ado.ListMedicineColumn.Add(colSoLuongToiDa);

                MedicineColumn colKhongHaoPhi = new MedicineColumn("Không hao phí", "checkExpend", 100, true);
                colKhongHaoPhi.VisibleIndex = 6;
                colKhongHaoPhi.Visible = false;
                colKhongHaoPhi.UnboundColumnType = DevExpress.Data.UnboundColumnType.Object;
                ado.ListMedicineColumn.Add(colKhongHaoPhi);

                MedicineColumn colGiaTien = new MedicineColumn(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_SERVICE_MATY__COLUMN_EXPEND_PRICE_STR", langManager, culture), "EXPEND_PRICE_STR", 100, true);
                colGiaTien.VisibleIndex = 7;
                colGiaTien.Visible = false;
                colGiaTien.UnboundColumnType = DevExpress.Data.UnboundColumnType.Object;
                ado.ListMedicineColumn.Add(colGiaTien);

                this.ucGridControlMedicine = (UserControl)MedicineProcessor.Run(ado);

                if (ucGridControlMedicine != null)
                {
                    this.panelControlMedicineType.Controls.Add(this.ucGridControlMedicine);
                    this.ucGridControlMedicine.Dock = DockStyle.Fill;
                }

            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void MedicineCheckedChanged(MedicineADO data)
        {
            try
            {
                bool success = false;
                var sources = (List<MedicineADO>)MedicineProcessor.GetDataGridView(ucGridControlMedicine);
                var itemSources = sources.FirstOrDefault(o => o.ID == data.ID);
                if (listCheckMedicineAdos != null && listCheckMedicineAdos.Count > 0)
                {
                    foreach (var item in listCheckMedicineAdos)
                    {
                        if (data.ID == item.ID)
                        {
                            listCheckMedicineAdos.FirstOrDefault(o => o.ID == itemSources.ID).checkMedi = itemSources.checkMedi;
                            success = true;
                            break;
                        }
                    }
                    if (!success)
                    {
                        listCheckMedicineAdos.Add(itemSources);
                    }
                }
                else
                {
                    listCheckMedicineAdos.Add(itemSources);
                }
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewMedicine_MouseDownMate(object sender, MouseEventArgs e)
        {
            try
            {
                if (isChooseMedicine == 2)
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
                        if (hi.Column.FieldName == "checkMedi")
                        {
                            var lstCheckAll = lstMedicineADOs;
                            List<HIS.UC.Medicine.MedicineADO> lstChecks = new List<HIS.UC.Medicine.MedicineADO>();

                            if (lstCheckAll != null && lstCheckAll.Count > 0)
                            {
                                var MedicineCheckedNum = lstMedicineADOs.Where(o => o.checkMedi == true).Count();
                                var MedicineNum = lstMedicineADOs.Count();
                                if ((MedicineCheckedNum > 0 && MedicineCheckedNum < MedicineNum) || MedicineCheckedNum == 0)
                                {
                                    isCheckAll = true;
                                    hi.Column.Image = imageCollectionMedicineType.Images[1];
                                }

                                if (MedicineCheckedNum == MedicineNum)
                                {
                                    isCheckAll = false;
                                    hi.Column.Image = imageCollectionMedicineType.Images[0];
                                }
                                if (isCheckAll)
                                {
                                    foreach (var item in lstCheckAll)
                                    {
                                        if (item.ID != null)
                                        {
                                            item.checkMedi = true;
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
                                            item.checkMedi = false;
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
                                MedicineProcessor.Reload(ucGridControlMedicine, lstChecks);
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

        private void btn_Radio_Enable_Click_Mate(V_HIS_MEDICINE_TYPE data)
        {
            try
            {
                WaitingManager.Show();
                CommonParam param = new CommonParam();
                MOS.Filter.HisServiceMetyViewFilter matyServiceFilter = new HisServiceMetyViewFilter();
                matyServiceFilter.MEDICINE_TYPE_ID = data.ID;
                MedicineIdCheck = data.ID;

                matyMedicines = new BackendAdapter(param).Get<List<V_HIS_SERVICE_METY>>(
                    "api/HisServiceMety/GetView",
                   ApiConsumers.MosConsumer,
                   matyServiceFilter,
                   param);
                lstServiceADOs = new List<HIS.UC.Service.ServiceADO>();

                lstServiceADOs = (from r in listService select new ServiceADO(r)).ToList();
                if (matyMedicines != null && matyMedicines.Count > 0)
                {
                    foreach (var itemUsername in matyMedicines)
                    {
                        var check = lstServiceADOs.FirstOrDefault(o => o.ID == itemUsername.SERVICE_ID);
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
                colCheck1.image = imageCollectionService.Images[0];
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
                                    hi.Column.Image = imageCollectionService.Images[1];
                                }

                                if (ServiceCheckedNum == ServiceNum)
                                {
                                    isCheckAll = false;
                                    hi.Column.Image = imageCollectionService.Images[0];
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
                MOS.Filter.HisServiceMetyViewFilter matyServiceFilter = new HisServiceMetyViewFilter();
                matyServiceFilter.SERVICE_ID = data.ID;
                ServiceIdCheckByService = data.ID;
                ServiceUnitId = data.SERVICE_UNIT_ID;

                matyServices = new BackendAdapter(param).Get<List<V_HIS_SERVICE_METY>>(
                                "api/HisServiceMety/GetView",
                                HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer,
                                matyServiceFilter,
                                param);
                lstMedicineADOs = new List<HIS.UC.Medicine.MedicineADO>();
                lstMedicineADOs = (from r in listMedicine select new MedicineADO(r)).ToList();
                if (matyServices != null && matyServices.Count > 0)
                {
                    foreach (var itemUsername in matyServices)
                    {
                        var check = lstMedicineADOs.FirstOrDefault(o => o.ID == itemUsername.MEDICINE_TYPE_ID);
                        if (check != null)
                        {
                            check.checkMedi = true;
                            check.checkExpend = itemUsername.IS_NOT_EXPEND == 1;
                            check.AMOUNT_BHYT_STR = itemUsername.AMOUNT_BHYT;
                            check.EXPEND_PRICE_STR = itemUsername.EXPEND_PRICE;
                            check.EXPEND_AMOUNT_STR = itemUsername.EXPEND_AMOUNT;
                        }
                    }
                }

                lstMedicineADOs = lstMedicineADOs.OrderByDescending(p => p.checkMedi).ToList();
                if (ucGridControlMedicine != null)
                {
                    MedicineProcessor.Reload(ucGridControlMedicine, lstMedicineADOs);
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
                FillDataToGridMedicine(this);
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
                ServiceIdCheckByService = 0;
                ServiceUnitId = 0;
                MedicineIdCheck = 0;
                checkRa = false;
                isChooseMedicine = 0;
                isChooseService = 0;
                txtKeyword1.Text = null;
                txtKeyword2.Text = null;
                listCheckServiceAdos = new List<ServiceADO>();
                listCheckMedicineAdos = new List<MedicineADO>();
                matyServices = new List<V_HIS_SERVICE_METY>();
                matyMedicines = new List<V_HIS_SERVICE_METY>();
                FillDataToGridMedicine(this);
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
                if (ucGridControlMedicine != null && ucGridControlService != null)
                {
                    object Medicine = MedicineProcessor.GetDataGridView(ucGridControlMedicine);
                    object Service = ServiceProcessor.GetDataGridView(ucGridControlService);
                    bool success = false;
                    CommonParam param = new CommonParam();
                    if (isChooseService == 1)
                    {
                        if (ServiceIdCheckByService == 0)
                        {
                            WaitingManager.Hide();
                            DevExpress.XtraEditors.XtraMessageBox.Show("Chưa chọn dịch vụ", "Thông báo");
                            return;
                        }

                        if (Medicine is List<HIS.UC.Medicine.MedicineADO>)
                        {
                            this.lstMedicineADOs = (List<HIS.UC.Medicine.MedicineADO>)Medicine;

                            if (this.lstMedicineADOs != null && this.lstMedicineADOs.Count > 0 && checkRa == true)
                            {
                                //Danh sach cac user duoc check

                                var dataCheckeds = this.lstMedicineADOs.Where(p => p.checkMedi == true).ToList();

                                //List xoa

                                var dataDeletes = this.lstMedicineADOs.Where(o => matyServices.Select(p => p.MEDICINE_TYPE_ID)
                                    .Contains(o.ID) && o.checkMedi == false).ToList();

                                //list them
                                var dataCreates = dataCheckeds.Where(o => !matyServices.Select(p => p.MEDICINE_TYPE_ID)
                                    .Contains(o.ID)).ToList();
                                //List update
                                var dataUpdate = dataCheckeds.Where(o => matyServices.Select(p => p.MEDICINE_TYPE_ID)
                                   .Contains(o.ID)).ToList();

                                if (dataCheckeds.Count == 0 && dataDeletes.Count == 0)
                                {
                                    WaitingManager.Hide();
                                    DevExpress.XtraEditors.XtraMessageBox.Show("Chưa chọn loại thuốc", "Thông báo");
                                    return;
                                }

                                if (dataUpdate.Exists(o => o.EXPEND_AMOUNT_STR <= 0) || dataCreates.Exists(o => o.EXPEND_AMOUNT_STR <= 0))
                                {
                                    WaitingManager.Hide();
                                    DevExpress.XtraEditors.XtraMessageBox.Show("Số lượng phải lớn hơn không", "Thông báo");
                                    throw new Exception("Số lượng sai");
                                }
                                //if (dataCheckeds != null)
                                //{
                                //    success = true;
                                //}

                                //xử lý update
                                if (dataUpdate != null && dataUpdate.Count > 0)
                                {
                                    success = UpdateMedicineProcess(dataUpdate, param);
                                }


                                //xử lý delete
                                if (dataDeletes != null && dataDeletes.Count > 0)
                                {
                                    success = DeleteMedicineProcess(dataDeletes, param);
                                }

                                //xử lý thêm
                                if (dataCreates != null && dataCreates.Count > 0)
                                {
                                    success = CreateMedicineProcess(dataCreates, param);
                                }


                                this.lstMedicineADOs = this.lstMedicineADOs.OrderByDescending(p => p.checkMedi).ToList();
                                if (ucGridControlMedicine != null)
                                {
                                    MedicineProcessor.Reload(ucGridControlMedicine, this.lstMedicineADOs);
                                }
                            }
                        }
                    }
                    if (isChooseMedicine == 2)
                    {
                        if (MedicineIdCheck == 0)
                        {
                            WaitingManager.Hide();
                            DevExpress.XtraEditors.XtraMessageBox.Show("Chưa chọn thuốc", "Thông báo");
                            return;
                        }
                        if (Service is List<HIS.UC.Service.ServiceADO>)
                        {
                            this.lstServiceADOs = (List<HIS.UC.Service.ServiceADO>)Service;

                            if (this.lstServiceADOs != null && this.lstServiceADOs.Count > 0)
                            {
                                //bool success = false;
                                HIS.UC.Medicine.MedicineADO MedicineType = this.lstMedicineADOs.FirstOrDefault(o => o.ID == MedicineIdCheck);
                                //Danh sach cac user duoc check

                                var dataChecked = this.lstServiceADOs.Where(p => p.checkService == true).ToList();


                                //List xoa

                                var dataDelete = this.lstServiceADOs.Where(o => matyMedicines.Select(p => p.SERVICE_ID)
                                    .Contains(o.ID) && o.checkService == false).ToList();

                                //list them
                                var dataCreate = dataChecked.Where(o => !matyMedicines.Select(p => p.SERVICE_ID)
                                    .Contains(o.ID)).ToList();
                                //list update
                                var dataUpdate = dataChecked.Where(o => matyMedicines.Select(p => p.SERVICE_ID)
                                   .Contains(o.ID)).ToList();

                                if (dataChecked.Count == 0 && dataDelete.Count == 0)
                                {
                                    WaitingManager.Hide();
                                    DevExpress.XtraEditors.XtraMessageBox.Show("Chưa chọn dịch vụ", "Thông báo");
                                    return;
                                }

                                if (MedicineType != null && MedicineType.EXPEND_AMOUNT_STR <= 0)
                                {
                                    WaitingManager.Hide();
                                    DevExpress.XtraEditors.XtraMessageBox.Show("Số lượng phải lớn hơn không", "Thông báo");
                                    throw new Exception("Số lượng sai");
                                }
                                //if (dataChecked != null)
                                //{
                                //    success = true;
                                //}

                                //xử lý update
                                if (dataUpdate != null && dataUpdate.Count > 0 && MedicineType != null)
                                {
                                    success = UpdateServiceProcess(dataUpdate, MedicineType, param);
                                }

                                //xử lý delete
                                if (dataDelete != null && dataDelete.Count > 0)
                                {
                                    success = DeleteServiceProcess(dataDelete, param);
                                }

                                //xử lý Create
                                if (dataCreate != null && dataCreate.Count > 0 && MedicineType != null)
                                {
                                    success = CreateServiceProcess(dataCreate, MedicineType, param);
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

        private bool CreateServiceProcess(List<ServiceADO> dataCreate, MedicineADO MedicineType, CommonParam param)
        {
            bool success = false;
            if (dataCreate != null && dataCreate.Count > 0 && MedicineType != null)
            {
                List<V_HIS_SERVICE_METY> mestServiceCreate = new List<V_HIS_SERVICE_METY>();
                foreach (var item in dataCreate)
                {
                    V_HIS_SERVICE_METY mestService = new V_HIS_SERVICE_METY();
                    mestService.SERVICE_ID = item.ID;
                    mestService.MEDICINE_TYPE_ID = MedicineIdCheck;
                    mestService.AMOUNT_BHYT = MedicineType.AMOUNT_BHYT_STR;
                    mestService.IS_NOT_EXPEND = MedicineType.checkExpend == true ? (short?)1 : null;
                    mestService.EXPEND_AMOUNT = MedicineType.EXPEND_AMOUNT_STR;
                    mestService.EXPEND_PRICE = MedicineType.EXPEND_PRICE_STR;
                    mestService.SERVICE_UNIT_ID = item.SERVICE_UNIT_ID;
                    mestServiceCreate.Add(mestService);
                }

                var createResult = new BackendAdapter(param).Post<List<HIS_SERVICE_METY>>(
                           "api/HisServiceMety/CreateList",
                           HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer,
                           mestServiceCreate,
                           param);
                if (createResult != null && createResult.Count > 0)
                {
                    success = true;
                    AutoMapper.Mapper.CreateMap<HIS_SERVICE_METY, V_HIS_SERVICE_METY>();
                    var vCreateResults = AutoMapper.Mapper.Map<List<HIS_SERVICE_METY>, List<V_HIS_SERVICE_METY>>(createResult);
                    matyMedicines.AddRange(vCreateResults);
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
                List<long> deleteId = matyMedicines.Where(o => dataDelete.Select(p => p.ID)
                    .Contains(o.SERVICE_ID)).Select(o => o.ID).ToList();
                bool deleteResult = new BackendAdapter(param).Post<bool>(
                          "api/HisServiceMety/DeleteList",
                          HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer,
                          deleteId,
                          param);
                if (deleteResult)
                {
                    success = true;
                    matyMedicines = matyMedicines.Where(o => !deleteId.Contains(o.ID)).ToList();
                }
            }
            return success;
        }

        //Hàm xử lý update Service
        private bool UpdateServiceProcess(List<ServiceADO> dataUpdate, MedicineADO MedicineType, CommonParam param)
        {
            bool success = false;
            if (dataUpdate != null && dataUpdate.Count > 0 && MedicineType != null)
            {
                var MedicineMetyUpdates = new List<V_HIS_SERVICE_METY>();
                foreach (var item in dataUpdate)
                {
                    var MedicineMaty = matyMedicines.FirstOrDefault(o => o.MEDICINE_TYPE_ID == MedicineIdCheck && o.SERVICE_ID == item.ID);
                    if (MedicineMaty != null)
                    {
                        MedicineMaty.EXPEND_AMOUNT = MedicineType.EXPEND_AMOUNT_STR;
                        MedicineMaty.EXPEND_PRICE = MedicineType.EXPEND_PRICE_STR;
                        MedicineMaty.AMOUNT_BHYT = MedicineType.AMOUNT_BHYT_STR;
                        MedicineMaty.IS_NOT_EXPEND = MedicineType.checkExpend == true ? (short?)1 : 0;
                        //MedicineMety.SERVICE_UNIT_ID = item.SERVICE_UNIT_ID;
                        //MedicineMety.MEDICINE_TYPE_ID = MedicineIdCheck;
                        MedicineMetyUpdates.Add(MedicineMaty);
                    }
                }
                if (MedicineMetyUpdates != null && MedicineMetyUpdates.Count > 0)
                {
                    var updateResult = new BackendAdapter(param).Post<List<HIS_SERVICE_METY>>(
                               "/api/HisServiceMety/UpdateList",
                               HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer,
                               MedicineMetyUpdates,
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
        private bool CreateMedicineProcess(List<MedicineADO> dataCreates, CommonParam param)
        {
            bool success = false;
            if (dataCreates != null && dataCreates.Count > 0)
            {
                List<V_HIS_SERVICE_METY> MestServiceCreates = new List<V_HIS_SERVICE_METY>();
                foreach (var item in dataCreates)
                {
                    V_HIS_SERVICE_METY mestService = new V_HIS_SERVICE_METY();
                    mestService.MEDICINE_TYPE_ID = item.ID;
                    mestService.SERVICE_UNIT_ID = ServiceUnitId;
                    mestService.SERVICE_ID = ServiceIdCheckByService;
                    mestService.AMOUNT_BHYT = item.AMOUNT_BHYT_STR;
                    mestService.IS_NOT_EXPEND = item.checkExpend == true ? (short?)1 : 0;
                    mestService.EXPEND_AMOUNT = item.EXPEND_AMOUNT_STR;
                    mestService.EXPEND_PRICE = item.EXPEND_PRICE_STR;
                    MestServiceCreates.Add(mestService);
                }

                var createResult = new BackendAdapter(param).Post<List<HIS_SERVICE_METY>>(
                           "api/HisServiceMety/CreateList",
                           HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer,
                           MestServiceCreates,
                           param);
                if (createResult != null && createResult.Count > 0)
                {
                    success = true;
                    AutoMapper.Mapper.CreateMap<HIS_SERVICE_METY, V_HIS_SERVICE_METY>();
                    var vCreateResults = AutoMapper.Mapper.Map<List<HIS_SERVICE_METY>, List<V_HIS_SERVICE_METY>>(createResult);
                    matyServices.AddRange(vCreateResults);
                }
            }
            return success;
        }

        //Hàm xóa Medicine
        private bool DeleteMedicineProcess(List<MedicineADO> dataDeletes, CommonParam param)
        {
            bool success = false;
            if (dataDeletes != null && dataDeletes.Count > 0)
            {
                List<long> deleteIds = matyServices.Where(o => dataDeletes.Select(p => p.ID)
                    .Contains(o.MEDICINE_TYPE_ID)).Select(o => o.ID).ToList();
                bool deleteResult = new BackendAdapter(param).Post<bool>(
                          "api/HisServiceMety/DeleteList",
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

        //Hàm update Medicine
        private bool UpdateMedicineProcess(List<MedicineADO> dataUpdate, CommonParam param)
        {

            bool success = false;
            var ServiceMetyUpdates = new List<V_HIS_SERVICE_METY>();
            foreach (var item in dataUpdate)
            {
                var ServiceMety = matyServices.FirstOrDefault(o => o.MEDICINE_TYPE_ID == item.ID && o.SERVICE_ID == ServiceIdCheckByService);
                if (ServiceMety != null)
                {
                    ServiceMety.EXPEND_AMOUNT = item.EXPEND_AMOUNT_STR;
                    ServiceMety.EXPEND_PRICE = item.EXPEND_PRICE_STR;
                    ServiceMetyUpdates.Add(ServiceMety);
                }
            }
            if (ServiceMetyUpdates != null && ServiceMetyUpdates.Count > 0)
            {
                var updateResult = new BackendAdapter(param).Post<List<HIS_SERVICE_METY>>(
                           "/api/HisServiceMety/UpdateList",
                           HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer,
                           ServiceMetyUpdates,
                           param);
                if (updateResult != null && updateResult.Count > 0)
                {
                    //listMediStockMety.AddRange(updateResult);
                    success = true;
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
                    FillDataToGridMedicine(this);
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
                Resources.ResourceLanguageManager.LanguageResource = new System.Resources.ResourceManager("HIS.Desktop.Plugins.ServiceMety.Resources.Lang", typeof(HIS.Desktop.Plugins.ServiceMety.UCServiceMety).Assembly);
                //Gán giá trị cho các control
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
