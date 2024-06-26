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
using HIS.Desktop.Plugins.ServiceMaty.entity;
using HIS.UC.Service;
using HIS.UC.Service.ADO;
using Inventec.Core;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using HIS.UC.Material;
using HIS.UC.Material.ADO;
using Inventec.Common.Controls.EditorLoader;
using Inventec.Common.Adapter;
using HIS.Desktop.ApiConsumer;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using DevExpress.XtraEditors;
using HIS.Desktop.LocalStorage.ConfigApplication;
using System.Resources;
using Inventec.Desktop.Common.LanguageManager;
using HIS.Desktop.LocalStorage.BackendData;

namespace HIS.Desktop.Plugins.ServiceMaty
{
    public partial class UCServiceMaty : HIS.Desktop.Utility.UserControlBase
    {
        List<HIS_SERVICE> Service { get; set; }
        UCServiceProcessor ServiceProcessor;
        UCMaterialProcessor MaterialProcessor;
        UserControl ucGridControlService;
        UserControl ucGridControlMaterial;
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
        List<MaterialADO> listCheckMaterialAdos = new List<MaterialADO>();
        internal List<HIS.UC.Service.ServiceADO> lstServiceADOs { get; set; }
        internal List<HIS.UC.Material.MaterialADO> lstMaterialADOs { get; set; }
        List<V_HIS_SERVICE> listService;
        List<V_HIS_MATERIAL_TYPE> listMaterial;
        long ServiceIdCheckByService = 0;
        long ServiceUnitId = 0;
        long MaterialIdCheck = 0;
        long isChooseService;
        long isChooseMaterial;
        bool isCheckAll;
        bool statecheckColumn;
        List<V_HIS_SERVICE_MATY> matyServices { get; set; }
        List<V_HIS_SERVICE_MATY> matyMaterials { get; set; }

        public UCServiceMaty(Inventec.Desktop.Common.Modules.Module _moduleData)
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

