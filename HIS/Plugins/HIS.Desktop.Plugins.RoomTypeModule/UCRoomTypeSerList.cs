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
using HIS.UC.RoomTypeList.ADO;
using HIS.UC.Module.ADO;
using HIS.UC.RoomTypeList;
using MOS.EFMODEL.DataModels;
using HIS.Desktop.ADO;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using HIS.Desktop.LocalStorage.ConfigApplication;
using MOS.Filter;
using Inventec.Common.Adapter;
using HIS.UC.Module;
using ACS.EFMODEL.DataModels;
using HIS.Desktop.Plugins.RoomTypeModule.Entity;
using Inventec.Common.Controls.EditorLoader;
using HIS.Desktop.ApiConsumer;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using System.Configuration;
using HIS.Desktop.LocalStorage.LocalData;
using System.Resources;
using Inventec.Desktop.Common.LanguageManager;

namespace HIS.Desktop.Plugins.RoomTypeModule
{
    public partial class UCRoomTypeSerList : HIS.Desktop.Utility.UserControlBase
    {
        internal List<HIS.UC.Module.ModuleADO> moduleAdo { get; set; }
        internal List<HIS.UC.RoomTypeList.RoomTypeListADO> roomtyleAdo { get; set; }
        UCRoomTypeListProcessor roomtypeProcessor = null;
        UCModuleProcessor moduleProcessor = null;
        UserControl ucRoomType = null;
        UserControl ucModule = null;
        Inventec.Desktop.Common.Modules.Module moduleData;
        List<HIS_ROOM_TYPE> listRoomtype = new List<HIS_ROOM_TYPE>();
        List<ACS_MODULE> listModule = new List<ACS_MODULE>();
        List<HIS_ROOM_TYPE_MODULE> listRoomTypeModule = new List<HIS_ROOM_TYPE_MODULE>();
        long checkRoomType = 0;
        int start = 0;
        int limit = 0;
        int rowCount = 0;
        int dataTotal = 0;
        int start1 = 0;
        int limit1 = 0;
        int rowCount1 = 0;
        int dataTotal1 = 0;
        string ModuleLink = null;
        long isChoseRoomType = 0;
        long isChoseModule = 0;
        bool isCheckAll;

        HIS.UC.Module.ModuleADO currentCopyModuleAdo;
        HIS.UC.RoomTypeList.RoomTypeListADO currentCopyRoomTypeAdo;

