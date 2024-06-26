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
using HIS.Desktop.LocalStorage.LocalData;
using Inventec.UC.Paging;
using MOS.Filter;
using HIS.Desktop.ApiConsumer;
using MOS.EFMODEL.DataModels;
using DevExpress.Data;
using System.Collections;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using HIS.Desktop.Utility;
using Inventec.Common.Adapter;
using System.Resources;
using Inventec.Desktop.Common.LanguageManager;
using HIS.Desktop.LocalStorage.ConfigApplication;
using HIS.Desktop.LocalStorage.BackendData;
using MOS.SDO;
using HIS.Desktop.Plugins.HisMachineServMaty.ADO;

namespace HIS.Desktop.Plugins.HisMachineServMaty
{
    public partial class UCHisMachineServMaty : HIS.Desktop.Utility.UserControlBase
    {
        internal Inventec.Desktop.Common.Modules.Module currentModule;
        List<HIS_MACHINE> machineAlls;
        List<ServiceADO> serviceAlls;
        List<MaterialTypeADO> materialTypeOlds;
        List<V_HIS_MATERIAL_TYPE> materialTypeAll;

        public UCHisMachineServMaty()
        {
            InitializeComponent();
        }

        public UCHisMachineServMaty(Inventec.Desktop.Common.Modules.Module currentModule)
            : base(currentModule)
        {
            InitializeComponent();
            this.currentModule = currentModule;
        }