        private void UCServiceMaty_Load(object sender, EventArgs e)
        {
            try
            {
                WaitingManager.Show();
                SetCaptionByLanguageKey();
                LoadComboStatus();
                InitComboServiceType();
                InitUCgridService();
                InitUCgridMaterial();
                FillDataToGridService(this);
                FillDataToGridMaterial(this);
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
                status.Add(new Status(2, "Vật tư"));

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

        private void InitComboServiceType()
        {
            try
            {
                var data = BackendDataWorker.Get<HIS_SERVICE_TYPE>();
                //.Where(o =>
                //o.ID != IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__THUOC
                //&& o.ID != IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__VT
                //&& o.ID != IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__MAU
                //).ToList();
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("SERVICE_TYPE_CODE", "", 100, 1, true));
                columnInfos.Add(new ColumnInfo("SERVICE_TYPE_NAME", "", 250, 2, true));
                ControlEditorADO controlEditorADO = new ControlEditorADO("SERVICE_TYPE_NAME", "ID", columnInfos, false, 350);
                ControlEditorLoader.Load(cboServiceType, data, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void FillDataToGridMaterial(UCServiceMaty uCServiceMaty)
        {
            try
            {
                MaterialIdCheck = 0;
                int numPageSize;
                if (ucPaging2.pagingGrid != null)
                {
                    numPageSize = ucPaging2.pagingGrid.PageSize;
                }
                else
                {
                    numPageSize = (int)ConfigApplications.NumPageSize;
                }

                FillDataToGridMaterialPage(new CommonParam(0, numPageSize));

                CommonParam param = new CommonParam();
                param.Limit = rowCount1;
                param.Count = dataTotal1;
                ucPaging2.Init(FillDataToGridMaterialPage, param, numPageSize);
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void FillDataToGridMaterialPage(object data)
        {
            try
            {
                WaitingManager.Show();
                listMaterial = new List<V_HIS_MATERIAL_TYPE>();
                int start1 = ((CommonParam)data).Start ?? 0;
                int limit1 = ((CommonParam)data).Limit ?? 0;
                CommonParam param = new CommonParam(start1, limit1);
                MOS.Filter.HisMaterialTypeFilter MaterialFilter = new HisMaterialTypeFilter();
                MaterialFilter.ORDER_FIELD = "CREATE_TIME";
                MaterialFilter.ORDER_DIRECTION = "DESC";
                MaterialFilter.KEY_WORD = txtKeyword2.Text;

                if ((long)cboChoose.EditValue == 2)
                {
                    isChooseMaterial = (long)cboChoose.EditValue;
                }

                var mest = new Inventec.Common.Adapter.BackendAdapter(param).GetRO<List<MOS.EFMODEL.DataModels.V_HIS_MATERIAL_TYPE>>(
                    HIS.Desktop.ApiConsumer.HisRequestUriStore.HIS_MATERIAL_TYPE_GETVIEW,
                    HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer,
                    MaterialFilter,
                    param);

                lstMaterialADOs = new List<MaterialADO>();
                if (mest != null && mest.Data.Count > 0)
                {
                    listMaterial = mest.Data;
                    foreach (var item in listMaterial)
                    {
                        MaterialADO MaterialADO = new MaterialADO(item);
                        if (isChooseMaterial == 2)
                        {
                            MaterialADO.isKeyChooseMate = true;
                        }
                        lstMaterialADOs.Add(MaterialADO);
                    }
                }

                if (matyServices != null && matyServices.Count > 0)
                {
                    foreach (var itemUsername in matyServices)
                    {
                        var check = lstMaterialADOs.FirstOrDefault(o => o.ID == itemUsername.MATERIAL_TYPE_ID);
                        if (check != null)
                        {
                            check.checkMate = true;
                            check.checkExpend = itemUsername.IS_NOT_EXPEND == 1;
                            check.AMOUNT_BHYT_STR = itemUsername.AMOUNT_BHYT;
                            check.EXPEND_AMOUNT_STR = itemUsername.EXPEND_AMOUNT;
                            check.EXPEND_PRICE_STR = itemUsername.EXPEND_PRICE;
                        }
                    }
                }
                lstMaterialADOs = lstMaterialADOs.OrderByDescending(p => p.checkMate).ToList();

                if (MaterialIdCheck != 0 && isChooseMaterial == 2)
                {
                    var radioMaterial = lstMaterialADOs.Where(o => o.ID == MaterialIdCheck).FirstOrDefault();
                    if (radioMaterial != null)
                    {
                        radioMaterial.radioMate = true;
                    }
                }
                lstMaterialADOs = lstMaterialADOs.OrderByDescending(p => p.radioMate).ToList();

                if (listCheckMaterialAdos != null && listCheckMaterialAdos.Count > 0)
                {
                    foreach (var item in listCheckMaterialAdos)
                    {
                        var check = lstMaterialADOs.FirstOrDefault(o => o.ID == item.ID);
                        if (check != null)
                        {
                            lstMaterialADOs.FirstOrDefault(o => o.ID == item.ID).checkMate = item.checkMate;
                            lstMaterialADOs.FirstOrDefault(o => o.ID == item.ID).AMOUNT_BHYT_STR = item.AMOUNT_BHYT_STR;
                            lstMaterialADOs.FirstOrDefault(o => o.ID == item.ID).checkExpend = item.checkExpend;
                            lstMaterialADOs.FirstOrDefault(o => o.ID == item.ID).EXPEND_AMOUNT_STR = item.EXPEND_AMOUNT_STR;
                            lstMaterialADOs.FirstOrDefault(o => o.ID == item.ID).EXPEND_PRICE_STR = item.EXPEND_PRICE_STR;
                        }
                    }
                }

                if (ucGridControlMaterial != null)
                {
                    MaterialProcessor.Reload(ucGridControlMaterial, lstMaterialADOs);
                }
                rowCount1 = (data == null ? 0 : lstMaterialADOs.Count);
                dataTotal1 = (mest.Param == null ? 0 : mest.Param.Count ?? 0);
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void FillDataToGridService(UCServiceMaty uCServiceMaty)
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

                if (cboServiceType.EditValue != null)
                {
                    ServiceFilter.SERVICE_TYPE_ID = Inventec.Common.TypeConvert.Parse.ToInt64(cboServiceType.EditValue.ToString());
                }

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

                if (matyMaterials != null && matyMaterials.Count > 0)
                {
                    foreach (var itemUsername in matyMaterials)
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

        private void InitUCgridMaterial()
        {
            try
            {
                var culture = Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture();
                var langManager = Resources.ResourceLanguageManager.LanguageResource;

                MaterialProcessor = new UCMaterialProcessor();
                MaterialInitADO ado = new MaterialInitADO();
                ado.ListMaterialColumn = new List<MaterialColumn>();
                ado.gridViewMaterial_MouseDownMate = gridViewMaterial_MouseDownMate;
                ado.btn_Radio_Enable_Click_Mate = btn_Radio_Enable_Click_Mate;
                ado.Check__Enable_CheckedChanged = MaterialCheckedChanged;

                MaterialColumn colRadio2 = new MaterialColumn("   ", "radioMate", 30, true);
                colRadio2.VisibleIndex = 0;
                colRadio2.Visible = false;
                colRadio2.UnboundColumnType = DevExpress.Data.UnboundColumnType.Object;
                ado.ListMaterialColumn.Add(colRadio2);

                MaterialColumn colCheck2 = new MaterialColumn("   ", "checkMate", 30, true);
                colCheck2.VisibleIndex = 1;
                colCheck2.Visible = false;
                colCheck2.image = imageCollectionMaterialType.Images[0];
                colCheck2.UnboundColumnType = DevExpress.Data.UnboundColumnType.Object;
                ado.ListMaterialColumn.Add(colCheck2);

                MaterialColumn colMaLoaiVatTu = new MaterialColumn(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_SERVICE_MATY__COLUMN_MATERIAL_TYPE_CODE", langManager, culture), "MATERIAL_TYPE_CODE", 60, false);
                colMaLoaiVatTu.VisibleIndex = 2;
                ado.ListMaterialColumn.Add(colMaLoaiVatTu);

                MaterialColumn colTenLoaiVatTu = new MaterialColumn(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_SERVICE_MATY__COLUMN_MATERIAL_TYPE_NAME", langManager, culture), "MATERIAL_TYPE_NAME", 100, false);
                colTenLoaiVatTu.VisibleIndex = 3;
                ado.ListMaterialColumn.Add(colTenLoaiVatTu);

                MaterialColumn colSoLuong = new MaterialColumn(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_SERVICE_MATY__COLUMN_EXPEND_AMOUNT_STR", langManager, culture), "EXPEND_AMOUNT_STR", 100, true);
                colSoLuong.VisibleIndex = 4;
                colSoLuong.Visible = false;
                colSoLuong.UnboundColumnType = DevExpress.Data.UnboundColumnType.Object;
                ado.ListMaterialColumn.Add(colSoLuong);

                MaterialColumn colSoLuongToiDa = new MaterialColumn("Số lượng tối đa BHYT chi trả", "AMOUNT_BHYT_STR", 100, true);
                colSoLuongToiDa.VisibleIndex = 5;
                colSoLuongToiDa.Visible = false;
                colSoLuongToiDa.UnboundColumnType = DevExpress.Data.UnboundColumnType.Object;
                ado.ListMaterialColumn.Add(colSoLuongToiDa);

                MaterialColumn colKhongHaoPhi = new MaterialColumn("Không hao phí", "checkExpend", 100, true);
                colKhongHaoPhi.VisibleIndex = 6;
                colKhongHaoPhi.Visible = false;
                colKhongHaoPhi.UnboundColumnType = DevExpress.Data.UnboundColumnType.Object;
                ado.ListMaterialColumn.Add(colKhongHaoPhi);

                MaterialColumn colGiaTien = new MaterialColumn(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_SERVICE_MATY__COLUMN_EXPEND_PRICE_STR", langManager, culture), "EXPEND_PRICE_STR", 100, true);
                colGiaTien.VisibleIndex = 7;
                colGiaTien.Visible = false;
                colGiaTien.UnboundColumnType = DevExpress.Data.UnboundColumnType.Object;
                ado.ListMaterialColumn.Add(colGiaTien);

                this.ucGridControlMaterial = (UserControl)MaterialProcessor.Run(ado);

                if (ucGridControlMaterial != null)
                {
                    this.panelControlMaterialtType.Controls.Add(this.ucGridControlMaterial);
                    this.ucGridControlMaterial.Dock = DockStyle.Fill;
                }

            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void MaterialCheckedChanged(MaterialADO data)
        {
            try
            {
                bool success = false;
                var sources = (List<MaterialADO>)MaterialProcessor.GetDataGridView(ucGridControlMaterial);
                var itemSources = sources.FirstOrDefault(o => o.ID == data.ID);
                if (listCheckMaterialAdos != null && listCheckMaterialAdos.Count > 0)
                {
                    foreach (var item in listCheckMaterialAdos)
                    {
                        if (data.ID == item.ID)
                        {
                            listCheckMaterialAdos.FirstOrDefault(o => o.ID == itemSources.ID).checkMate = itemSources.checkMate;
                            success = true;
                            break;
                        }
                    }
                    if (!success)
                    {
                        listCheckMaterialAdos.Add(itemSources);
                    }
                }
                else
                {
                    listCheckMaterialAdos.Add(itemSources);
                }
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewMaterial_MouseDownMate(object sender, MouseEventArgs e)
        {
            try
            {
                if (isChooseMaterial == 2)
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
                        if (hi.Column.FieldName == "checkMate")
                        {
                            var lstCheckAll = lstMaterialADOs;
                            List<HIS.UC.Material.MaterialADO> lstChecks = new List<HIS.UC.Material.MaterialADO>();

                            if (lstCheckAll != null && lstCheckAll.Count > 0)
                            {
                                var MaterialCheckedNum = lstMaterialADOs.Where(o => o.checkMate == true).Count();
                                var MaterialNum = lstMaterialADOs.Count();
                                if ((MaterialCheckedNum > 0 && MaterialCheckedNum < MaterialNum) || MaterialCheckedNum == 0)
                                {
                                    isCheckAll = true;
                                    hi.Column.Image = imageCollectionMaterialType.Images[1];
                                }

                                if (MaterialCheckedNum == MaterialNum)
                                {
                                    isCheckAll = false;
                                    hi.Column.Image = imageCollectionMaterialType.Images[0];
                                }
                                if (isCheckAll)
                                {
                                    foreach (var item in lstCheckAll)
                                    {
                                        if (item.ID != null)
                                        {
                                            item.checkMate = true;
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
                                            item.checkMate = false;
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
                                MaterialProcessor.Reload(ucGridControlMaterial, lstChecks);
                                //??

                            }
                        }
                        //if (hi.Column.FieldName == "checkExpend")
                        //{
                        //    var lstCheckAll = lstMaterialADOs;
                        //    List<HIS.UC.Material.MaterialADO> lstChecks = new List<HIS.UC.Material.MaterialADO>();

                        //    if (lstCheckAll != null && lstCheckAll.Count > 0)
                        //    {
                        //        var MaterialCheckedNum = lstMaterialADOs.Where(o => o.checkExpend == true).Count();
                        //        var MaterialNum = lstMaterialADOs.Count();
                        //        if ((MaterialCheckedNum > 0 && MaterialCheckedNum < MaterialNum) || MaterialCheckedNum == 0)
                        //        {
                        //            isCheckAll = true;
                        //            hi.Column.Image = imageCollectionMaterialType.Images[1];
                        //        }

                        //        if (MaterialCheckedNum == MaterialNum)
                        //        {
                        //            isCheckAll = false;
                        //            hi.Column.Image = imageCollectionMaterialType.Images[0];
                        //        }
                        //        if (isCheckAll)
                        //        {
                        //            foreach (var item in lstCheckAll)
                        //            {
                        //                if (item.ID != null)
                        //                {
                        //                    item.checkExpend = true;
                        //                    lstChecks.Add(item);
                        //                }
                        //                else
                        //                {
                        //                    lstChecks.Add(item);
                        //                }
                        //            }
                        //            isCheckAll = false;
                        //        }
                        //        else
                        //        {
                        //            foreach (var item in lstCheckAll)
                        //            {
                        //                if (item.ID != null)
                        //                {
                        //                    item.checkExpend = false;
                        //                    lstChecks.Add(item);
                        //                }
                        //                else
                        //                {
                        //                    lstChecks.Add(item);
                        //                }
                        //            }
                        //            isCheckAll = true;
                        //        }

                        //        //ReloadData
                        //        MaterialProcessor.Reload(ucGridControlMaterial, lstChecks);
                        //        //??

                        //    }
                        //}
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

        private void btn_Radio_Enable_Click_Mate(V_HIS_MATERIAL_TYPE data)
        {
            try
            {
                WaitingManager.Show();
                CommonParam param = new CommonParam();
                MOS.Filter.HisServiceMatyViewFilter matyServiceFilter = new HisServiceMatyViewFilter();
                matyServiceFilter.MATERIAL_TYPE_ID = data.ID;
                MaterialIdCheck = data.ID;

                matyMaterials = new BackendAdapter(param).Get<List<V_HIS_SERVICE_MATY>>(
                    "api/HisServiceMaty/GetView",
                   ApiConsumers.MosConsumer,
                   matyServiceFilter,
                   param);
                lstServiceADOs = new List<HIS.UC.Service.ServiceADO>();

                lstServiceADOs = (from r in listService select new ServiceADO(r)).ToList();
                if (matyMaterials != null && matyMaterials.Count > 0)
                {
                    foreach (var itemUsername in matyMaterials)
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
                MOS.Filter.HisServiceMatyViewFilter matyServiceFilter = new HisServiceMatyViewFilter();
                matyServiceFilter.SERVICE_ID = data.ID;
                ServiceIdCheckByService = data.ID;
                ServiceUnitId = data.SERVICE_UNIT_ID;

                matyServices = new BackendAdapter(param).Get<List<V_HIS_SERVICE_MATY>>(
                                "api/HisServiceMaty/GetView",
                                HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer,
                                matyServiceFilter,
                                param);
                lstMaterialADOs = new List<HIS.UC.Material.MaterialADO>();
                lstMaterialADOs = (from r in listMaterial select new MaterialADO(r)).ToList();
                if (matyServices != null && matyServices.Count > 0)
                {
                    foreach (var itemUsername in matyServices)
                    {
                        var check = lstMaterialADOs.FirstOrDefault(o => o.ID == itemUsername.MATERIAL_TYPE_ID);
                        if (check != null)
                        {
                            check.checkMate = true;
                            check.checkExpend = itemUsername.IS_NOT_EXPEND == 1;
                            check.AMOUNT_BHYT_STR = itemUsername.AMOUNT_BHYT;
                            check.EXPEND_PRICE_STR = itemUsername.EXPEND_PRICE;
                            check.EXPEND_AMOUNT_STR = itemUsername.EXPEND_AMOUNT;
                        }
                    }
                }

                lstMaterialADOs = lstMaterialADOs.OrderByDescending(p => p.checkMate).ToList();
                if (ucGridControlMaterial != null)
                {
                    MaterialProcessor.Reload(ucGridControlMaterial, lstMaterialADOs);
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
                FillDataToGridMaterial(this);
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
                MaterialIdCheck = 0;
                checkRa = false;
                isChooseMaterial = 0;
                isChooseService = 0;
                txtKeyword1.Text = null;
                txtKeyword2.Text = null;
                listCheckServiceAdos = new List<ServiceADO>();
                listCheckMaterialAdos = new List<MaterialADO>();
                matyServices = new List<V_HIS_SERVICE_MATY>();
                matyMaterials = new List<V_HIS_SERVICE_MATY>();
                FillDataToGridMaterial(this);
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
                if (ucGridControlMaterial != null && ucGridControlService != null)
                {
                    object Material = MaterialProcessor.GetDataGridView(ucGridControlMaterial);
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

                        if (Material is List<HIS.UC.Material.MaterialADO>)
                        {
                            this.lstMaterialADOs = (List<HIS.UC.Material.MaterialADO>)Material;

                            if (this.lstMaterialADOs != null && this.lstMaterialADOs.Count > 0 && checkRa == true)
                            {
                                //Danh sach cac user duoc check

                                var dataCheckeds = this.lstMaterialADOs.Where(p => p.checkMate == true).ToList();

                                //List xoa
                                var dataDeletes = this.lstMaterialADOs.Where(o => matyServices.Select(p => p.MATERIAL_TYPE_ID).Contains(o.ID) && o.checkMate == false).ToList();

                                //list them
                                var dataCreates = dataCheckeds.Where(o => !matyServices.Select(p => p.MATERIAL_TYPE_ID)
                                    .Contains(o.ID)).ToList();
                                //List update
                                var dataUpdate = dataCheckeds.Where(o => matyServices.Select(p => p.MATERIAL_TYPE_ID)
                                   .Contains(o.ID)).ToList();

                                if (dataCheckeds.Count == 0 && dataDeletes.Count == 0)
                                {
                                    WaitingManager.Hide();
                                    DevExpress.XtraEditors.XtraMessageBox.Show("Chưa chọn loại vật tư", "Thông báo");

                                    return;
                                }
                                //if (dataCheckeds != null)
                                //{
                                //    success = true;
                                //}

                                if (dataUpdate.Exists(o => o.EXPEND_AMOUNT_STR <= 0) || dataCreates.Exists(o => o.EXPEND_AMOUNT_STR <= 0))
                                {
                                    WaitingManager.Hide();
                                    DevExpress.XtraEditors.XtraMessageBox.Show("Số lượng phải lớn hơn không", "Thông báo");
                                    throw new Exception("Số lượng sai");
                                }
                                List<bool> lstSuccess = new List<bool>();
                                //xử lý update
                                if (dataUpdate != null && dataUpdate.Count() > 0)
                                {
                                    var successReturn = UpdateMaterialProcess(dataUpdate, param);
                                    lstSuccess.Add(successReturn);
                                }
                                //xử lý delete
                                if (dataDeletes != null && dataDeletes.Count() > 0)
                                {
                                    var successReturn = DeleteMaterialProcess(dataDeletes, param);
                                    lstSuccess.Add(successReturn);
                                }
                                //xử lý thêm
                                if (dataCreates != null && dataCreates.Count() > 0)
                                {
                                    var successReturn = CreateMaterialProcess(dataCreates, param);
                                    lstSuccess.Add(successReturn);
                                }
                                // xu ly trang thai thanh cong/that bai
                                if (lstSuccess.Count() > 0)
                                    success = lstSuccess.Where(o => o == false).ToList().Count() > 0 ? false : true;



                                this.lstMaterialADOs = this.lstMaterialADOs.OrderByDescending(p => p.checkMate).ToList();
                                if (ucGridControlMaterial != null)
                                {
                                    MaterialProcessor.Reload(ucGridControlMaterial, this.lstMaterialADOs);
                                }
                            }
                        }
                    }
                    if (isChooseMaterial == 2)
                    {
                        if (MaterialIdCheck == 0)
                        {
                            WaitingManager.Hide();
                            DevExpress.XtraEditors.XtraMessageBox.Show("Chưa chọn vật tư", "Thông báo");
                            return;
                        }

                        if (Service is List<HIS.UC.Service.ServiceADO>)
                        {
                            this.lstServiceADOs = (List<HIS.UC.Service.ServiceADO>)Service;

                            if (this.lstServiceADOs != null && this.lstServiceADOs.Count > 0)
                            {
                                //bool success = false;
                                HIS.UC.Material.MaterialADO materialType = this.lstMaterialADOs.FirstOrDefault(o => o.ID == MaterialIdCheck);
                                //Danh sach cac user duoc check

                                var dataChecked = this.lstServiceADOs.Where(p => p.checkService == true).ToList();


                                //List xoa

                                var dataDelete = this.lstServiceADOs.Where(o => matyMaterials.Select(p => p.SERVICE_ID)
                                    .Contains(o.ID) && o.checkService == false).ToList();

                                //list them
                                var dataCreate = dataChecked.Where(o => !matyMaterials.Select(p => p.SERVICE_ID)
                                    .Contains(o.ID)).ToList();
                                //list update
                                var dataUpdate = dataChecked.Where(o => matyMaterials.Select(p => p.SERVICE_ID)
                                   .Contains(o.ID)).ToList();

                                if (dataChecked.Count == 0 && dataDelete.Count == 0)
                                {
                                    WaitingManager.Hide();
                                    DevExpress.XtraEditors.XtraMessageBox.Show("Chưa chọn dịch vụ nào", "Thông báo");

                                    return;
                                }
                                //if (dataChecked != null)
                                //{
                                //    success = true;
                                //}

                                if (materialType != null && materialType.EXPEND_AMOUNT_STR <= 0)
                                {
                                    WaitingManager.Hide();
                                    DevExpress.XtraEditors.XtraMessageBox.Show("Số lượng phải lớn hơn không", "Thông báo");
                                    throw new Exception("Số lượng sai");
                                }
                                List<bool> lstSuccess = new List<bool>();
                                //xử lý update
                                if (dataUpdate != null && dataUpdate.Count() > 0)
                                {
                                    var successReturn = UpdateServiceProcess(dataUpdate, materialType, param);
                                    lstSuccess.Add(successReturn);
                                }
                                //xử lý delete
                                if (dataDelete != null && dataCreate.Count() > 0)
                                {
                                    var successReturn = DeleteServiceProcess(dataDelete, param);
                                    lstSuccess.Add(successReturn);
                                }
                                //xử lý thêm
                                if (dataCreate != null && dataCreate.Count() > 0)
                                {
                                    var successReturn = CreateServiceProcess(dataCreate, materialType, param);
                                    lstSuccess.Add(successReturn);
                                }
                                // xu ly trang thai thanh cong/that bai
                                if (lstSuccess.Count() > 0)
                                    success = lstSuccess.Where(o => o == false).ToList().Count() > 0 ? false : true;

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

        private bool CreateServiceProcess(List<ServiceADO> dataCreate, MaterialADO materialType, CommonParam param)
        {
            bool success = false;
            if (dataCreate != null && dataCreate.Count > 0 && materialType != null)
            {
                if (materialType.EXPEND_AMOUNT_STR <= 0)
                {
                    WaitingManager.Hide();
                    DevExpress.XtraEditors.XtraMessageBox.Show("Số lượng phải lớn hơn không", "Thông báo");
                    success = false;
                    return success;
                }
                List<V_HIS_SERVICE_MATY> mestServiceCreate = new List<V_HIS_SERVICE_MATY>();
                foreach (var item in dataCreate)
                {
                    V_HIS_SERVICE_MATY mestService = new V_HIS_SERVICE_MATY();
                    mestService.SERVICE_ID = item.ID;
                    mestService.MATERIAL_TYPE_ID = MaterialIdCheck;
                    mestService.AMOUNT_BHYT = materialType.AMOUNT_BHYT_STR;
                    mestService.IS_NOT_EXPEND = materialType.checkExpend == true ? (short?)1 : null; ;
                    mestService.EXPEND_AMOUNT = materialType.EXPEND_AMOUNT_STR;
                    mestService.EXPEND_PRICE = materialType.EXPEND_PRICE_STR;
                    mestService.SERVICE_UNIT_ID = item.SERVICE_UNIT_ID;
                    mestServiceCreate.Add(mestService);
                }

                var createResult = new BackendAdapter(param).Post<List<HIS_SERVICE_MATY>>(
                           "api/HisServiceMaty/CreateList",
                           HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer,
                           mestServiceCreate,
                           param);
                if (createResult != null && createResult.Count > 0)
                {
                    success = true;
                    AutoMapper.Mapper.CreateMap<HIS_SERVICE_MATY, V_HIS_SERVICE_MATY>();
                    var vCreateResults = AutoMapper.Mapper.Map<List<HIS_SERVICE_MATY>, List<V_HIS_SERVICE_MATY>>(createResult);
                    matyMaterials.AddRange(vCreateResults);
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
                List<long> deleteId = matyMaterials.Where(o => dataDelete.Select(p => p.ID)
                    .Contains(o.SERVICE_ID)).Select(o => o.ID).ToList();
                bool deleteResult = new BackendAdapter(param).Post<bool>(
                          "api/HisServiceMaty/DeleteList",
                          HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer,
                          deleteId,
                          param);
                if (deleteResult)
                {
                    success = true;
                    matyMaterials = matyMaterials.Where(o => !deleteId.Contains(o.ID)).ToList();
                }
            }
            return success;
        }

        //Hàm xử lý update Service
        private bool UpdateServiceProcess(List<ServiceADO> dataUpdate, MaterialADO materialType, CommonParam param)
        {
            bool success = false;
            if (dataUpdate != null && dataUpdate.Count > 0 && materialType != null)
            {
                var materialMetyUpdates = new List<V_HIS_SERVICE_MATY>();
                foreach (var item in dataUpdate)
                {
                    var materialMaty = matyMaterials.FirstOrDefault(o => o.MATERIAL_TYPE_ID == MaterialIdCheck && o.SERVICE_ID == item.ID);
                    if (materialMaty != null)
                    {
                        materialMaty.IS_NOT_EXPEND = materialType.checkExpend == true ? (short?)1 : null;
                        materialMaty.AMOUNT_BHYT = materialType.AMOUNT_BHYT_STR;
                        materialMaty.EXPEND_AMOUNT = materialType.EXPEND_AMOUNT_STR;
                        materialMaty.EXPEND_PRICE = materialType.EXPEND_PRICE_STR;
                        //MedicineMety.SERVICE_UNIT_ID = item.SERVICE_UNIT_ID;
                        //MedicineMety.MEDICINE_TYPE_ID = MedicineIdCheck;
                        materialMetyUpdates.Add(materialMaty);
                    }
                }
                if (materialMetyUpdates != null && materialMetyUpdates.Count > 0)
                {
                    var updateResult = new BackendAdapter(param).Post<List<HIS_SERVICE_MATY>>(
                               "/api/HisServiceMaty/UpdateList",
                               HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer,
                               materialMetyUpdates,
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
        private bool CreateMaterialProcess(List<MaterialADO> dataCreates, CommonParam param)
        {
            bool success = false;
            if (dataCreates != null && dataCreates.Count > 0)
            {
                List<V_HIS_SERVICE_MATY> MestServiceCreates = new List<V_HIS_SERVICE_MATY>();
                foreach (var item in dataCreates)
                {
                    if (item.EXPEND_AMOUNT_STR <= 0)
                    {
                        WaitingManager.Hide();
                        DevExpress.XtraEditors.XtraMessageBox.Show("Số lượng phải lớn hơn không", "Thông báo");
                        success = false;
                        return success;
                    }
                    V_HIS_SERVICE_MATY mestService = new V_HIS_SERVICE_MATY();
                    mestService.MATERIAL_TYPE_ID = item.ID;
                    mestService.SERVICE_UNIT_ID = ServiceUnitId;
                    mestService.SERVICE_ID = ServiceIdCheckByService;
                    mestService.IS_NOT_EXPEND = item.checkExpend == true ? (short?)1 : null;
                    mestService.AMOUNT_BHYT = item.AMOUNT_BHYT_STR;
                    mestService.EXPEND_AMOUNT = item.EXPEND_AMOUNT_STR;
                    mestService.EXPEND_PRICE = item.EXPEND_PRICE_STR;
                    MestServiceCreates.Add(mestService);
                }

                var createResult = new BackendAdapter(param).Post<List<HIS_SERVICE_MATY>>(
                           "api/HisServiceMaty/CreateList",
                           HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer,
                           MestServiceCreates,
                           param);
                if (createResult != null && createResult.Count > 0)
                {
                    success = true;
                    AutoMapper.Mapper.CreateMap<HIS_SERVICE_MATY, V_HIS_SERVICE_MATY>();
                    var vCreateResults = AutoMapper.Mapper.Map<List<HIS_SERVICE_MATY>, List<V_HIS_SERVICE_MATY>>(createResult);
                    matyServices.AddRange(vCreateResults);
                }
            }
            return success;
        }

        //Hàm xóa material
        private bool DeleteMaterialProcess(List<MaterialADO> dataDeletes, CommonParam param)
        {
            bool success = false;
            if (dataDeletes != null && dataDeletes.Count > 0)
            {
                List<long> deleteIds = matyServices.Where(o => dataDeletes.Select(p => p.ID)
                    .Contains(o.MATERIAL_TYPE_ID)).Select(o => o.ID).ToList();
                bool deleteResult = new BackendAdapter(param).Post<bool>(
                          "api/HisServiceMaty/DeleteList",
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

        //Hàm update material
        private bool UpdateMaterialProcess(List<MaterialADO> dataUpdate, CommonParam param)
        {

            bool success = false;
            if (dataUpdate != null && dataUpdate.Count > 0)
            {
                var serviceMatyUpdates = new List<V_HIS_SERVICE_MATY>();
                foreach (var item in dataUpdate)
                {
                    if (item.EXPEND_AMOUNT_STR <= 0)
                    {
                        WaitingManager.Hide();
                        DevExpress.XtraEditors.XtraMessageBox.Show("Số lượng phải lớn hơn không", "Thông báo");
                        success = false;
                        return success;
                    }
                    var ServiceMaty = matyServices.FirstOrDefault(o => o.MATERIAL_TYPE_ID == item.ID && o.SERVICE_ID == ServiceIdCheckByService);
                    if (ServiceMaty != null)
                    {
                        ServiceMaty.IS_NOT_EXPEND = item.checkExpend == true ? (short?)1 : null;
                        ServiceMaty.AMOUNT_BHYT = item.AMOUNT_BHYT_STR;
                        ServiceMaty.EXPEND_AMOUNT = item.EXPEND_AMOUNT_STR;
                        ServiceMaty.EXPEND_PRICE = item.EXPEND_PRICE_STR;
                        serviceMatyUpdates.Add(ServiceMaty);
                    }
                }
                Inventec.Common.Logging.LogSystem.Error(Inventec.Common.Logging.LogUtil.TraceData("serviceMatyUpdates_____", serviceMatyUpdates));
                if (serviceMatyUpdates != null && serviceMatyUpdates.Count > 0)
                {
                    var updateResult = new BackendAdapter(param).Post<List<HIS_SERVICE_MATY>>(
                               "/api/HisServiceMaty/UpdateList",
                               HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer,
                               serviceMatyUpdates,
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
                    FillDataToGridMaterial(this);
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
                Resources.ResourceLanguageManager.LanguageResource = new System.Resources.ResourceManager("HIS.Desktop.Plugins.ServiceMaty.Resources.Lang", typeof(HIS.Desktop.Plugins.ServiceMaty.UCServiceMaty).Assembly);
                //Gán giá trị cho các control
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridLookUpEdit1_Properties_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == DevExpress.XtraEditors.Controls.ButtonPredefines.Delete)
                {
                    cboServiceType.EditValue = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
