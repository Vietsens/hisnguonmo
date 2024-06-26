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
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using MOS.EFMODEL.DataModels;
using Inventec.Desktop.Common.Message;
using Inventec.Core;
using MOS.Filter;
using HIS.Desktop.Plugins.HisServSegr.entity;
using Inventec.Common.Controls.EditorLoader;
using Inventec.Common.Adapter;
using HIS.Desktop.LocalStorage.ConfigApplication;
using DevExpress.XtraGrid.Views.Base;
using HIS.Desktop.Controls.Session;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using System.Resources;
using Inventec.Desktop.Common.LanguageManager;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.ViewInfo;
using HIS.UC.ServiceGroup;
using HIS.UC.ServiceGroup.ADO;
using Inventec.Desktop.Common.Controls.ValidationRule;
using HIS.Desktop.LibraryMessage;
using System.Threading.Tasks;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.LocalStorage.BackendData;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraGrid;

namespace HIS.Desktop.Plugins.HisServSegr
{
    public partial class ucServSegr : HIS.Desktop.Utility.UserControlBase
    {
        #region Declare
        List<ServiceADO> listCheckServiceAdos = new List<ServiceADO>();
        List<ServiceGroupADO> listCheckServiceGroupAdos = new List<ServiceGroupADO>();
        ServiceADO checkServiceAdo;
        List<HIS_SERVICE_GROUP> serviceGroups { get; set; }
        ServiceGroupADO serviceGroupADORa;
        UCServiceGroupProcessor ServiceGroupProcessor;
        UserControl ucGridControlServiceGroup;
        private int positionHandleControlEditor = -1;
        int rowCount = 0;
        int dataTotal = 0;
        int rowCount1 = 0;
        int dataTotal1 = 0;
        bool isCheckAll;
        internal List<ServiceADO> lstServiceADOs { get; set; }
        internal List<ServiceGroupADO> lstServiceGroupADOs { get; set; }
        ServiceADO serviceADORa;
        List<V_HIS_SERVICE> listService;
        List<HIS_SERVICE_GROUP> listServiceGroup;
        long ERIdCheckByER = 0;
        long ServiceIdCheckByService = 0;
        long isChoseER;
        long isChoseService;
        bool checkRa = false;
        int ActionType = -1;
        int positionHandle = -1;
        List<HIS_SERV_SEGR> servSegrs { get; set; }
        List<HIS_SERV_SEGR> servSegr1s { get; set; }
        ServiceGroupADO serviceGroupFocus = null;

        #endregion

        #region Contructor

        public ucServSegr(Inventec.Desktop.Common.Modules.Module _moduleData)
            : base(_moduleData)
        {
            InitializeComponent();
        }