        private void UCCoTreatment_Load(object sender, EventArgs e)
        {
            try
            {
                SetCaptionByLanguageKey();
                SetDataToMachine();
                GetServiceAll();
                GetMaterialTypeAll();

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetDataToMachine()
        {
            try
            {
                CommonParam param = new CommonParam();
                MOS.Filter.HisMachineFilter filter = new HisMachineFilter();
                filter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                machineAlls = new BackendAdapter(param).Get<List<HIS_MACHINE>>("api/HisMachine/Get", ApiConsumer.ApiConsumers.MosConsumer, filter, param);
                gridControlHisMachine.DataSource = machineAlls;

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void GetServiceAll()
        {
            try
            {
                MOS.Filter.HisServiceViewFilter filter = new HisServiceViewFilter();
                filter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                filter.SERVICE_TYPE_ID = IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__XN;
                var services = new BackendAdapter(new CommonParam()).Get<List<V_HIS_SERVICE>>("api/HisService/GetView", ApiConsumer.ApiConsumers.MosConsumer, filter, null);

                this.serviceAlls = new List<ServiceADO>();
                foreach (var item in services)
                {
                    ServiceADO serviceADO = new ServiceADO(item);
                    serviceADO.IsHightLight = false;
                    this.serviceAlls.Add(serviceADO);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void GetMaterialTypeAll()
        {
            try
            {
                MOS.Filter.HisMaterialTypeViewFilter filter = new HisMaterialTypeViewFilter();
                filter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                this.materialTypeAll = new BackendAdapter(new CommonParam()).Get<List<V_HIS_MATERIAL_TYPE>>("api/HisMaterialType/GetView", ApiConsumer.ApiConsumers.MosConsumer, filter, null);

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewCoTreatment_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)
                {
                    MOS.EFMODEL.DataModels.HIS_MACHINE data = (MOS.EFMODEL.DataModels.HIS_MACHINE)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    if (data != null)
                    {
                        if (e.Column.FieldName == "STT")
                        {
                            e.Value = e.ListSourceRowIndex + 1;
                        }
                        else if (e.Column.FieldName == "IS_ACTIVE_DISPLAY")
                        {
                            if (data.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE)
                            {
                                e.Value = "Hoạt động";
                            }
                            else
                            {
                                e.Value = "Tạm khóa";
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewCoTreatment_CustomRowCellEdit(object sender, DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs e)
        {
            try
            {
                GridView View = sender as GridView;

                if (e.RowHandle >= 0)
                {
                    HIS_MACHINE data = (HIS_MACHINE)((IList)((BaseView)sender).DataSource)[e.RowHandle];
                    if (e.Column.FieldName == "XuLyTiepNhan")
                    {
                        if (data != null)
                        {

                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            try
            {
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnReload_Click(object sender, EventArgs e)
        {
            try
            {
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ButtonEditXLChuyenKhoa_enable_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                //mở module 
                var row = (MOS.EFMODEL.DataModels.V_HIS_CO_TREATMENT)gridViewHisMachine.GetFocusedRow();

                if (row != null)
                {
                    Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.HisCoTreatmentReceive").FirstOrDefault();
                    moduleData.RoomId = this.currentModule.RoomId;
                    moduleData.RoomTypeId = this.currentModule.RoomTypeId;
                    if (moduleData == null) Inventec.Common.Logging.LogSystem.Error("khong tim thay moduleLink = HIS.Desktop.Plugins.HisCoTreatmentReceive");
                    if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                    {
                        List<object> listArgs = new List<object>();
                        listArgs.Add(row.ID);
                        listArgs.Add(PluginInstance.GetModuleWithWorkingRoom(moduleData, this.currentModule.RoomId, this.currentModule.RoomTypeId));
                        var extenceInstance = PluginInstance.GetPluginInstance(PluginInstance.GetModuleWithWorkingRoom(moduleData, this.currentModule.RoomId, this.currentModule.RoomTypeId), listArgs);
                        if (extenceInstance == null) throw new ArgumentNullException("moduleData is null");

                        ((Form)extenceInstance).ShowDialog();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public void bbtnSearch()
        {
            try
            {
                btnSearch_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public void bbtnReload()
        {
            try
            {
                btnReload_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private List<HIS_MACHINE_SERV_MATY> GetMachineServMatyByMachine(long machineId)
        {
            List<HIS_MACHINE_SERV_MATY> result = new List<HIS_MACHINE_SERV_MATY>();
            CommonParam param = new CommonParam();
            try
            {
                MOS.Filter.HisMachineServMatyFilter filter = new HisMachineServMatyFilter();
                filter.MACHINE_ID = machineId;
                filter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                result = new BackendAdapter(param).Get<List<MOS.EFMODEL.DataModels.HIS_MACHINE_SERV_MATY>>("api/HisMachineServMaty/Get", ApiConsumer.ApiConsumers.MosConsumer, filter, param);

            }
            catch (Exception ex)
            {
                result = null;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        private List<HIS_MACHINE_SERV_MATY> GetMachineServMatyByMachineAndService(long machineId, long serviceId)
        {
            List<HIS_MACHINE_SERV_MATY> result = new List<HIS_MACHINE_SERV_MATY>();
            CommonParam param = new CommonParam();
            try
            {
                MOS.Filter.HisMachineServMatyFilter filter = new HisMachineServMatyFilter();
                filter.MACHINE_ID = machineId;
                filter.SERVICE_ID = serviceId;
                result = new BackendAdapter(param).Get<List<MOS.EFMODEL.DataModels.HIS_MACHINE_SERV_MATY>>("api/HisMachineServMaty/Get", ApiConsumer.ApiConsumers.MosConsumer, filter, param);

            }
            catch (Exception ex)
            {
                result = null;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        private void gridViewHisMachine_Click(object sender, EventArgs e)
        {
            try
            {
                var focus = (HIS_MACHINE)gridViewHisMachine.GetFocusedRow();
                if (focus != null)
                {
                    WaitingManager.Show();
                    List<HIS_MACHINE_SERV_MATY> machineServMaties = GetMachineServMatyByMachine(focus.ID);
                    //reset
                    Parallel.ForEach(this.serviceAlls, l => l.IsHightLight = false);
                    if (machineServMaties != null && machineServMaties.Count() > 0)
                    {
                        Parallel.ForEach(this.serviceAlls.Where(f => machineServMaties.Select(o => o.SERVICE_ID).Contains(f.ID)), l => l.IsHightLight = true);
                        // sắp xếp
                        this.serviceAlls = (this.serviceAlls != null && this.serviceAlls.Count() > 0) ? this.serviceAlls.OrderByDescending(o => o.IsHightLight).ThenBy(p => p.SERVICE_NAME).ToList() : this.serviceAlls;
                    }
                    gridViewService.BeginDataUpdate();
                    gridControlService.DataSource = this.serviceAlls;
                    gridViewService.EndDataUpdate();
                    WaitingManager.Hide();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        List<HIS_MACHINE_SERV_MATY> machineServMaties;
        private void gridViewService_Click(object sender, EventArgs e)
        {
            try
            {
                var focusService = (ServiceADO)gridViewService.GetFocusedRow();
                var focusMachine = (HIS_MACHINE)gridViewHisMachine.GetFocusedRow();
                if (focusMachine != null && focusService != null)
                {
                    WaitingManager.Show();
                    materialTypeOlds = new List<MaterialTypeADO>();
                    machineServMaties = GetMachineServMatyByMachineAndService(focusMachine.ID, focusService.ID);


                    Inventec.Common.Logging.LogSystem.Info("count materialTYpe " + this.materialTypeAll.Count());

                    foreach (var item in this.materialTypeAll)
                    {
                        MaterialTypeADO materialTypeADO = new MaterialTypeADO(item);
                        if (machineServMaties != null && machineServMaties.Count() > 0)
                        {
                            var materialType = machineServMaties.FirstOrDefault(o => o.MATERIAL_TYPE_ID == item.ID);
                            if (materialType != null)
                            {
                                materialTypeADO.EXPEND_AMOUNT = materialType.EXPEND_AMOUNT;
                                materialTypeADO.EXPEND_PRICE = materialType.EXPEND_PRICE;
                            }
                        }
                        materialTypeOlds.Add(materialTypeADO);
                    }
                    materialTypeOlds = (materialTypeOlds != null && materialTypeOlds.Count() > 0) ? materialTypeOlds.OrderByDescending(o => o.EXPEND_AMOUNT).ThenBy(p => p.MATERIAL_TYPE_NAME).ToList() : materialTypeOlds;
                    gridViewMaterialType.BeginDataUpdate();
                    gridControlMaterialType.DataSource = null;
                    gridControlMaterialType.DataSource = materialTypeOlds;
                    gridViewMaterialType.EndDataUpdate();
                    WaitingManager.Hide();

                }
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
                gridViewMaterialType.PostEditor();
                WaitingManager.Show();

                var newDataSource = (List<MaterialTypeADO>)gridControlMaterialType.DataSource;

                List<HIS_MACHINE_SERV_MATY> deleteMaterialTypes = new List<HIS_MACHINE_SERV_MATY>();
                List<HIS_MACHINE_SERV_MATY> addMaterialTypes = new List<HIS_MACHINE_SERV_MATY>();
                List<HIS_MACHINE_SERV_MATY> updateMaterialTypes = new List<HIS_MACHINE_SERV_MATY>();

                foreach (var item in machineServMaties)
                {
                    var newData = newDataSource.FirstOrDefault(o => o.ID == item.MATERIAL_TYPE_ID);
                    if (newData == null)
                        continue;

                    if (item.EXPEND_AMOUNT > 0 && newData.EXPEND_AMOUNT == null)
                    {
                        item.EXPEND_AMOUNT = newData.EXPEND_AMOUNT ?? 0;
                        item.EXPEND_PRICE = newData.EXPEND_PRICE;
                        deleteMaterialTypes.Add(item);
                    }

                    if (item.EXPEND_AMOUNT > 0 && newData.EXPEND_AMOUNT != null)
                    {
                        item.EXPEND_AMOUNT = newData.EXPEND_AMOUNT ?? 0;
                        item.EXPEND_PRICE = newData.EXPEND_PRICE;
                        updateMaterialTypes.Add(item);
                    }
                }

                var add = newDataSource.Where(o => o.EXPEND_AMOUNT != null && !machineServMaties.Select(p => p.MATERIAL_TYPE_ID).Contains(o.ID)).ToList();

                var focusMachine = (HIS_MACHINE)gridViewHisMachine.GetFocusedRow();
                var service = (ServiceADO)gridViewService.GetFocusedRow();

                if (add != null && add.Count() > 0)
                {
                    foreach (var item in add)
                    {
                        HIS_MACHINE_SERV_MATY addItem = new HIS_MACHINE_SERV_MATY();
                        addItem.MATERIAL_TYPE_ID = item.ID;
                        addItem.SERVICE_ID = service.ID;
                        addItem.EXPEND_AMOUNT = item.EXPEND_AMOUNT ?? 0;
                        addItem.EXPEND_PRICE = item.EXPEND_PRICE;
                        addItem.MACHINE_ID = focusMachine.ID;
                        addMaterialTypes.Add(addItem);
                    }

                }

                CommonParam param = new CommonParam();
                bool success = false;
                if (deleteMaterialTypes != null && deleteMaterialTypes.Count() > 0)
                {
                    var apiresult = new Inventec.Common.Adapter.BackendAdapter
                         (param).Post<bool>
                         ("api/HisMachineServMaty/DeleteList", ApiConsumer.ApiConsumers.MosConsumer, deleteMaterialTypes.Select(o => o.ID
                         ).ToList(), param);
                    if (apiresult)
                    {
                        success = true;
                    }

                }

                if (addMaterialTypes != null && addMaterialTypes.Count() > 0)
                {
                    var errorList = addMaterialTypes.Where(o => o.EXPEND_AMOUNT <= 0 || o.EXPEND_PRICE <= 0).ToList();
                    if (errorList != null && errorList.Count() > 0)
                    {
                        MessageBox.Show("Tồn tại giá số lượng hoặc đơn giá giá trị không hợp lệ");
                        return;
                    }

                    var apiresult = new Inventec.Common.Adapter.BackendAdapter
                         (param).Post<List<HIS_MACHINE_SERV_MATY>>
                         ("api/HisMachineServMaty/CreateList", ApiConsumer.ApiConsumers.MosConsumer, addMaterialTypes, param);
                    if (apiresult != null && apiresult.Count() > 0)
                    {
                        success = true;
                    }
                }

                if (updateMaterialTypes != null && updateMaterialTypes.Count() > 0)
                {
                    var errorList = updateMaterialTypes.Where(o => o.EXPEND_AMOUNT <= 0 || o.EXPEND_PRICE <= 0).ToList();
                    if (errorList != null && errorList.Count() > 0)
                    {
                        MessageBox.Show("Tồn tại giá số lượng hoặc đơn giá giá trị không hợp lệ");
                        return;
                    }

                    var apiresult = new Inventec.Common.Adapter.BackendAdapter
                         (param).Post<List<HIS_MACHINE_SERV_MATY>>
                         ("api/HisMachineServMaty/UpdateList", ApiConsumer.ApiConsumers.MosConsumer, updateMaterialTypes, param);
                    if (apiresult != null && apiresult.Count() > 0)
                    {
                        success = true;
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

        private void gridViewService_RowCellStyle(object sender, RowCellStyleEventArgs e)
        {
            try
            {
                if (e.RowHandle < 0)
                    return;
                var data = (ServiceADO)gridViewService.GetRow(e.RowHandle);
                if (data != null && data.IsHightLight)
                {
                    e.Appearance.ForeColor = Color.Red;
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewService_CustomUnboundColumnData(object sender, CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)
                {
                    ServiceADO data = (ServiceADO)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    if (data != null)
                    {
                        if (e.Column.FieldName == "STT")
                        {
                            e.Value = e.ListSourceRowIndex + 1;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewMaterialType_CustomUnboundColumnData(object sender, CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)
                {
                    MaterialTypeADO data = (MaterialTypeADO)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    if (data != null)
                    {
                        if (e.Column.FieldName == "STT")
                        {
                            e.Value = e.ListSourceRowIndex + 1;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public void btnSaveShort()
        {
            btnSave_Click(null, null);
        }
    }
}
