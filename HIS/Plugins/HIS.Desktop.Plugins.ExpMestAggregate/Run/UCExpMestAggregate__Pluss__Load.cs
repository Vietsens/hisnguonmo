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
using AutoMapper;
using DevExpress.Data;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Controls.Session;
using HIS.Desktop.LibraryMessage;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.ConfigApplication;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Plugins.ExpMestAggregate.ADO;
using HIS.Desktop.Utilities.Extensions;
using Inventec.Common.Adapter;
using Inventec.Common.Logging;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.ExpMestAggregate
{
    public partial class UCExpMestAggregate : HIS.Desktop.Utility.UserControlBase
    {
        List<ExpMestADO> _ExpMestADOs = new List<ExpMestADO>();
        List<V_HIS_EXP_MEST> _ExpMest_PLs = new List<V_HIS_EXP_MEST>();
        /// <summary>
        /// Khoi tao du lieu danh sach phong
        /// </summary>
        private void FillDataNavListRoom()
        {
            try
            {
                WaitingManager.Show();
                navBarFilterProcess.BeginUpdate();
                int dem = 0;
                long roomTypeIdDV = IMSys.DbConfig.HIS_RS.HIS_ROOM_TYPE.ID__XL;
                long roomTypeIdGI = IMSys.DbConfig.HIS_RS.HIS_ROOM_TYPE.ID__BUONG;
                long departmentId = WorkPlace.WorkPlaceSDO.FirstOrDefault(p => p.RoomId == this.currentModule.RoomId).DepartmentId;
                var lstRoomWithDepartment = BackendDataWorker.Get<V_HIS_ROOM>().Where(o => o.DEPARTMENT_ID == departmentId && (o.ROOM_TYPE_ID == roomTypeIdGI || o.ROOM_TYPE_ID == roomTypeIdDV) && o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList();
                if (lstRoomWithDepartment != null && lstRoomWithDepartment.Count > 0)
                {
                    navBarGroupControlContainerRoom.Controls.Clear();
                    foreach (var item in lstRoomWithDepartment)
                    {
                        DevExpress.XtraEditors.CheckEdit checkEditRoom = new CheckEdit();
                        navBarGroupControlContainerRoom.Controls.Add(checkEditRoom);
                        checkEditRoom.Location = new System.Drawing.Point(20, 3 + (dem * 26));
                        checkEditRoom.Name = item.ID.ToString();
                        checkEditRoom.Properties.Caption = item.ROOM_NAME;
                        checkEditRoom.Size = new System.Drawing.Size(210, 19);
                        checkEditRoom.TabIndex = dem;
                        checkEditRoom.EnterMoveNextControl = true;
                        checkEditRoom.CheckedChanged += new System.EventHandler(checkRoom);

                        dem++;
                    }
                    navBarFilterProcess.EndUpdate();
                }
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void checkRoom(object sender, EventArgs e)
        {
            if (navBarFilterProcess.Controls.Count > 0)
            {
                for (int i = 0; i < navBarFilterProcess.Controls.Count; i++)
                {
                    if (navBarFilterProcess.Controls[i] is DevExpress.XtraNavBar.NavBarGroupControlContainer)
                        continue;
                    if (navBarFilterProcess.Controls[i] is DevExpress.XtraNavBar.NavBarGroupControlContainerWrapper)
                    {
                        var groupWrapper = navBarFilterProcess.Controls[i] as DevExpress.XtraNavBar.NavBarGroupControlContainerWrapper;
                        string name = groupWrapper.Name;
                        foreach (DevExpress.XtraNavBar.NavBarGroupControlContainer group in groupWrapper.Controls)
                        {
                            foreach (var itemCheckEdit in group.Controls)
                            {
                                if (itemCheckEdit is CheckEdit)
                                {
                                    var checkEdit = itemCheckEdit as CheckEdit;
                                    if (checkEdit.Checked)
                                    {
                                        if (group.Name == "navBarGroupControlContainerRoom")
                                        {
                                            lstRoomSelectedId.Add(Inventec.Common.TypeConvert.Parse.ToInt64(checkEdit.Name));
                                            lstRoomSelectedId = lstRoomSelectedId.Distinct().ToList();
                                        }
                                    }
                                    else
                                    {
                                        lstRoomSelectedId.RemoveAll(o => o == Inventec.Common.TypeConvert.Parse.ToInt64(checkEdit.Name));
                                    }
                                }
                            }
                        }


                    }
                }
                CommonParam paramBedRoom = new CommonParam();
                HisBedRoomFilter filterBedRoom = new HisBedRoomFilter();
                if (lstRoomSelectedId != null && lstRoomSelectedId.Count() > 0)
                    filterBedRoom.ROOM_IDs = lstRoomSelectedId;
                var result = new BackendAdapter(paramBedRoom).Get<List<HIS_BED_ROOM>>("api/HisBedRoom/Get", ApiConsumers.MosConsumer, filterBedRoom, paramBedRoom);
                if (result != null && result.Count() > 0)
                {
                    foreach (var item in result)
                    {
                        lstBedRoomIds = new List<long>();
                        lstBedRoomIds.Add(item.ID);
                    }
                }
            }
            InitcboBed();
        }

        /// <summary>
        /// Khoi tao du lieu danh sach don thuoc
        /// </summary>
        private void LoadDataExpMestThuocNT()
        {
            try
            {
                WaitingManager.Show();
                #region Filter
                MOS.Filter.HisExpMestViewFilter _expMestFilter = new HisExpMestViewFilter();
                _expMestFilter.KEY_WORD = txtKeywordProcess.Text.Trim();
                if (_lstCurrentBedId != null && _lstCurrentBedId.Count() > 0)
                    _expMestFilter.CURRENT_BED_IDs = this._lstCurrentBedId.Distinct().ToList();
                _expMestFilter.IS_NOT_TAKEN = false;
                _expMestFilter.ORDER_FIELD = "MODIFY_TIME";
                _expMestFilter.ORDER_DIRECTION = "DESC";
                _expMestFilter.REQ_DEPARTMENT_ID = WorkPlace.WorkPlaceSDO.FirstOrDefault(p => p.RoomId == this.currentModule.RoomId).DepartmentId; // filter theo khoa yeu cau(khoa cua nguoi dung dang dang nhap)
                //==>: Mac DInh Chon Tat Ca Cac Khoa

                _expMestFilter.EXP_MEST_TYPE_ID = IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__DDT;//La Don Thuoc Noi Tru

                if (chkNotSynthetic.Checked == true && chkSynthesized.Checked == false)
                {
                    //HAS_AGGR 
                    //Nếu chưa thuộc phiếu nào thì = false
                    //Nếu đã đc tổng hợp = true
                    //Chua thuoc phieu nao va phieu do chua xuat
                    _expMestFilter.HAS_AGGR = false;
                    _expMestFilter.EXP_MEST_STT_ID = IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__REQUEST;
                }
                else if (chkNotSynthetic.Checked == false && chkSynthesized.Checked == true)
                    _expMestFilter.HAS_AGGR = true;
                #endregion
                if (_MediStockSelecteds != null && _MediStockSelecteds.Count > 0)
                {
                    _expMestFilter.MEDI_STOCK_IDs = _MediStockSelecteds.Select(p => p.ID).Distinct().ToList();
                }
                if(cboPatientType.EditValue != null && !string.IsNullOrEmpty(cboPatientType.EditValue.ToString()))
				{
                   _expMestFilter.TDL_PATIENT_TYPE_ID = Int64.Parse(cboPatientType.EditValue.ToString());
				}                    
                #region DateTime
                if (dtFromIntructionTime.EditValue != null && dtFromIntructionTime.DateTime != DateTime.MinValue)
                {
                    _expMestFilter.TDL_INTRUCTION_TIME_FROM = Inventec.Common.TypeConvert.Parse.ToInt64(Convert.ToDateTime(dtFromIntructionTime.EditValue).ToString("yyyyMMddHHmmss"));
                }
                if (dtToIntructionTime.EditValue != null && dtToIntructionTime.DateTime != DateTime.MinValue)
                {
                    _expMestFilter.TDL_INTRUCTION_TIME_TO = Inventec.Common.TypeConvert.Parse.ToInt64(Convert.ToDateTime(dtToIntructionTime.EditValue).ToString("yyyyMMddHHmmss"));
                }
                if (dteUseTimeFrom.EditValue != null && dteUseTimeFrom.DateTime != DateTime.MinValue)
                {
                    _expMestFilter.TDL_USE_TIME_FROM = Inventec.Common.TypeConvert.Parse.ToInt64(Convert.ToDateTime(dteUseTimeFrom.EditValue).ToString("yyyyMMdd") + "000000");
                }
                if (dteUseTimeTo.EditValue != null && dteUseTimeTo.DateTime != DateTime.MinValue)
                {
                    _expMestFilter.TDL_USE_TIME_TO = Inventec.Common.TypeConvert.Parse.ToInt64(Convert.ToDateTime(dteUseTimeTo.EditValue).ToString("yyyyMMdd") + "235959");
                }
                #endregion

                #region navBarFilterProcess
                try
                {
                    if (navBarFilterProcess.Controls.Count > 0)
                    {
                        for (int i = 0; i < navBarFilterProcess.Controls.Count; i++)
                        {
                            if (navBarFilterProcess.Controls[i] is DevExpress.XtraNavBar.NavBarGroupControlContainer)
                                continue;
                            if (navBarFilterProcess.Controls[i] is DevExpress.XtraNavBar.NavBarGroupControlContainerWrapper)
                            {
                                var groupWrapper = navBarFilterProcess.Controls[i] as DevExpress.XtraNavBar.NavBarGroupControlContainerWrapper;
                                string name = groupWrapper.Name;
                                foreach (DevExpress.XtraNavBar.NavBarGroupControlContainer group in groupWrapper.Controls)
                                {
                                    foreach (var itemCheckEdit in group.Controls)
                                    {
                                        if (itemCheckEdit is CheckEdit)
                                        {
                                            var checkEdit = itemCheckEdit as CheckEdit;
                                            if (checkEdit.Checked)
                                            {
                                                if (group.Name == "navBarGroupControlContainerRoom")
                                                {
                                                    if (_expMestFilter.REQ_ROOM_IDs == null)
                                                        _expMestFilter.REQ_ROOM_IDs = new List<long>();
                                                    _expMestFilter.REQ_ROOM_IDs.Add(Inventec.Common.TypeConvert.Parse.ToInt64(checkEdit.Name));
                                                    // load data combobed
                                                }
                                            }
                                        }
                                    }
                                }


                            }
                        }

                    }
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Error(ex);
                }
                #endregion

                CommonParam param = new CommonParam();
                _ExpMestADOs = new List<ExpMestADO>();

                Inventec.Common.Logging.LogSystem.Info("_expMestFilter: " + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => _expMestFilter), _expMestFilter));

                var _ExpMest_NTs = new BackendAdapter(param).Get<List<V_HIS_EXP_MEST>>(HisRequestUriStore.HIS_EXP_MEST_GETVIEW, ApiConsumers.MosConsumer, _expMestFilter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, param);
                if (_ExpMest_NTs != null && _ExpMest_NTs.Count > 0)
                {

                    if (checkPresNormal.Checked && !checkIHomePres.Checked && !checkPresKidney.Checked)
                    {
                        _ExpMest_NTs = _ExpMest_NTs.Where(o => o.IS_HOME_PRES == null && o.IS_KIDNEY == null).ToList();
                    }
                    else if (checkPresNormal.Checked && checkIHomePres.Checked && !checkPresKidney.Checked)
                    {
                        _ExpMest_NTs = _ExpMest_NTs.Where(o => (o.IS_HOME_PRES == 1) || (o.IS_HOME_PRES == null && o.IS_KIDNEY == null)).ToList();
                    }
                    else if (checkPresNormal.Checked && !checkIHomePres.Checked && checkPresKidney.Checked)
                    {
                        _ExpMest_NTs = _ExpMest_NTs.Where(o => (o.IS_KIDNEY == 1) || (o.IS_HOME_PRES == null && o.IS_KIDNEY == null)).ToList();
                    }
                    else if (checkPresNormal.Checked && checkIHomePres.Checked && checkPresKidney.Checked)
                    {
                        _ExpMest_NTs = _ExpMest_NTs.Where(o => (o.IS_HOME_PRES == 1) || (o.IS_KIDNEY == 1) || (o.IS_HOME_PRES == null && o.IS_KIDNEY == null)).ToList();
                    }
                    else if (!checkPresNormal.Checked && !checkIHomePres.Checked && !checkPresKidney.Checked)
                    {
                        _ExpMest_NTs = _ExpMest_NTs.Where(o => o.IS_HOME_PRES != null && o.IS_KIDNEY != null).ToList();
                    }
                    else if (!checkPresNormal.Checked && checkIHomePres.Checked && !checkPresKidney.Checked)
                    {
                        _ExpMest_NTs = _ExpMest_NTs.Where(o => o.IS_HOME_PRES == 1).ToList();
                    }
                    else if (!checkPresNormal.Checked && !checkIHomePres.Checked && checkPresKidney.Checked)
                    {
                        _ExpMest_NTs = _ExpMest_NTs.Where(o => o.IS_KIDNEY == 1).ToList();
                    }
                    else if (!checkPresNormal.Checked && checkIHomePres.Checked && checkPresKidney.Checked)
                    {
                        _ExpMest_NTs = _ExpMest_NTs.Where(o => (o.IS_HOME_PRES == 1) || (o.IS_KIDNEY == 1)).ToList();
                    }

                    foreach (var item in _ExpMest_NTs)
                    {
                        ExpMestADO ado = new ExpMestADO();
                        Inventec.Common.Mapper.DataObjectMapper.Map<V_HIS_EXP_MEST>(ado, item);
                        if (item.EXP_MEST_STT_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__REQUEST && (item.AGGR_EXP_MEST_ID == null || item.AGGR_EXP_MEST_ID <= 0))
                        {
                            ado.IsCheck = true;
                        }
                        _ExpMestADOs.Add(ado);
                    }
                }
                WaitingManager.Hide();
            }

            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void FillDataToGridControlExpMestReq()
        {
            try
            {
                gridControlExpMestReq.DataSource = null;
                gridControlExpMestReq.DataSource = _ExpMestADOs;
                isCheckAll = false;
                gridColumnCheck.Image = imageListIcon.Images[5];
                this.LoadExpMestDetail();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>
        /// Khoi tao du lieu danh sach phieu linh
        /// </summary>
        /// 
        private void LoadDataAggrExpMest()
        {
            try
            {
                int pageSize = ucPagingControlAggrExpMest.pagingGrid != null ? ucPagingControlAggrExpMest.pagingGrid.PageSize : (int)ConfigApplications.NumPageSize;
                PagingAggrExpMest(new CommonParam(0, pageSize));

                CommonParam param = new CommonParam();
                param.Limit = rowCount;
                param.Count = dataTotal;
                ucPagingControlAggrExpMest.Init(PagingAggrExpMest, param, pageSize, gridControlAggrExpMest);
                if (_ExpMest_PLs != null)
                    gridControlAggrExpMest.DataSource = _ExpMest_PLs;
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }
        private void PagingAggrExpMest(object param)
        {
            try
            {
                int start = ((CommonParam)param).Start ?? 0;
                int limit = ((CommonParam)param).Limit ?? 0;
                CommonParam paramCommon = new CommonParam(start, limit);
                gridControlAggrExpMest.DataSource = null;

                HisExpMestViewFilter filter = new HisExpMestViewFilter();
                filter.REQ_DEPARTMENT_ID = WorkPlace.WorkPlaceSDO.FirstOrDefault(p => p.RoomId == this.currentModule.RoomId).DepartmentId;
                filter.EXP_MEST_TYPE_ID = IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__PL;
                filter.ORDER_FIELD = "MODIFY_TIME";
                filter.ORDER_DIRECTION = "DESC";
                var apiResult = new BackendAdapter(paramCommon).GetRO<List<V_HIS_EXP_MEST>>(HisRequestUriStore.HIS_EXP_MEST_GETVIEW, ApiConsumers.MosConsumer, filter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, paramCommon);
                _ExpMest_PLs = new List<V_HIS_EXP_MEST>();
                if (apiResult != null)
                {
                    _ExpMest_PLs = (List<V_HIS_EXP_MEST>)apiResult.Data;
                    if (_ExpMest_PLs != null)
                    {
                        rowCount = _ExpMest_PLs.Count;
                        dataTotal = apiResult.Param.Count ?? 0;
                    }
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                LogSystem.Error(ex);
            }
        }

        /// <summary>
        /// Khoi tao du lieu chi tieu phieu linh
        /// </summary>
        private void LoadDetailAggrExpMestByAggrExpMestId()
        {
            try
            {
                WaitingManager.Show();
                int pageSize = ucPagingAggregateRequest.pagingGrid != null ? ucPagingAggregateRequest.pagingGrid.PageSize : (int)ConfigApplications.NumPageSize;
                PagingDetailAggrExpMest(new CommonParam(0, pageSize));

                CommonParam param = new CommonParam();
                param.Limit = rowCountExpM;
                param.Count = dataTotalExpM;
                ucPagingAggregateRequest.Init(PagingDetailAggrExpMest, param, pageSize, gridControlAggregateRequest);

                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                WaitingManager.Hide();
            }
        }
        private void PagingDetailAggrExpMest(object param)
        {
            try
            {
                WaitingManager.Show();
                int start = ((CommonParam)param).Start ?? 0;
                int limit = ((CommonParam)param).Limit ?? 0;
                CommonParam paramCommon = new CommonParam(start, limit);
                gridControlAggregateRequest.DataSource = null;
                MOS.Filter.HisExpMestViewFilter expMestFilter = new HisExpMestViewFilter();
                expMestFilter.AGGR_EXP_MEST_ID = this.aggrExpMestId;
                expMestFilter.ORDER_DIRECTION = "DESC";
                expMestFilter.ORDER_FIELD = "MODIFY_TIME";
                var apiResult = new BackendAdapter(paramCommon).GetRO<List<V_HIS_EXP_MEST>>(HisRequestUriStore.HIS_EXP_MEST_GETVIEW, ApiConsumers.MosConsumer, expMestFilter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, paramCommon);

                if (apiResult != null)
                {
                    var data = (List<V_HIS_EXP_MEST>)apiResult.Data;
                    if (data != null)
                    {
                        gridControlAggregateRequest.DataSource = data;
                        rowCountExpM = data.Count;
                        dataTotalExpM = apiResult.Param.Count ?? 0;
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

        List<V_HIS_MEDI_STOCK> _MediStockSelecteds;
        private void InitComboMediStock()
        {
            try
            {
                List<V_HIS_MEDI_STOCK> _mediStocks = new List<V_HIS_MEDI_STOCK>();
                var datas = BackendDataWorker.Get<V_HIS_MEST_ROOM>().Where(p => p.ROOM_ID == this.currentModule.RoomId).ToList();
                if (datas != null)
                {
                    List<long> mediStockIds = datas.Select(p => p.MEDI_STOCK_ID).ToList();
                    _mediStocks = BackendDataWorker.Get<V_HIS_MEDI_STOCK>().Where(p => mediStockIds.Contains(p.ID) && p.IS_CABINET != 1).ToList();
                }
                cboMediStock.Properties.DataSource = _mediStocks;
                cboMediStock.Properties.DisplayMember = "MEDI_STOCK_NAME";
                cboMediStock.Properties.ValueMember = "ID";
                DevExpress.XtraGrid.Columns.GridColumn col2 = cboMediStock.Properties.View.Columns.AddField("MEDI_STOCK_NAME");
                col2.VisibleIndex = 1;
                col2.Width = 200;
                col2.Caption = Resources.ResourceMessage.TatCa;
                cboMediStock.Properties.PopupFormWidth = 200;
                cboMediStock.Properties.View.OptionsView.ShowColumnHeaders = true;
                cboMediStock.Properties.View.OptionsSelection.MultiSelect = true;
                GridCheckMarksSelection gridCheckMark = cboMediStock.Properties.Tag as GridCheckMarksSelection;
                if (gridCheckMark != null)
                {
                    gridCheckMark.SelectAll(cboMediStock.Properties.DataSource);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void InitMediStockCheck()
        {
            try
            {
                GridCheckMarksSelection gridCheck = new GridCheckMarksSelection(cboMediStock.Properties);
                gridCheck.SelectionChanged += new GridCheckMarksSelection.SelectionChangedEventHandler(SelectionGrid__MediStock);
                cboMediStock.Properties.Tag = gridCheck;
                cboMediStock.Properties.View.OptionsSelection.MultiSelect = true;
                GridCheckMarksSelection gridCheckMark = cboMediStock.Properties.Tag as GridCheckMarksSelection;
                if (gridCheckMark != null)
                {
                    gridCheckMark.ClearSelection(cboMediStock.Properties.View);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void SelectionGrid__MediStock(object sender, EventArgs e)
        {
            try
            {
                _MediStockSelecteds = new List<V_HIS_MEDI_STOCK>();
                foreach (V_HIS_MEDI_STOCK rv in (sender as GridCheckMarksSelection).Selection)
                {
                    if (rv != null)
                        _MediStockSelecteds.Add(rv);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