        private void UC_ServSegr_Load(object sender, EventArgs e)
        {
            try
            {
                WaitingManager.Show();
                SetCaptionByLanguageKey();
                txtSearchServiceGroup.Focus();
                txtSearchServiceGroup.SelectAll();
                LoadDataToCombo();
                InitUcgridServiceGroup();
                InitDataToGridServiceGroup(this);
                ValidateForm();
                InitComboServiceType();

                gridViewService.Columns.Where(o => o.FieldName == "check2").FirstOrDefault().Image = imageCollection.Images[0];
                gridViewService.Columns.Where(o => o.FieldName == "check2").FirstOrDefault().ImageAlignment = StringAlignment.Center;
                FillDataToGrid2(this);
                spinEdit_NumOrder.EditValue = null;
                this.ActionType = HIS.Desktop.LocalStorage.LocalData.GlobalVariables.ActionAdd;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        #endregion

        #region Load Column

        private void InitComboServiceType()
        {
            try
            {
                var data = BackendDataWorker.Get<HIS_SERVICE_TYPE>().Where(o => o.ID != IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__VT && o.ID != IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__THUOC && o.ID != IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__MAU).OrderByDescending(o => o.MODIFY_TIME).ToList();
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("SERVICE_TYPE_CODE", "", 150, 1));
                columnInfos.Add(new ColumnInfo("SERVICE_TYPE_NAME", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("SERVICE_TYPE_NAME", "ID", columnInfos, false, 400);
                ControlEditorLoader.Load(cboServiceType, data, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitUcgridServiceGroup()
        {
            try
            {
                ServiceGroupProcessor = new UCServiceGroupProcessor();
                ServiceGroupInitADO ado = new ServiceGroupInitADO();
                ado.ListServiceGroupColumn = new List<ServiceGroupColumn>();
                ado.btn_Radio_Enable_Click1 = btn_Radio_Enable_Click;//TODO
                ado.gridViewServiceGroup_MouseDownMest = ServiceGroup_MouseDown;
                ado.check_CheckedChanged = Check_CheckedChanged;//TODO
                ado.spin_EditValueChanged = Spin_EditValueChanged;//TODO
                ado.serviceGroupGridView_Click = serviceGroupGridView_Click;
                ado.lockItem_Click = lockItem_Click;
                ado.unLockItem_Click = unLockItem_Click;
                ado.deleteItem_Click = deleteItem_Click;

                ServiceGroupColumn colRadio1 = new ServiceGroupColumn("   ", "radioGroup", 30, true);
                colRadio1.VisibleIndex = 0;
                colRadio1.Visible = false;
                colRadio1.UnboundColumnType = DevExpress.Data.UnboundColumnType.Object;
                ado.ListServiceGroupColumn.Add(colRadio1);

                ServiceGroupColumn colCheck1 = new ServiceGroupColumn("   ", "checkGroup", 30, true);
                colCheck1.VisibleIndex = 1;
                colCheck1.Visible = false;
                colCheck1.image = imageCollection.Images[0];
                colCheck1.ToolTip = "Chọn tất cả";//TODO
                colCheck1.imageAlignment = StringAlignment.Center;//TODO
                colCheck1.UnboundColumnType = DevExpress.Data.UnboundColumnType.Object;
                ado.ListServiceGroupColumn.Add(colCheck1);

                ServiceGroupColumn colLockUnlock = new ServiceGroupColumn("   ", "LOCK_UNLOCK", 30, true);
                colLockUnlock.VisibleIndex = 2;
                colLockUnlock.Visible = true;
                colLockUnlock.UnboundColumnType = DevExpress.Data.UnboundColumnType.Object;
                ado.ListServiceGroupColumn.Add(colLockUnlock);

                ServiceGroupColumn colLockDelete = new ServiceGroupColumn("   ", "DELETE_ITEM", 30, true);
                colLockDelete.VisibleIndex = 3;
                colLockDelete.Visible = true;
                colLockDelete.UnboundColumnType = DevExpress.Data.UnboundColumnType.Object;
                ado.ListServiceGroupColumn.Add(colLockDelete);

                ServiceGroupColumn colMaVaiTro = new ServiceGroupColumn("Mã nhóm", "SERVICE_GROUP_CODE", 80, false);
                colMaVaiTro.VisibleIndex = 4;
                ado.ListServiceGroupColumn.Add(colMaVaiTro);

                ServiceGroupColumn colTenVaiTro = new ServiceGroupColumn("Tên nhóm", "SERVICE_GROUP_NAME", 180, false);
                colTenVaiTro.VisibleIndex = 5;
                ado.ListServiceGroupColumn.Add(colTenVaiTro);

                ServiceGroupColumn colPrice = new ServiceGroupColumn("Số lượng", "AMOUNT", 70, true);
                colPrice.VisibleIndex = 6;
                ado.ListServiceGroupColumn.Add(colPrice);

                ServiceGroupColumn colIsPublic = new ServiceGroupColumn("Công khai", "IsPublic", 60, false);
                colIsPublic.VisibleIndex = 7;
                ado.ListServiceGroupColumn.Add(colIsPublic);
                ServiceGroupColumn colSttHien = new ServiceGroupColumn("STT", "NUM_ORDER", 50, false);

                colSttHien.VisibleIndex = 8;
                ado.ListServiceGroupColumn.Add(colSttHien);

                ServiceGroupColumn colCreator = new ServiceGroupColumn("Người tạo", "CREATOR", 120, false);
                colCreator.VisibleIndex = 9;
                ado.ListServiceGroupColumn.Add(colCreator);

                ServiceGroupColumn colModifier = new ServiceGroupColumn("Người sửa", "MODIFIER", 120, false);
                colModifier.VisibleIndex = 10;
                ado.ListServiceGroupColumn.Add(colModifier);

                this.ucGridControlServiceGroup = (UserControl)ServiceGroupProcessor.Run(ado);
                if (ucGridControlServiceGroup != null)
                {
                    this.panelControlServiceGroup.Controls.Add(this.ucGridControlServiceGroup);
                    this.ucGridControlServiceGroup.Dock = DockStyle.Fill;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void deleteItem_Click(ServiceGroupADO data)
        {
            try
            {
                WaitingManager.Show();
                CommonParam param = new CommonParam();
                if (IsNotAdminAndCreator(Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName(), data.CREATOR))
                {
                    WaitingManager.Hide();
                    MessageBox.Show("Không thể xóa. Bạn không phải là người tạo và không có quyền quản trị.", "Thông báo");
                    return;
                }
                bool success = new BackendAdapter(param).Post<bool>(HisRequestUriStore.HIS_SERVICE_GROUP_DELETE, ApiConsumers.MosConsumer, data.ID, param);
                if (success)
                {
                    lstServiceGroupADOs.RemoveAll(o => o.ID == data.ID);
                    ServiceGroupProcessor.Reload(this.ucGridControlServiceGroup, lstServiceGroupADOs);
                }
                WaitingManager.Hide();

                #region Hien thi message thong bao
                MessageManager.Show(this.ParentForm, param, success);
                #endregion

                #region Neu phien lam viec bi mat, phan mem tu dong logout va tro ve trang login
                SessionManager.ProcessTokenLost(param);
                #endregion
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void unLockItem_Click(ServiceGroupADO data)
        {
            try
            {
                if (data != null)
                {
                    LockUnlockServiceGroup(data);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void lockItem_Click(ServiceGroupADO data)
        {
            try
            {
                if (data != null)
                {
                    LockUnlockServiceGroup(data);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        // khóa, bỏ khóa
        private void LockUnlockServiceGroup(ServiceGroupADO serviceGroupADO)
        {
            try
            {
                WaitingManager.Show();
                bool success = false;
                CommonParam param = new CommonParam();
                HIS_SERVICE_GROUP serviceGroup = new HIS_SERVICE_GROUP();
                Inventec.Common.Mapper.DataObjectMapper.Map<HIS_SERVICE_GROUP>(serviceGroup, serviceGroupADO);
                if (IsNotAdminAndCreator(Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName(), serviceGroupADO.CREATOR))
                {
                    WaitingManager.Hide();
                    MessageBox.Show("Không được phép Khóa. Bạn không phải người tạo và không có quyền quản trị.", "Thông báo");
                    return;
                }
                var result = new BackendAdapter(param).Post<HIS_SERVICE_GROUP>(HisRequestUriStore.HIS_SERVICE_GROUP_CHANGE_LOCK, ApiConsumers.MosConsumer, serviceGroup, param);
                if (result != null)
                {
                    success = true;
                    var serviceGroupCheck = lstServiceGroupADOs.FirstOrDefault(o => o.ID == result.ID);
                    if (serviceGroupCheck != null)
                    {
                        Parallel.ForEach(lstServiceGroupADOs.Where(f => f.ID == result.ID), l => l.IS_ACTIVE = result.IS_ACTIVE);
                        ServiceGroupProcessor.Reload(this.ucGridControlServiceGroup, lstServiceGroupADOs);
                    }
                }
                WaitingManager.Hide();

                #region Hien thi message thong bao
                MessageManager.Show(this.ParentForm, param, success);
                #endregion

                #region Neu phien lam viec bi mat, phan mem tu dong logout va tro ve trang login
                SessionManager.ProcessTokenLost(param);
                #endregion
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SetDataToServiceGroupControl(ServiceGroupADO data)
        {
            try
            {
                if (data != null)
                {
                    txtServiceGroupCode.Text = data.SERVICE_GROUP_CODE;
                    txtServiceGroupName.Text = data.SERVICE_GROUP_NAME;



                    if (data.IS_PUBLIC == 1)
                    {
                        chkIsPublic.CheckState = CheckState.Checked;
                    }
                    else
                    {
                        chkIsPublic.CheckState = CheckState.Unchecked;
                    }
                    if (data.NUM_ORDER != null)
                    {
                        spinEdit_NumOrder.Value = (decimal)(data.NUM_ORDER);
                    }
                    else
                    {
                        spinEdit_NumOrder.EditValue = null;
                    }
                }
                else
                {
                    spinEdit_NumOrder.EditValue = null;
                    txtServiceGroupCode.Text = "";
                    txtServiceGroupName.Text = "";
                    chkIsPublic.CheckState = CheckState.Unchecked;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void serviceGroupGridView_Click(ServiceGroupADO data)
        {
            try
            {
                this.ActionType = HIS.Desktop.LocalStorage.LocalData.GlobalVariables.ActionEdit;
                btnSaveServiceGroup.Enabled = false;
                SetDataToServiceGroupControl(data);
                if (data.IS_ACTIVE != IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE || IsNotAdminAndCreator(Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName(), data.CREATOR))
                {

                    btnEdit.Enabled = false;
                    btnSave.Enabled = false;
                    EnableServiceGroupControl(false);
                }
                else
                {
                    btnSave.Enabled = true;
                    btnEdit.Enabled = true;
                    EnableServiceGroupControl(true);
                }
                serviceGroupFocus = data;
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private bool IsNotAdminAndCreator(string loginname, string creator)
        {
            bool isNotprivileged = false;
            try
            {
                if (!HIS.Desktop.IsAdmin.CheckLoginAdmin.IsAdmin(loginname)
                            && creator.Trim() != loginname)
                {
                    isNotprivileged = true;
                }
            }
            catch (Exception ex)
            {
                isNotprivileged = false;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return isNotprivileged;
        }

        private bool IsNotAdmin(string loginname)
        {
            bool isNotprivileged = false;
            try
            {
                if (!HIS.Desktop.IsAdmin.CheckLoginAdmin.IsAdmin(loginname))
                {
                    isNotprivileged = true;
                }
            }
            catch (Exception ex)
            {
                isNotprivileged = false;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return isNotprivileged;
        }

        private void InitDataToGridServiceGroup(ucServSegr uCServiceGroup)
        {
            try
            {
                int numPageSize;
                if (ucPagingServiceGroup.pagingGrid != null)
                {
                    numPageSize = ucPagingServiceGroup.pagingGrid.PageSize;
                }
                else
                {
                    numPageSize = (int)ConfigApplications.NumPageSize;
                }
                FillDataToGridServiceGroup(new CommonParam(0, numPageSize));

                CommonParam param = new CommonParam();
                param.Limit = rowCount;
                param.Count = dataTotal;
                ucPagingServiceGroup.Init(FillDataToGridServiceGroup, param, numPageSize, (GridControl)ServiceGroupProcessor.GetGridControl(ucGridControlServiceGroup));
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void FillDataToGridServiceGroup(object data)
        {
            try
            {
                WaitingManager.Show();
                //var listERSource = (List<ServiceGroupADO>)ERProcessor.GetDataGridView(ucGridControlER);
                listServiceGroup = new List<HIS_SERVICE_GROUP>();
                int start = ((CommonParam)data).Start ?? 0;
                int limit = ((CommonParam)data).Limit ?? 0;
                CommonParam param = new CommonParam(start, limit);
                MOS.Filter.HisServiceGroupFilter hisServiceGroupFilter = new HisServiceGroupFilter();
                hisServiceGroupFilter.ORDER_FIELD = "CREATE_TIME";
                hisServiceGroupFilter.ORDER_DIRECTION = "DESC";
                hisServiceGroupFilter.KEY_WORD = txtSearchServiceGroup.Text;
                if ((long)cboChooseBy.EditValue == 1)
                {
                    isChoseER = (long)cboChooseBy.EditValue;
                }
                if (IsNotAdmin(Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName()))
                {
                    hisServiceGroupFilter.CAN_VIEW_ACTIVE = true;

                }
                var rs = new Inventec.Common.Adapter.BackendAdapter(param).GetRO<List<MOS.EFMODEL.DataModels.HIS_SERVICE_GROUP>>(
                     "api/HisServiceGroup/Get",
                     HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer,
                     hisServiceGroupFilter,
                     param);
                lstServiceGroupADOs = new List<ServiceGroupADO>();

                if (rs != null && rs.Data.Count > 0)
                {
                    //List<V_HIS_ROOM> dataByRoom = new List<V_HIS_ROOM>();
                    listServiceGroup = rs.Data;
                    foreach (var item in listServiceGroup)
                    {
                        ServiceGroupADO ServiceGroupADO = new ServiceGroupADO(item);
                        if (isChoseER == 1)
                        {
                            ServiceGroupADO.isKeyChooseGroup = true;
                        }
                        lstServiceGroupADOs.Add(ServiceGroupADO);
                    }
                }

                //FILTER TEST CẦN SỬA LẠI 

                if (servSegr1s != null && servSegr1s.Count > 0)
                {
                    foreach (var item in servSegr1s)
                    {
                        var check = lstServiceGroupADOs.FirstOrDefault(o => o.ID == item.SERVICE_GROUP_ID);
                        if (check != null)
                        {
                            check.checkGroup = true;
                            check.AMOUNT = item.AMOUNT;
                        }
                    }
                }


                lstServiceGroupADOs = lstServiceGroupADOs.OrderByDescending(p => p.checkGroup).ToList();

                if (serviceGroupADORa != null && isChoseER == 1)
                {
                    var erADO = lstServiceGroupADOs.Where(o => o.ID == serviceGroupADORa.ID).FirstOrDefault();
                    if (erADO != null)
                    {
                        erADO.radioGroup = true;
                        lstServiceGroupADOs = lstServiceGroupADOs.OrderByDescending(p => p.radioGroup).ToList();
                    }
                }

                if (listCheckServiceGroupAdos != null && listCheckServiceGroupAdos.Count > 0)
                {
                    foreach (var item in listCheckServiceGroupAdos)
                    {
                        var kq = lstServiceGroupADOs.FirstOrDefault(o => o.ID == item.ID);
                        if (kq != null)
                        {
                            lstServiceGroupADOs.FirstOrDefault(o => o.ID == item.ID).checkGroup = item.checkGroup;
                            lstServiceGroupADOs.FirstOrDefault(o => o.ID == item.ID).AMOUNT = item.AMOUNT;
                        }
                    }
                }


                if (ucGridControlServiceGroup != null)
                {
                    ServiceGroupProcessor.Reload(ucGridControlServiceGroup, lstServiceGroupADOs);
                }
                rowCount = (data == null ? 0 : lstServiceGroupADOs.Count);
                dataTotal = (rs.Param == null ? 0 : rs.Param.Count ?? 0);
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void FillDataToGrid2(ucServSegr uCExecuteRoleUser)
        {
            try
            {
                int numPageSize;
                if (ucPagingService.pagingGrid != null)
                {
                    numPageSize = ucPagingService.pagingGrid.PageSize;
                }
                else
                {
                    numPageSize = (int)ConfigApplications.NumPageSize;
                }
                FillDataToGridService(new CommonParam(0, numPageSize));
                CommonParam param = new CommonParam();
                param.Limit = rowCount1;
                param.Count = dataTotal1;
                ucPagingService.Init(FillDataToGridService, param, numPageSize, this.gridControlService);
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

                if (cboActive.EditValue != null)
                {
                    if (Inventec.Common.TypeConvert.Parse.ToInt16((cboActive.EditValue ?? "0").ToString()) == 1)
                        serviceFilter.IS_ACTIVE = 1;
                    else
                    {
                        serviceFilter.IS_ACTIVE = 0;
                    }
                }

                if (cboServiceType.EditValue != null)
                {
                    serviceFilter.SERVICE_TYPE_ID = Inventec.Common.TypeConvert.Parse.ToInt64(cboServiceType.EditValue.ToString());
                }

                else
                {
                    serviceFilter.SERVICE_TYPE_IDs = new List<long> { IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__CDHA, IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__G, IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__GPBL, IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__KH, IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__KHAC, IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__NS, IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__PHCN, IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__PT, IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__SA, IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__TDCN, IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__TT, IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__XN, IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__AN };
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

                if (servSegrs != null && servSegrs.Count > 0)
                {
                    foreach (var item in servSegrs)
                    {
                        var check = lstServiceADOs.FirstOrDefault(o => o.ID == item.SERVICE_ID);
                        if (check != null)
                        {
                            check.check2 = true;
                            check.AMOUNT = item.AMOUNT;
                            check.NOTE = item.NOTE;

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
                            lstServiceADOs.FirstOrDefault(o => o.ID == item.ID).AMOUNT = item.AMOUNT;
                            lstServiceADOs.FirstOrDefault(o => o.ID == item.ID).NOTE = item.NOTE;
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
                LoadComboActive();
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
                status.Add(new Status(1, "Nhóm dịch vụ"));
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

        private void LoadComboActive()
        {
            try
            {
                List<Status> status = new List<Status>();
                status.Add(new Status(1, "Không khóa"));
                status.Add(new Status(2, "Khóa"));

                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("statusName", "", 300, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("statusName", "id", columnInfos, false, 350);
                ControlEditorLoader.Load(cboActive, status, controlEditorADO);
                //cboChooseBy.EditValue = status[0].id;
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
                Resources.ResourceLanguageManager.LanguageResource = new ResourceManager("HIS.Desktop.Plugins.HisServSegr.Resources.Lang", typeof(HIS.Desktop.Plugins.HisServSegr.ucServSegr).Assembly);

                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.layoutControl1.Text = Inventec.Common.Resource.Get.Value("ucServSegr.layoutControl1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnSave.Text = Inventec.Common.Resource.Get.Value("ucServSegr.btnSave.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn1.Caption = Inventec.Common.Resource.Get.Value("ucServSegr.gridColumn1.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn2.Caption = Inventec.Common.Resource.Get.Value("ucServSegr.gridColumn2.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn3.Caption = Inventec.Common.Resource.Get.Value("ucServSegr.gridColumn3.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn4.Caption = Inventec.Common.Resource.Get.Value("ucServSegr.gridColumn4.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn5.Caption = Inventec.Common.Resource.Get.Value("ucServSegr.gridColumn5.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn7.Caption = Inventec.Common.Resource.Get.Value("ucServSegr.gridColumn7.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnSearchService.Text = Inventec.Common.Resource.Get.Value("ucServSegr.btnSearchService.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.txtSearchService.Properties.NullValuePrompt = Inventec.Common.Resource.Get.Value("ucServSegr.txtSearchService.Properties.NullValuePrompt", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnSearchServiceGroup.Text = Inventec.Common.Resource.Get.Value("ucServSegr.btnSearchExecuteRole.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.txtSearchServiceGroup.Properties.NullValuePrompt = Inventec.Common.Resource.Get.Value("ucServSegr.txtSearchExecuteRole.Properties.NullValuePrompt", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboChooseBy.Properties.NullText = Inventec.Common.Resource.Get.Value("ucServSegr.cboChooseBy.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem5.Text = Inventec.Common.Resource.Get.Value("ucServSegr.layoutControlItem5.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        #endregion

        #region Event

        private void ServiceGroup_MouseDown(object sender, MouseEventArgs e)
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
                        if (hi.Column.FieldName == "checkGroup")
                        {
                            var lstCheckAll = lstServiceGroupADOs;
                            List<ServiceGroupADO> lstChecks = new List<ServiceGroupADO>();

                            if (lstCheckAll != null && lstCheckAll.Count > 0)
                            {
                                if (isCheckAll)
                                {
                                    foreach (var item in lstCheckAll)
                                    {
                                        if (item.ID != null)
                                        {
                                            item.checkGroup = true;
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
                                            item.checkGroup = false;
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

                                ServiceGroupProcessor.Reload(ucGridControlServiceGroup, lstChecks);
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

        private void btn_Radio_Enable_Click(HIS_SERVICE_GROUP data, ServiceGroupADO ERado)
        {
            try
            {
                serviceGroupGridView_Click(ERado);
                WaitingManager.Show();
                CommonParam param = new CommonParam();

                serviceGroupADORa = new ServiceGroupADO();
                serviceGroupADORa = ERado;
                //FILTER TEST CẦN SỬA LẠI


                HisServSegrFilter filter = new HisServSegrFilter();
                filter.SERVICE_GROUP_ID = data.ID;
                ERIdCheckByER = data.ID;
                servSegrs = new BackendAdapter(param).Get<List<HIS_SERV_SEGR>>(
                                "api/HisServSegr/Get",
                                HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer,
                                filter,
                                param);
                List<ServiceADO> dataNew = new List<ServiceADO>();
                dataNew = (from r in listService select new ServiceADO(r)).ToList();
                if (servSegrs != null && servSegrs.Count > 0)
                {
                    foreach (var item in servSegrs)
                    {
                        var check = dataNew.FirstOrDefault(o => o.ID == item.SERVICE_ID);
                        if (check != null)
                        {
                            check.check2 = true;
                            check.AMOUNT = item.AMOUNT;
                            check.NOTE = item.NOTE;
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

                HisServSegrFilter filter = new HisServSegrFilter();
                filter.SERVICE_ID = data.ID;
                ServiceIdCheckByService = data.ID;
                servSegr1s = new BackendAdapter(param).Get<List<HIS_SERV_SEGR>>(
                                "api/HisServSegr/Get",
                                HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer,
                                filter,
                                param);
                List<ServiceGroupADO> dataNew = new List<ServiceGroupADO>();
                dataNew = (from r in listServiceGroup select new ServiceGroupADO(r)).ToList();
                if (servSegr1s != null && servSegr1s.Count > 0)
                {

                    foreach (var item in servSegr1s)
                    {
                        var check = dataNew.FirstOrDefault(o => o.ID == item.SERVICE_GROUP_ID);
                        if (check != null)
                        {
                            check.checkGroup = true;
                            check.AMOUNT = item.AMOUNT;

                        }
                    }

                    dataNew = dataNew.OrderByDescending(p => p.checkGroup).ToList();
                    if (ucGridControlServiceGroup != null)
                    {
                        ServiceGroupProcessor.Reload(ucGridControlServiceGroup, dataNew);
                    }
                }
                else
                {
                    InitDataToGridServiceGroup(this);
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
                    InitDataToGridServiceGroup(this);
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
                InitDataToGridServiceGroup(this);
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
                listCheckServiceGroupAdos = new List<ServiceGroupADO>();
                listCheckServiceAdos = new List<ServiceADO>();
                serviceGroupADORa = null;
                serviceADORa = null;
                servSegrs = null;
                servSegr1s = null;
                InitDataToGridServiceGroup(this);
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

                    if (e.Column.FieldName == "AMOUNT")
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
                    if (e.Column.FieldName == " NOTE")
                    {
                        if (data.isKeyChoose1)
                        {
                            e.RepositoryItem = NoteD;
                        }
                        else
                        {
                            e.RepositoryItem = NoteE;
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
                            List<ServiceADO> lstChecks = gridControlService.DataSource as List<ServiceADO>;

                            if (lstChecks != null && lstChecks.Count > 0)
                            {
                                if (isCheckAll)
                                {
                                    lstChecks.ForEach(o =>
                                        {
                                            o.AMOUNT = 1;
                                            o.check2 = true;
                                        }
                                    );

                                    //foreach (var item in lstCheckAll)
                                    //{
                                    //    if (item.ID != null)
                                    //    {
                                    //        item.check2 = true;
                                    //        lstChecks.Add(item);
                                    //    }
                                    //    else
                                    //    {
                                    //        lstChecks.Add(item);
                                    //    }
                                    //}
                                    isCheckAll = false;
                                    hi.Column.Image = imageCollection.Images[1];
                                }
                                else
                                {
                                    lstChecks.ForEach(o =>
                                    {
                                        o.AMOUNT = 0;
                                        o.check2 = false;
                                    }
                                    );
                                    //foreach (var item in lstCheckAll)
                                    //{
                                    //    if (item.ID != null)
                                    //    {
                                    //        item.check2 = false;
                                    //        lstChecks.Add(item);
                                    //    }
                                    //    else
                                    //    {
                                    //        lstChecks.Add(item);
                                    //    }
                                    //}
                                    isCheckAll = true;
                                    hi.Column.Image = imageCollection.Images[0];
                                }


                                //ReloadData
                                //gridControlService.BeginUpdate();
                                gridControlService.RefreshDataSource();
                                //gridControlService.EndUpdate();
                                //??

                            }
                        }
                    }

                    if (hi.InRowCell && hi.Column.FieldName == "check2")
                    {
                        if (hi.Column.RealColumnEdit.GetType() == typeof(DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit))
                        {
                            view.FocusedRowHandle = hi.RowHandle;
                            view.FocusedColumn = hi.Column;
                            view.ShowEditor();
                            CheckEdit checkEdit = view.ActiveEditor as CheckEdit;
                            DevExpress.XtraEditors.ViewInfo.CheckEditViewInfo checkInfo = (DevExpress.XtraEditors.ViewInfo.CheckEditViewInfo)checkEdit.GetViewInfo();
                            Rectangle glyphRect = checkInfo.CheckInfo.GlyphRect;
                            Rectangle gridGlyphRect =
                                new Rectangle(viewInfo.GetGridCellInfo(hi).Bounds.X + glyphRect.X,
                                 viewInfo.GetGridCellInfo(hi).Bounds.Y + glyphRect.Y,
                                 glyphRect.Width,
                                 glyphRect.Height);
                            if (!gridGlyphRect.Contains(e.Location))
                            {
                                view.CloseEditor();
                                if (!view.IsCellSelected(hi.RowHandle, hi.Column))
                                {
                                    view.SelectCell(hi.RowHandle, hi.Column);
                                }
                                else
                                {
                                    view.UnselectCell(hi.RowHandle, hi.Column);
                                }
                            }
                            else
                            {
                                checkEdit.Checked = !checkEdit.Checked;
                                view.CloseEditor();
                            }
                            (e as DevExpress.Utils.DXMouseEventArgs).Handled = true;
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

                if (ucGridControlServiceGroup != null)
                {
                    object controlService = gridControlService.DataSource;
                    object ExecuteR = ServiceGroupProcessor.GetDataGridView(ucGridControlServiceGroup);
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
                                    HisServSegrFilter filter = new HisServSegrFilter();

                                    filter.SERVICE_GROUP_ID = ERIdCheckByER;

                                    var remunerationGet = new BackendAdapter(param).Get<List<HIS_SERV_SEGR>>(
                                      "api/HisServSegr/Get",
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

                                    //if ((dataCheckeds == null || dataCheckeds.Count == 0)
                                    //    && (dataUpdate == null || dataUpdate.Count == 0))
                                    //{
                                    //    MessageBox.Show("Vui lòng chọn dịch vụ", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                    //    WaitingManager.Hide();
                                    //    return;
                                    //}

                                    bool checkDelete = false;
                                    bool checkCreate = false;
                                    bool checkUpdate = false;

                                    //if (dataCheckeds.Count != erUser.Select(p => p.LOGINNAME).Count())
                                    //{

                                    serviceGroupADORa = new ServiceGroupADO();
                                    serviceGroupADORa = ((List<ServiceGroupADO>)ServiceGroupProcessor.GetDataGridView(ucGridControlServiceGroup)).FirstOrDefault(o => o.radioGroup == true);

                                    if (dataUpdate != null && dataUpdate.Count > 0)
                                    {
                                        List<HIS_SERV_SEGR> Updates = new List<HIS_SERV_SEGR>();
                                        foreach (var item in dataUpdate)
                                        {
                                            HIS_SERV_SEGR remuID = new HIS_SERV_SEGR();
                                            remuID.ID = remunerationGet.Where(o => o.SERVICE_ID == item.ID && o.SERVICE_GROUP_ID == ERIdCheckByER).FirstOrDefault().ID;
                                            remuID.SERVICE_GROUP_ID = ERIdCheckByER;
                                            remuID.SERVICE_ID = item.ID;
                                            remuID.AMOUNT = item.AMOUNT;

                                            remuID.NOTE = item.NOTE;
                                            string b = item.AMOUNT.ToString();

                                            if (item.AMOUNT < 0)
                                                validate = false;
                                            Updates.Add(remuID);
                                        }
                                        if (validate)
                                        {
                                            var updateResult = new BackendAdapter(param).Post<List<HIS_SERV_SEGR>>(
                                                       "/api/HisServSegr/UpdateList",
                                                       HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer,
                                                       Updates,
                                                       param);
                                            if (updateResult != null && updateResult.Count > 0)
                                            {
                                                resultSuccess = true;
                                                checkUpdate = true;
                                                foreach (var item in updateResult)
                                                {
                                                    servSegrs.FirstOrDefault(o => o.ID == item.ID).AMOUNT = item.AMOUNT;
                                                    servSegrs.FirstOrDefault(i => i.ID == item.ID).NOTE = item.NOTE;
                                                }
                                            }
                                        }
                                    }

                                    if (dataDeletes != null && dataDeletes.Count > 0)
                                    {
                                        List<long> deleteIds = remunerationGet.Where(o => dataDeletes.Select(p => p.ID)
                                            .Contains(o.SERVICE_ID)).Select(o => o.ID).ToList();
                                        bool deleteResult = new BackendAdapter(param).Post<bool>(
                                                  "/api/HisServSegr/DeleteList",
                                                  HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer,
                                                  deleteIds,
                                                  param);
                                        if (deleteResult)
                                        {
                                            resultSuccess = true;
                                            checkDelete = true;
                                            servSegrs = servSegrs.Where(o => !deleteIds.Contains(o.ID)).ToList();
                                        }
                                    }

                                    if (dataCreates != null && dataCreates.Count > 0)
                                    {
                                        List<HIS_SERV_SEGR> remuCreates = new List<HIS_SERV_SEGR>();
                                        foreach (var item in dataCreates)
                                        {
                                            HIS_SERV_SEGR remuCreate = new HIS_SERV_SEGR();
                                            remuCreate.SERVICE_ID = item.ID;
                                            remuCreate.AMOUNT = item.AMOUNT;

                                            remuCreate.NOTE = item.NOTE;

                                            if (item.AMOUNT < 0)
                                            {
                                                validate = false;
                                            }
                                            remuCreate.SERVICE_GROUP_ID = ERIdCheckByER;
                                            remuCreates.Add(remuCreate);
                                        }

                                        if (validate)
                                        {
                                            var createResult = new BackendAdapter(param).Post<List<HIS_SERV_SEGR>>(
                                                       "/api/HisServSegr/CreateList",
                                                       HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer,
                                                       remuCreates,
                                                       param);
                                            if (createResult != null && createResult.Count > 0)
                                            {
                                                resultSuccess = true;
                                                checkCreate = true;
                                                servSegrs.AddRange(createResult);
                                            }
                                        }
                                    }
                                    WaitingManager.Hide();

                                    if (resultSuccess == false && (dataCheckeds == null || dataCheckeds.Count == 0))
                                    {
                                        MessageBox.Show("Vui lòng chọn dịch vụ", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                        return;
                                    }

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
                                            listCheckServiceGroupAdos = new List<ServiceGroupADO>();
                                            listCheckServiceAdos = new List<ServiceADO>();
                                            data = data.OrderByDescending(p => p.check2).ToList();
                                            gridControlService.BeginUpdate();
                                            gridControlService.DataSource = data;
                                            gridControlService.EndUpdate();
                                        }
                                    }
                                    else
                                    {
                                        DevExpress.XtraEditors.XtraMessageBox.Show("Số lượng không được âm", "Thông báo");
                                    }
                                    WaitingManager.Hide();
                                }
                                else
                                {
                                    DevExpress.XtraEditors.XtraMessageBox.Show("Chưa chọn nhóm dịch vụ");
                                    WaitingManager.Hide();
                                }

                            }

                        }
                    }
                    if (isChoseService == 2)
                    {
                        //checkRa = false;
                        if (ExecuteR is List<ServiceGroupADO>)
                        {
                            var data = (List<ServiceGroupADO>)ExecuteR;

                            if (data != null && data.Count > 0)
                            {
                                if (checkRa == true)
                                {

                                    //Danh sach cac user duoc check
                                    MOS.Filter.HisServSegrFilter filter = new HisServSegrFilter();
                                    filter.SERVICE_ID = ServiceIdCheckByService;
                                    var remunerationGet1 = new BackendAdapter(param).Get<List<HIS_SERV_SEGR>>(
                                       "api/HisServSegr/Get",
                                       HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer,
                                       filter,
                                       param);

                                    var listERID = remunerationGet1.Select(p => p.SERVICE_GROUP_ID).ToList();

                                    var dataChecked = data.Where(p => p.checkGroup == true).ToList();
                                    var dataUpdate = dataChecked;
                                    //List xoa

                                    var dataDelete = data.Where(o => remunerationGet1.Select(p => p.SERVICE_GROUP_ID)
                                        .Contains(o.ID) && o.checkGroup == false).ToList();

                                    //list them
                                    var dataCreate = dataChecked.Where(o => !remunerationGet1.Select(p => p.SERVICE_GROUP_ID)
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
                                        List<HIS_SERV_SEGR> Updates = new List<HIS_SERV_SEGR>();
                                        foreach (var item in dataUpdate)
                                        {
                                            HIS_SERV_SEGR remuID = new HIS_SERV_SEGR();
                                            if (remunerationGet1 != null && remunerationGet1.Count > 0)
                                            {
                                                remuID.ID = remunerationGet1.Where(o => o.SERVICE_ID == ServiceIdCheckByService && o.SERVICE_GROUP_ID == item.ID).FirstOrDefault().ID;
                                            }
                                            remuID.SERVICE_GROUP_ID = item.ID;
                                            remuID.SERVICE_ID = ServiceIdCheckByService;
                                            remuID.AMOUNT = item.AMOUNT;








                                            if (item.AMOUNT < 0)
                                                validate = false;
                                            Updates.Add(remuID);
                                        }
                                        if (validate)
                                        {
                                            var updateResult = new BackendAdapter(param).Post<List<HIS_SERV_SEGR>>(
                                                       "/api/HisServSegr/UpdateList",
                                                       HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer,
                                                       Updates,
                                                       param);
                                            if (updateResult != null && updateResult.Count > 0)
                                            {
                                                resultSuccess = true;
                                                checkUpdate = true;
                                                foreach (var item in updateResult)
                                                {
                                                    servSegr1s.FirstOrDefault(o => o.ID == item.ID).AMOUNT = item.AMOUNT;
                                                    servSegr1s.FirstOrDefault(o => o.ID == item.ID).NOTE = item.NOTE;
                                                }
                                            }
                                        }
                                    }

                                    if (dataDelete != null && dataDelete.Count > 0)
                                    {
                                        List<long> deleteId = remunerationGet1.Where(o => dataDelete.Select(p => p.ID)

                                            .Contains(o.SERVICE_GROUP_ID)).Select(o => o.ID).ToList();
                                        bool deleteResult = new BackendAdapter(param).Post<bool>(
                                                  "/api/HisServSegr/DeleteList",
                                                  HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer,
                                                  deleteId,
                                                  param);
                                        if (deleteResult)
                                        {
                                            checkDelete = true;
                                            resultSuccess = true;
                                            servSegr1s = servSegr1s.Where(o => !deleteId.Contains(o.ID)).ToList();
                                        }

                                    }

                                    if (dataCreate != null && dataCreate.Count > 0)
                                    {
                                        List<HIS_SERV_SEGR> Creates = new List<HIS_SERV_SEGR>();
                                        foreach (var item in dataCreate)
                                        {
                                            HIS_SERV_SEGR remunerationID = new HIS_SERV_SEGR();
                                            remunerationID.SERVICE_GROUP_ID = item.ID;
                                            remunerationID.SERVICE_ID = ServiceIdCheckByService;
                                            remunerationID.AMOUNT = item.AMOUNT;
                                            //remunerationID.NOTE = txtNote.Text;








                                            if (item.AMOUNT < 0)
                                            {
                                                validate = false;
                                            }
                                            Creates.Add(remunerationID);
                                        }
                                        if (validate)
                                        {
                                            var createResult = new BackendAdapter(param).Post<List<HIS_SERV_SEGR>>(
                                                       "/api/HisServSegr/CreateList",
                                                       HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer,
                                                       Creates,
                                                       param);
                                            if (createResult != null && createResult.Count > 0)
                                            {
                                                checkCreate = true;
                                                resultSuccess = true;
                                                servSegr1s.AddRange(createResult);
                                            }
                                        }
                                    }


                                    WaitingManager.Hide();
                                    if (resultSuccess == false && (dataChecked == null || dataChecked.Count == 0))
                                    {
                                        MessageBox.Show("Vui lòng chọn nhóm dịch vụ", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                        return;
                                    }

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
                                            listCheckServiceGroupAdos = new List<ServiceGroupADO>();
                                            listCheckServiceAdos = new List<ServiceADO>();
                                            data = data.OrderByDescending(p => p.checkGroup).ToList();
                                            if (ucGridControlServiceGroup != null)
                                            {
                                                ServiceGroupProcessor.Reload(ucGridControlServiceGroup, data);
                                            }

                                        }
                                    }
                                    else
                                    {
                                        DevExpress.XtraEditors.XtraMessageBox.Show("Số lượng không được âm", "Thông báo");
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
                InitDataToGridServiceGroup(this);
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
                if (IsNotAdminAndCreator(Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName(), serviceGroupFocus.CREATOR))
                {
                    MessageBox.Show("Bạn không phải người tạo và không có quyền quản trị.", "Thông báo");
                    return;
                }
                btnSave.Focus();
                btnSave_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public void SaveServiceGroup()
        {
            try
            {
                if (btnSaveServiceGroup.Enabled)
                {
                    btnSaveServiceGroup.Focus();
                    btnSaveServiceGroup_Click(null, null);
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public void EditServiceGroup()
        {
            try
            {
                if (btnEdit.Enabled)
                {
                    btnEdit.Focus();
                    btnEdit_Click(null, null);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public void New()
        {
            try
            {
                if (btnNew.Enabled)
                {
                    btnNew.Focus();
                    btnNew_Click(null, null);
                }
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
                            listCheckServiceAdos.FirstOrDefault(o => o.ID == itemSources.ID).AMOUNT = itemSources.AMOUNT;
                            listCheckServiceAdos.FirstOrDefault(o => o.ID == itemSources.ID).NOTE = itemSources.NOTE;
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

        private void Check_CheckedChanged(ServiceGroupADO data)
        {
            try
            {
                bool success = false;
                var sources = (List<ServiceGroupADO>)ServiceGroupProcessor.GetDataGridView(ucGridControlServiceGroup);
                var itemSources = sources.FirstOrDefault(o => o.ID == data.ID);
                if (listCheckServiceGroupAdos != null && listCheckServiceGroupAdos.Count > 0)
                {
                    foreach (var item in listCheckServiceGroupAdos)
                    {
                        if (data.ID == item.ID)
                        {
                            listCheckServiceGroupAdos.FirstOrDefault(o => o.ID == itemSources.ID).checkGroup = itemSources.checkGroup;
                            listCheckServiceGroupAdos.FirstOrDefault(o => o.ID == itemSources.ID).AMOUNT = itemSources.AMOUNT;
                            if (itemSources.checkGroup)
                            {
                                listCheckServiceGroupAdos.FirstOrDefault(o => o.ID == itemSources.ID).AMOUNT = 1;
                            }
                            else
                            {
                                listCheckServiceGroupAdos.FirstOrDefault(o => o.ID == itemSources.ID).AMOUNT = 0;
                            }
                            success = true;
                            break;
                        }
                    }
                    if (!success)
                    {

                        if (itemSources.checkGroup)
                        {
                            itemSources.AMOUNT = 1;
                        }
                        else
                        {
                            itemSources.AMOUNT = 0;
                        }
                        listCheckServiceGroupAdos.Add(itemSources);
                    }
                }
                else
                {
                    if (itemSources.checkGroup)
                    {
                        itemSources.AMOUNT = 1;
                    }
                    else
                    {
                        itemSources.AMOUNT = 0;
                    }
                    listCheckServiceGroupAdos.Add(itemSources);
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
                            listCheckServiceAdos.FirstOrDefault(o => o.ID == itemSources.ID).AMOUNT = itemSources.AMOUNT;

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

        private void Spin_EditValueChanged(ServiceGroupADO data)
        {
            try
            {
                bool success = false;
                var sources = (List<ServiceGroupADO>)ServiceGroupProcessor.GetDataGridView(ucGridControlServiceGroup);
                var itemSources = sources.FirstOrDefault(o => o.ID == data.ID);
                if (listCheckServiceGroupAdos != null && listCheckServiceGroupAdos.Count > 0)
                {
                    foreach (var item in listCheckServiceGroupAdos)
                    {
                        if (data.ID == item.ID)
                        {
                            //listCheckERAdos.FirstOrDefault(o => o.ID == itemSources.ID).check1 = itemSources.check1;
                            listCheckServiceGroupAdos.FirstOrDefault(o => o.ID == itemSources.ID).AMOUNT = itemSources.AMOUNT;
                            success = true;
                            break;
                        }
                    }
                    if (!success)
                    {
                        listCheckServiceGroupAdos.Add(itemSources);
                    }
                }
                else
                {
                    listCheckServiceGroupAdos.Add(itemSources);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void ValidateForm()
        {
            try
            {
                ValidationSingleControl(txtServiceGroupCode);
                ValidationSingleControl(txtServiceGroupName);

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ValidationSingleControl(BaseEdit control)
        {
            try
            {
                ControlEditValidationRule validRule = new ControlEditValidationRule();
                validRule.editor = control;
                validRule.ErrorText = MessageUtil.GetMessage(LibraryMessage.Message.Enum.TruongDuLieuBatBuoc);
                validRule.ErrorType = DevExpress.XtraEditors.DXErrorProvider.ErrorType.Warning;
                dxValidationProviderEditorInfo.SetValidationRule(control, validRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        void EnableServiceGroupControl(bool stateControl)
        {
            try
            {
                txtServiceGroupCode.Enabled = stateControl;
                txtServiceGroupName.Enabled = stateControl;
                chkIsPublic.Enabled = stateControl;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        void SaveServiceGroupProcess(ref HIS_SERVICE_GROUP serviceGroup)
        {
            try
            {

                serviceGroup.SERVICE_GROUP_CODE = txtServiceGroupCode.Text.Trim();
                serviceGroup.SERVICE_GROUP_NAME = txtServiceGroupName.Text.Trim();

                if (spinEdit_NumOrder.EditValue != null)
                {
                    serviceGroup.NUM_ORDER = (short)(spinEdit_NumOrder.Value);
                }
                else
                {
                    serviceGroup.NUM_ORDER = null;
                }

                if (chkIsPublic.CheckState == CheckState.Checked)
                {
                    serviceGroup.IS_PUBLIC = 1;
                }
                else
                {
                    serviceGroup.IS_PUBLIC = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SaveClick()
        {
            try
            {
                bool success = false;
                HIS_SERVICE_GROUP serviceGroupResult = null;
                CommonParam param = new CommonParam();
                positionHandle = -1;
                if (!dxValidationProviderEditorInfo.Validate())
                    return;
                WaitingManager.Show();
                if (this.ActionType == HIS.Desktop.LocalStorage.LocalData.GlobalVariables.ActionAdd)
                {
                    HIS_SERVICE_GROUP serviceGroup = new HIS_SERVICE_GROUP();
                    SaveServiceGroupProcess(ref serviceGroup);
                    serviceGroupResult = new BackendAdapter(param).Post<HIS_SERVICE_GROUP>(
                                             HisRequestUriStore.HIS_SERVICE_GROUP_CREATE,
                                             HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer,
                                             serviceGroup,
                                             param);
                    if (serviceGroupResult != null)
                    {
                        ServiceGroupADO ServiceGroupADOResult = new ServiceGroupADO(serviceGroupResult);
                        lstServiceGroupADOs.Add(ServiceGroupADOResult);
                        ServiceGroupProcessor.Reload(this.ucGridControlServiceGroup, lstServiceGroupADOs);
                    }

                }
                else if (this.ActionType == HIS.Desktop.LocalStorage.LocalData.GlobalVariables.ActionEdit)
                {
                    HIS_SERVICE_GROUP serviceGroupFS = new HIS_SERVICE_GROUP();
                    AutoMapper.Mapper.CreateMap<ServiceGroupADO, HIS_SERVICE_GROUP>();
                    serviceGroupFS = AutoMapper.Mapper.Map<HIS_SERVICE_GROUP>(serviceGroupFocus);
                    SaveServiceGroupProcess(ref serviceGroupFS);
                    serviceGroupResult = new BackendAdapter(param).Post<HIS_SERVICE_GROUP>(
                                         HisRequestUriStore.HIS_SERVICE_GROUP_UPDATE,
                                         HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer,
                                         serviceGroupFS,
                                         param);
                    if (serviceGroupResult != null)
                    {
                        var serviceGroupCheck = lstServiceGroupADOs.FirstOrDefault(o => o.ID == serviceGroupResult.ID);
                        if (serviceGroupCheck != null)
                        {
                            Parallel.ForEach(lstServiceGroupADOs.Where(f => f.ID == serviceGroupResult.ID), l => l.SERVICE_GROUP_CODE = serviceGroupResult.SERVICE_GROUP_CODE);
                            Parallel.ForEach(lstServiceGroupADOs.Where(f => f.ID == serviceGroupResult.ID), l => l.SERVICE_GROUP_NAME = serviceGroupResult.SERVICE_GROUP_NAME);
                            Parallel.ForEach(lstServiceGroupADOs.Where(f => f.ID == serviceGroupResult.ID), l => l.IS_PUBLIC = serviceGroupResult.IS_PUBLIC);
                            Parallel.ForEach(lstServiceGroupADOs.Where(f => f.ID == serviceGroupResult.ID), l => l.IsPublic = (serviceGroupResult.IS_PUBLIC == 1));
                            Parallel.ForEach(lstServiceGroupADOs.Where(f => f.ID == serviceGroupResult.ID), l => l.NUM_ORDER = serviceGroupResult.NUM_ORDER);
                            ServiceGroupProcessor.Reload(this.ucGridControlServiceGroup, lstServiceGroupADOs);
                        }
                    }
                }
                if (serviceGroupResult != null)
                {
                    success = true;
                }
                WaitingManager.Hide();
                #region Hien thi message thong bao
                MessageManager.Show(this.ParentForm, param, success);
                #endregion

                #region Neu phien lam viec bi mat, phan mem tu dong logout va tro ve trang login
                SessionManager.ProcessTokenLost(param);
                #endregion
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnSaveServiceGroup_Click(object sender, EventArgs e)
        {
            SaveClick();
        }

        private void btnNew_Click(object sender, EventArgs e)
        {
            try
            {

                spinEdit_NumOrder.EditValue = null;
                btnEdit.Enabled = false;
                EnableServiceGroupControl(true);
                btnSaveServiceGroup.Enabled = true;
                SetDataToServiceGroupControl(null);
                this.ActionType = HIS.Desktop.LocalStorage.LocalData.GlobalVariables.ActionAdd;
                Inventec.Desktop.Controls.ControlWorker.ValidationProviderRemoveControlError(dxValidationProviderEditorInfo, dxErrorProvider1);
                txtServiceGroupCode.Focus();
                txtServiceGroupCode.SelectAll();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            try
            {
                SaveClick();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            try
            {
                EnableServiceGroupControl(false);
                Inventec.Desktop.Controls.ControlWorker.ValidationProviderRemoveControlError(dxValidationProviderEditorInfo, dxErrorProvider1);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtServiceGroupCode_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtServiceGroupName.Focus();
                    txtServiceGroupName.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtServiceGroupName_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    spinEdit_NumOrder.Focus();
                    spinEdit_NumOrder.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void chkIsPublic_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    btnSave.Focus();
                    e.Handled = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewService_CellValueChanged(object sender, CellValueChangedEventArgs e)
        {
            try
            {
                if (e.RowHandle >= 0 && e.Column.FieldName == "check2")
                {
                    gridViewService.BeginUpdate();
                    bool checkItem = Inventec.Common.TypeConvert.Parse.ToBoolean(gridViewService.GetRowCellValue(e.RowHandle, "check2").ToString());
                    decimal amountItem = Inventec.Common.TypeConvert.Parse.ToDecimal(gridViewService.GetRowCellValue(e.RowHandle, "AMOUNT").ToString());
                    //var note = gridViewService.GetRowCellValue(e.RowHandle, "NOTE").ToString();
                    //if (checkItem && note == "")
                    //{
                    //    gridViewService.SetRowCellValue(e.RowHandle, "NOTE", 1);
                    //}
                    //else
                    //{
                    //    gridViewService.SetRowCellValue(e.RowHandle, "NOTE", 0);
                    //}
                    if (checkItem && amountItem == 0)
                    {
                        gridViewService.SetRowCellValue(e.RowHandle, "AMOUNT", 1);
                    }
                    else
                    {
                        gridViewService.SetRowCellValue(e.RowHandle, "AMOUNT", 0);
                    }

                    gridViewService.EndUpdate();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboServiceType_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    FillDataToGrid2(this);
                    cboServiceType.Properties.Buttons[1].Visible = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboServiceType_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    cboServiceType.Properties.Buttons[1].Visible = true;
                    cboServiceType.EditValue = null;
                    FillDataToGrid2(this);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboServiceType_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    FillDataToGrid2(this);
                    txtSearchService.Focus();
                    txtSearchService.SelectAll();
                }
                else
                {
                    cboServiceType.ShowPopup();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void spinEdit_NumOrder_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    chkIsPublic.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void panelControlServiceGroup_Paint(object sender, PaintEventArgs e)
        {

        }

        private void gridControlService_Click(object sender, EventArgs e)
        {

        }

        private void gridViewService_ValidateRow(object sender, ValidateRowEventArgs e)
        {
            if (e.RowHandle < 0)
            {
                XtraMessageBox.Show(gridViewService.GetRowCellValue(e.RowHandle, "NOTE").ToString());
            }

        }

        private void cboChooseBy_EditValueChanged(object sender, EventArgs e)
        {

        }

        private void NoteE_EditValueChanged(object sender, EventArgs e)
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
                            listCheckServiceAdos.FirstOrDefault(o => o.ID == itemSources.ID).NOTE = itemSources.NOTE;

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

    }
}