        public UCRoomTypeSerList(Inventec.Desktop.Common.Modules.Module _moduleData)
            : base(_moduleData)
        {
            InitializeComponent();
            try
            {
                SetCaptionByLanguageKey();
                InitRoomType();
                InitModule();
                LoadComboStatus();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void UCRoomTypeSerList_Load(object sender, EventArgs e)
        {
            FillDataToGrid1(this);
            FillDataToGrid2(this);
            
        }
        
        private void FillDataToRoomtype(UCRoomTypeSerList _roomtype)
        {
            try
            {
                int numPageSize = 0;
                FillDataToGridRoomType(new CommonParam(0, numPageSize));
                CommonParam param = new CommonParam();
                param.Limit = rowCount;
                param.Count = dataTotal;
                ucPaging1.Init(FillDataToGridRoomType, param);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void FillDataToGridRoomType(object data)
        {
            try
            {
                WaitingManager.Show();
                listRoomtype = new List<HIS_ROOM_TYPE>();
                int start = ((CommonParam)data).Start ?? 0;
                int limit = ((CommonParam)data).Limit ?? 0;
                CommonParam param = new CommonParam(start, limit);
                MOS.Filter.HisRoomTypeFilter RoomtypeFilter = new MOS.Filter.HisRoomTypeFilter();
                RoomtypeFilter.ORDER_FIELD = "DEPARTMENT_NAME";
                RoomtypeFilter.ORDER_DIRECTION = "ASC";
                RoomtypeFilter.KEY_WORD = txtKeyword1.Text;

                if ((long)cboChoose.EditValue == 1)
                {
                    isChoseRoomType = (long)cboChoose.EditValue;
                }
                if ((long)cboChoose.EditValue == 2)
                {
                    isChoseRoomType = (long)cboChoose.EditValue;
                }

                var rs = new Inventec.Common.Adapter.BackendAdapter(param).GetRO<List<MOS.EFMODEL.DataModels.HIS_ROOM_TYPE>>(
                    "api/HisRoomType/Get",
                    HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer,
                    RoomtypeFilter,
                    param);

                roomtyleAdo = new List<HIS.UC.RoomTypeList.RoomTypeListADO>();
                if (rs != null && rs.Data.Count > 0)
                {
                    listRoomtype = rs.Data;
                    foreach (var item in listRoomtype)
                    {
                        HIS.UC.RoomTypeList.RoomTypeListADO RoomtypeADO = new HIS.UC.RoomTypeList.RoomTypeListADO(item);
                        if (isChoseRoomType == 1)
                        {
                            RoomtypeADO.isKeyChoose = true;
                        }
                        roomtyleAdo.Add(RoomtypeADO);
                    }
                }

                if (listRoomTypeModule != null && listRoomTypeModule.Count > 0)
                {
                    foreach (var item in listRoomTypeModule)
                    {
                        var check = roomtyleAdo.FirstOrDefault(o => o.ID == item.ROOM_TYPE_ID);
                        if (check != null)
                        {
                            check.check1 = true;
                        }
                    }
                }

                if (ucRoomType != null)
                {
                    roomtypeProcessor.Reload(ucRoomType, roomtyleAdo);
                }
                rowCount = (data == null ? 0 : roomtyleAdo.Count);
                dataTotal = (rs.Param == null ? 0 : rs.Param.Count ?? 0);
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void FillDataToGrid1(UCRoomTypeSerList _roomtype)
        {
            try
            {
                int numPageSize = 0;
                if (ucPaging1.pagingGrid != null)
                {
                    numPageSize = ucPaging1.pagingGrid.PageSize;
                }
                else
                {
                    numPageSize = (int)ConfigApplications.NumPageSize;
                }
                FillDataToGridRoomType(new CommonParam(0, numPageSize));

                CommonParam param = new CommonParam();
                param.Limit = rowCount;
                param.Count = dataTotal;
                ucPaging1.Init(FillDataToGridRoomType, param);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btn_Radio_Enable_Click(HIS_ROOM_TYPE data)
        {
            try
            {
                WaitingManager.Show();
                CommonParam param = new CommonParam();
                MOS.Filter.HisRoomTypeModuleFilter filter = new HisRoomTypeModuleFilter();
                filter.ROOM_TYPE_ID = data.ID;
                checkRoomType = data.ID;
                listRoomTypeModule = new List<HIS_ROOM_TYPE_MODULE>();
                listRoomTypeModule = new BackendAdapter(param).Get<List<HIS_ROOM_TYPE_MODULE>>(
                                HIS.Desktop.ApiConsumer.HisRequestUriStore.HIS_ROOM_TYPE_MODULE__GET,
                                HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer,
                                filter,
                                param);
                List<HIS.UC.Module.ModuleADO> dataNew = new List<HIS.UC.Module.ModuleADO>();
                dataNew = (from r in listModule select new HIS.UC.Module.ModuleADO(r)).ToList();
                if (listRoomTypeModule != null && listRoomTypeModule.Count > 0)
                {
                    foreach (var itemRoom in listRoomTypeModule)
                    {
                        var check = dataNew.FirstOrDefault(o => o.MODULE_LINK == itemRoom.MODULE_LINK);
                        if (check != null)
                        {
                            check.check2 = true;

                        }
                    }
                }

                dataNew = dataNew.OrderByDescending(p => p.check2).ToList();
                if (ucModule != null)
                {
                    moduleProcessor.Reload(ucModule, dataNew);
                }
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void btn_Radio_Enable_Click1(ACS_MODULE data)
        {
            try
            {
                WaitingManager.Show();
                CommonParam param = new CommonParam();
                MOS.Filter.HisRoomTypeModuleFilter filter = new HisRoomTypeModuleFilter();
                filter.MODULE_LINK__EXACT = data.MODULE_LINK;
                ModuleLink = data.MODULE_LINK;
                listRoomTypeModule = new List<HIS_ROOM_TYPE_MODULE>();
                listRoomTypeModule = new BackendAdapter(param).Get<List<HIS_ROOM_TYPE_MODULE>>(
                                HIS.Desktop.ApiConsumer.HisRequestUriStore.HIS_ROOM_TYPE_MODULE__GET,
                                HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer,
                                filter,
                                param);
                List<HIS.UC.RoomTypeList.RoomTypeListADO> dataNew = new List<HIS.UC.RoomTypeList.RoomTypeListADO>();
                dataNew = (from r in listRoomtype select new RoomTypeListADO(r)).ToList();
                if (listRoomTypeModule != null && listRoomTypeModule.Count > 0)
                {
                    foreach (var itemRoomtypeModule in listRoomTypeModule)
                    {
                        var check = dataNew.FirstOrDefault(o => o.ID == itemRoomtypeModule.ROOM_TYPE_ID);
                        if (check != null)
                        {
                            check.check1 = true;
                        }
                    }

                    dataNew = dataNew.OrderByDescending(p => p.check1).ToList();
                    if (ucRoomType != null)
                    {
                        roomtypeProcessor.Reload(ucRoomType, dataNew);
                    }
                }
                else
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

        private void FillDataToRoom(UCRoomTypeSerList _module)
        {
            try
            {
                int numPageSize = 0;
                if (ucPaging2.pagingGrid != null)
                {
                    numPageSize = ucPaging2.pagingGrid.PageSize;
                }
                else
                {
                    numPageSize = (int)ConfigApplications.NumPageSize;
                }
                FillDataToGridRoom(new CommonParam(0, numPageSize));

                CommonParam param = new CommonParam();
                param.Limit = rowCount;
                param.Count = dataTotal;
                ucPaging2.Init(FillDataToGridRoom, param);
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
                listModule = new List<ACS_MODULE>();
                int start = ((CommonParam)data).Start ?? 0;
                int limit = ((CommonParam)data).Limit ?? 0;
                CommonParam param = new CommonParam(start, limit);
                ACS.Filter.AcsModuleViewFilter ModuleFilter = new ACS.Filter.AcsModuleViewFilter();
                ModuleFilter.ORDER_FIELD = "DEPARTMENT_NAME";
                ModuleFilter.ORDER_DIRECTION = "ASC";
                ModuleFilter.APPLICATION_CODE = GlobalVariables.APPLICATION_CODE;
                ModuleFilter.IS_VISIBLE = 1;
                ModuleFilter.KEY_WORD = txtKeyword2.Text;

                List<ACS_MODULE> filteredModules = listModule.Where(item =>
                    item.MODULE_LINK.Contains(txtKeyword2.Text.Trim()) ||
                    item.MODULE_NAME.Contains(txtKeyword2.Text.Trim())
                ).ToList();

                if ((long)cboChoose.EditValue == 2)
                {
                    isChoseModule = (long)cboChoose.EditValue;
                }
                if ((long)cboChoose.EditValue == 1)
                {
                    isChoseModule = (long)cboChoose.EditValue;
                }

                var rs = new Inventec.Common.Adapter.BackendAdapter(param).GetRO<List<ACS.EFMODEL.DataModels.ACS_MODULE>>(
                    "api/AcsModule/Get",
                    HIS.Desktop.ApiConsumer.ApiConsumers.AcsConsumer,
                    ModuleFilter,
                    param);

                moduleAdo = new List<HIS.UC.Module.ModuleADO>();
                if (rs != null && rs.Data.Count > 0)
                {
                    listModule = rs.Data;
                    //Bo trung nhau 
                    var moduleGroups = listModule.GroupBy(o => o.MODULE_LINK);
                    listModule = new List<ACS_MODULE>();
                    foreach (var moduleGroup in moduleGroups)
                    {
                        listModule.Add(moduleGroup.First());
                    }

                    foreach (var item in listModule)
                    {
                        HIS.UC.Module.ModuleADO ModuleADO = new HIS.UC.Module.ModuleADO(item);
                        if (isChoseModule == 2)
                        {
                            ModuleADO.isKeyChoose1 = true;
                        }
                        moduleAdo.Add(ModuleADO);
                    }
                }

                if (listRoomTypeModule != null && listRoomTypeModule.Count > 0)
                {

                    foreach (var itemRoom in listRoomTypeModule)
                    {
                        var check = moduleAdo.FirstOrDefault(o => o.MODULE_LINK == itemRoom.MODULE_LINK);
                        if (check != null)
                        {
                            check.check2 = true;
                        }
                    }
                }

                moduleAdo = moduleAdo.OrderByDescending(p => p.check2).ToList();
                if (ucModule != null)
                {
                    moduleProcessor.Reload(ucModule, moduleAdo);
                }

                rowCount = (data == null ? 0 : moduleAdo.Count);
                dataTotal = (rs.Param == null ? 0 : rs.Param.Count ?? 0);
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void FillDataToGrid2(UCRoomTypeSerList _module)
        {
            try
            {
                int numPageSize = 0;
                if (ucPaging2.pagingGrid != null)
                {
                    numPageSize = ucPaging2.pagingGrid.PageSize;
                }
                else
                {
                    numPageSize = (int)ConfigApplications.NumPageSize;
                }
                FillDataToGridRoom(new CommonParam(0, numPageSize));

                CommonParam param = new CommonParam();
                param.Limit = rowCount;
                param.Count = dataTotal;
                ucPaging2.Init(FillDataToGridRoom, param);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void btnSearch1_Click(object sender, EventArgs e)
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

        private void btnSearch2_Click(object sender, EventArgs e)
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
        private void simpleButton3_Click(object sender, EventArgs e)
        {
            try
            {
                if (ucModule != null && ucRoomType != null)
                {
                    bool success = false;
                    CommonParam param = new CommonParam();
                    List<HIS.UC.Module.ModuleADO> aModule = moduleProcessor.GetDataGridView(ucModule) as List<HIS.UC.Module.ModuleADO>;
                    List<HIS.UC.RoomTypeList.RoomTypeListADO> aRoomType = roomtypeProcessor.GetDataGridView(ucRoomType) as List<HIS.UC.RoomTypeList.RoomTypeListADO>;
                    if (isChoseRoomType == 1 && aModule != null && aModule.Count > 0)
                    {
                        if (checkRoomType > 0)
                        {
                            if (aModule != null && aModule.Count > 0)
                            {
                                MOS.Filter.HisRoomTypeModuleFilter modulefilter = new HisRoomTypeModuleFilter();
                                modulefilter.ROOM_TYPE_ID = checkRoomType;
                                var moduleRoomType = new BackendAdapter(param).Get<List<HIS_ROOM_TYPE_MODULE>>("api/HisRoomTypeModule/Get", ApiConsumers.MosConsumer, modulefilter, param);

                                var dataCheckeds = aModule.Where(p => (p.check2 == true)).ToList();

                                //List xóa
                                var dataDeletes = aModule.Where(o => moduleRoomType.Select(p => p.MODULE_LINK)
                               .Contains(o.MODULE_LINK) && o.check2 == false).ToList();

                                //list them
                                var dataCreates = dataCheckeds.Where(o => !moduleRoomType.Select(p => p.MODULE_LINK)
                                    .Contains(o.MODULE_LINK)).ToList();

                                if ((dataDeletes == null || dataDeletes.Count <= 0) && (dataCreates == null || dataCreates.Count <= 0))
                                {
                                    MessageManager.Show(Resources.ResourceMessage.Plugin_XuLyThatBaiKhongCoDuLieuThayDoi);
                                    return;
                                }

                                if (dataDeletes != null && dataDeletes.Count > 0)// && dataDeletes.Count < 3)
                                {
                                    List<long> deleteIds = moduleRoomType.Where(o => dataDeletes.Select(p => p.MODULE_LINK)
                                        .Contains(o.MODULE_LINK)).Select(o => o.ID).ToList();
                                    bool deleteResult = new BackendAdapter(param).Post<bool>(
                                              "/api/HisRoomTypeModule/DeleteList",
                                              HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer,
                                              deleteIds,
                                              param);
                                    if (deleteResult)
                                    {
                                        success = true;
                                        listRoomTypeModule = listRoomTypeModule.Where(o => !deleteIds.Contains(o.ID)).ToList();
                                    }
                                }

                                if (dataCreates != null && dataCreates.Count > 0)
                                {
                                    List<HIS_ROOM_TYPE_MODULE> moduleRoomTypeCreates = new List<HIS_ROOM_TYPE_MODULE>();
                                    foreach (var item in dataCreates)
                                    {
                                        HIS_ROOM_TYPE_MODULE moduleRoomTypes = new HIS_ROOM_TYPE_MODULE();
                                        moduleRoomTypes.MODULE_LINK = item.MODULE_LINK;
                                        moduleRoomTypes.ROOM_TYPE_ID = checkRoomType;
                                        moduleRoomTypeCreates.Add(moduleRoomTypes);
                                    }

                                    var createResult = new BackendAdapter(param).Post<List<HIS_ROOM_TYPE_MODULE>>(
                                               "/api/HisRoomTypeModule/CreateList",
                                               HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer,
                                               moduleRoomTypeCreates,
                                               param);
                                    if (createResult != null && createResult.Count > 0)
                                    {
                                        success = true;
                                        listRoomTypeModule.AddRange(createResult);
                                    }
                                }
                                aModule = aModule.OrderByDescending(p => p.check2).ToList();
                                moduleProcessor.Reload(ucModule, aModule);
                            }
                        }
                        else
                        {
                            MessageManager.Show(Resources.ResourceMessage.Plugin_XuLyThatBaiChuaChonLoaiPhong);
                            return;
                        }
                    }

                    if (isChoseModule == 2 && aRoomType != null && aRoomType.Count > 0)
                    {
                        if (!String.IsNullOrEmpty(ModuleLink))
                        {
                            if (aRoomType != null && aRoomType.Count > 0)
                            {
                                MOS.Filter.HisRoomTypeModuleFilter roomtypefilter = new HisRoomTypeModuleFilter();
                                roomtypefilter.MODULE_LINK__EXACT = ModuleLink;
                                var RoomtypeModule = new BackendAdapter(param).Get<List<HIS_ROOM_TYPE_MODULE>>("api/HisRoomTypeModule/Get", ApiConsumers.MosConsumer, roomtypefilter, param);

                                var dataCheckeds = aRoomType.Where(p => (p.check1 == true)).ToList();

                                //List xóa
                                var dataDeletes = aRoomType.Where(o => RoomtypeModule.Select(p => p.ROOM_TYPE_ID)
                               .Contains(o.ID) && o.check1 == false).ToList();

                                //list them
                                var dataCreates = dataCheckeds.Where(o => !RoomtypeModule.Select(p => p.ROOM_TYPE_ID)
                                    .Contains(o.ID)).ToList();

                                if ((dataDeletes == null || dataDeletes.Count <= 0) && (dataCreates == null || dataCreates.Count <= 0))
                                {
                                    MessageManager.Show(Resources.ResourceMessage.Plugin_XuLyThatBaiKhongCoDuLieuThayDoi);
                                    return;
                                }

                                if (dataDeletes != null && dataDeletes.Count > 0 && dataDeletes.Count < 3)
                                {
                                    List<long> deleteIds = RoomtypeModule.Where(o => dataDeletes.Select(p => p.ID)
                                        .Contains(o.ROOM_TYPE_ID)).Select(o => o.ID).ToList();
                                    bool deleteResult = new BackendAdapter(param).Post<bool>(
                                              "/api/HisRoomTypeModule/DeleteList",
                                              HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer,
                                              deleteIds,
                                              param);
                                    if (deleteResult)
                                    {
                                        success = true;
                                        listRoomTypeModule = listRoomTypeModule.Where(o => !deleteIds.Contains(o.ID)).ToList();
                                    }

                                }

                                if (dataCreates != null && dataCreates.Count > 0)
                                {
                                    List<HIS_ROOM_TYPE_MODULE> RoomtypeModuleCreates = new List<HIS_ROOM_TYPE_MODULE>();
                                    foreach (var item in dataCreates)
                                    {
                                        HIS_ROOM_TYPE_MODULE RoomtypeModules = new HIS_ROOM_TYPE_MODULE();
                                        RoomtypeModules.ROOM_TYPE_ID = item.ID;
                                        RoomtypeModules.MODULE_LINK = ModuleLink;
                                        RoomtypeModuleCreates.Add(RoomtypeModules);
                                    }

                                    var createResult = new BackendAdapter(param).Post<List<HIS_ROOM_TYPE_MODULE>>(
                                               "/api/HisRoomTypeModule/CreateList",
                                               HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer,
                                               RoomtypeModuleCreates,
                                               param);
                                    if (createResult != null && createResult.Count > 0)
                                    {
                                        success = true;
                                        listRoomTypeModule.AddRange(createResult);
                                    }

                                }
                                aRoomType = aRoomType.OrderByDescending(o => o.check1).ToList();
                                roomtypeProcessor.Reload(ucRoomType, aRoomType);
                            }
                        }
                        else
                        {
                            MessageManager.Show(Resources.ResourceMessage.Plugin_XuLyThatBaiChuaChonChucNang);
                            return;
                        }
                    }
                        MessageManager.Show(this.ParentForm, param, success);                    
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtKeyword1_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    btnSearch1.Focus();
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtKeyword2_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    btnSearch2.Focus();
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        public void Save()
        {
            try
            {
                btnSave.Focus();
                simpleButton3_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void LoadComboStatus()
        {
            try
            {
                List<Status> status = new List<Status>();
                status.Add(new Status(1, "Loại phòng"));
                status.Add(new Status(2, "Chức năng"));

                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("statusName", "", 300, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("statusName", "id", columnInfos, false, 350);
                ControlEditorLoader.Load(cboChoose, status, controlEditorADO);
                cboChoose.EditValue = status[0].id;
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void gridViewRoomType_MouseDownRoomType(object sender, MouseEventArgs e)
        {
            try
            {
                if (isChoseRoomType == 1)
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
                            var lstCheckAll = roomtyleAdo;
                            List<HIS.UC.RoomTypeList.RoomTypeListADO> lstChecks = new List<HIS.UC.RoomTypeList.RoomTypeListADO>();

                            if (lstCheckAll != null && lstCheckAll.Count > 0)
                            {
                                var roomCheckedNum = roomtyleAdo.Where(o => o.check1 == true).Count();
                                var roomNum = roomtyleAdo.Count();
                                if ((roomCheckedNum > 0 && roomCheckedNum < roomNum) || roomCheckedNum == 0)
                                {
                                    isCheckAll = true;
                                    hi.Column.Image = imgRoomType.Images[0];
                                }

                                if (roomCheckedNum == roomNum)
                                {
                                    isCheckAll = false;
                                    hi.Column.Image = imgRoomType.Images[1];
                                }
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
                                }

                                roomtypeProcessor.Reload(ucRoomType, lstChecks);
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
        private void gridViewModule_MouseDownModule(object sender, MouseEventArgs e)
        {
            try
            {
                if (isChoseModule == 2)
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
                            var lstCheckAll = moduleAdo;
                            List<HIS.UC.Module.ModuleADO> lstChecks = new List<HIS.UC.Module.ModuleADO>();

                            if (lstCheckAll != null && lstCheckAll.Count > 0)
                            {
                                var accountCheckedNum = moduleAdo.Where(o => o.check2 == true).Count();
                                var accountmNum = moduleAdo.Count();
                                if ((accountCheckedNum > 0 && accountCheckedNum < accountmNum) || accountCheckedNum == 0)
                                {
                                    isCheckAll = true;
                                    hi.Column.Image = imgModule.Images[0];
                                }

                                if (accountCheckedNum == accountmNum)
                                {
                                    isCheckAll = false;
                                    hi.Column.Image = imgModule.Images[1];
                                }
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
                                }

                                moduleProcessor.Reload(ucModule, lstChecks);
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
        private void cboChoose_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                isChoseRoomType = 0;
                isChoseModule = 0;
                listRoomTypeModule = new List<HIS_ROOM_TYPE_MODULE>();
                FillDataToGrid1(this);
                FillDataToGrid2(this);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
