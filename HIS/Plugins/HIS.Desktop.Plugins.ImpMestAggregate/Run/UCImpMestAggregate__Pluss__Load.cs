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
using Inventec.Common.Logging;
using Inventec.Core;
using MOS.Filter;
using MOS.EFMODEL.DataModels;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using DevExpress.Utils;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Grid;
using AutoMapper;
using HIS.Desktop.Common;
using Inventec.Desktop.Common.Message;
using Inventec.Common.Adapter;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Controls.Session;
using HIS.Desktop.LocalStorage.LocalData;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraBars;
using HIS.Desktop.Utility;
using HIS.Desktop.LocalStorage.ConfigApplication;
using HIS.Desktop.Plugins.ImpMestAggregate.ADO;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.Utilities.Extensions;

namespace HIS.Desktop.Plugins.ImpMestAggregate
{
    public partial class UCImpMestAggregate : HIS.Desktop.Utility.UserControlBase
    {
        /// <summary>
        /// </summary>

        /// <summary>
        /// Load Khoi Tao Du Lieu Danh Sach Phieu Tra
        /// </summary>
        private void LoadDataMobaImpMest()
        {
            try
            {
                WaitingManager.Show();
                HisImpMestView2Filter impMestFilter = new HisImpMestView2Filter();
                impMestFilter.KEY_WORD = txtKeywordProcess.Text.Trim();
                impMestFilter.ORDER_FIELD = "MODIFY_TIME";
                impMestFilter.ORDER_DIRECTION = "DESC";
                //impMestFilter.MEDI_STOCK_ID = (long)cboMediStock.EditValue;...//...
                if (_MediStockSelecteds != null && _MediStockSelecteds.Count > 0)
                {
                    impMestFilter.MEDI_STOCK_IDs = _MediStockSelecteds.Select(p => p.ID).Distinct().ToList();
                }
                impMestFilter.REQ_DEPARTMENT_ID = WorkPlace.WorkPlaceSDO.FirstOrDefault(p => p.RoomId == this.currentModule.RoomId).DepartmentId;// filter theo khoa yeu cau
                impMestFilter.WORKING_ROOM_ID = this.currentModule.RoomId;
                impMestFilter.DATA_DOMAIN_FILTER = true;

                //Là Đơn điều trị trả lại && Là Đơn tủ trực trả lại
                impMestFilter.IMP_MEST_TYPE_IDs = new List<long> { IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__DNTTL, IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__DTTTL };
                //IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__DNTTL ? IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__DNTTL
                if (chkNotSynthetic.Checked && !chkSynthesized.Checked)
                {
                    //HAS_AGGR
                    //Nếu chưa thuộc phiếu nào thì = false
                    //Nếu đã đc tổng hợp = true
                    impMestFilter.HAS_AGGR = false;
                    impMestFilter.IMP_MEST_STT_ID = IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_STT.ID__REQUEST;
                }
                else if (!chkNotSynthetic.Checked && chkSynthesized.Checked)
                    impMestFilter.HAS_AGGR = true;

                if (dtFromTime.EditValue != null && dtFromTime.DateTime != DateTime.MinValue)
                {
                    impMestFilter.CREATE_TIME_FROM = Inventec.Common.TypeConvert.Parse.ToInt64(Convert.ToDateTime(dtFromTime.EditValue).ToString("yyyyMMdd") + "000000");
                }
                if (dtToTime.EditValue != null && dtToTime.DateTime != DateTime.MinValue)
                {
                    impMestFilter.CREATE_TIME_TO = Inventec.Common.TypeConvert.Parse.ToInt64(Convert.ToDateTime(dtToTime.EditValue).ToString("yyyyMMdd") + "235959");
                }
                _ImpMestADOs = new List<ADO.ImpMestADO>();
                gridControlMobaImpMest.DataSource = null;
                CommonParam param = new CommonParam();
                var dataImpMests = new BackendAdapter(param).Get<List<V_HIS_IMP_MEST_2>>("api/HisImpMest/GetView2", ApiConsumers.MosConsumer, impMestFilter, param).Where(o => o.IS_CABINET != 1).ToList();
                if (dataImpMests != null && dataImpMests.Count > 0)
                {
                    foreach (var item in dataImpMests)
                    {
                        ImpMestADO ado = new ImpMestADO();
                        Inventec.Common.Mapper.DataObjectMapper.Map<V_HIS_IMP_MEST_2>(ado, item);
                        if (item.IMP_MEST_STT_ID == IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_STT.ID__REQUEST && (item.AGGR_IMP_MEST_ID == null || item.AGGR_IMP_MEST_ID <= 0))
                        {
                            ado.IsCheck = true;
                        }
                        _ImpMestADOs.Add(ado);
                    }
                }

                gridControlMobaImpMest.DataSource = _ImpMestADOs;
                isCheckAll = false;
                gridColumnCheck.Image = imageListIcon.Images[5];
                this.LoadImpMestDetail();
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>
        /// Khoi Tao Du Lieu Danh Sach Phieu Tong Hop Tra
        /// </summary>
        private void LoadDataAggrImpMest()
        {
            try
            {
                WaitingManager.Show();
                int pageSize = ucPagingAggrImpMest.pagingGrid != null ? ucPagingAggrImpMest.pagingGrid.PageSize : (int)ConfigApplications.NumPageSize;
                PagingAggrImpMest(new CommonParam(0, pageSize));
                CommonParam param = new CommonParam();
                param.Limit = rowCount;
                param.Count = dataTotal;
                ucPagingAggrImpMest.Init(PagingAggrImpMest, param, pageSize, gridControlAggrImpMest);
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void PagingAggrImpMest(object param)
        {
            try
            {
                int start = ((CommonParam)param).Start ?? 0;
                int limit = ((CommonParam)param).Limit ?? 0;
                CommonParam paramCommon = new CommonParam(start, limit);
                gridControlAggrImpMest.DataSource = null;
                HisImpMestView2Filter filter = new HisImpMestView2Filter();
                filter.REQ_DEPARTMENT_ID = WorkPlace.WorkPlaceSDO.FirstOrDefault(p => p.RoomId == this.currentModule.RoomId).DepartmentId;
                filter.ORDER_FIELD = "MODIFY_TIME";
                filter.ORDER_DIRECTION = "DESC";
                filter.IMP_MEST_TYPE_ID = IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__THT;
                var apiResult = new BackendAdapter(paramCommon).GetRO<List<V_HIS_IMP_MEST_2>>(HisRequestUriStore.HIS_IMP_MEST_GETVIEW, ApiConsumers.MosConsumer, filter, paramCommon);
                if (apiResult != null)
                {
                    var data = (List<V_HIS_IMP_MEST_2>)apiResult.Data;
                    if (data != null)
                    {
                        gridControlAggrImpMest.DataSource = data;
                        rowCount = data.Count;
                        dataTotal = apiResult.Param.Count ?? 0;
                    }
                }
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        /// <summary>
        /// Load Du Lieu Chi tiet Phieu Tra
        /// </summary>
        private void LoadDataDetailAggrImpMest()
        {
            try
            {
                WaitingManager.Show();
                int pageSize = (ucPagingAggrImpMestReq.pagingGrid != null) ? (int)ucPagingAggrImpMestReq.pagingGrid.PageSize : (int)ConfigApplications.NumPageSize;

                PagingDetailAggrImpMest(new CommonParam(0, pageSize));
                CommonParam param = new CommonParam();
                param.Limit = rowCountImpM;
                param.Count = dataTotalImpM;
                ucPagingAggrImpMestReq.Init(PagingDetailAggrImpMest, param, pageSize, gridControlImpMestReq);
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
                WaitingManager.Hide();
            }
        }

        private void PagingDetailAggrImpMest(object param)
        {
            try
            {
                int start = ((CommonParam)param).Start ?? 0;
                int limit = ((CommonParam)param).Limit ?? 0;
                CommonParam paramCommon = new CommonParam(start, limit);
                gridControlImpMestReq.DataSource = null;


                HisImpMestView2Filter impMestViewfilter = new HisImpMestView2Filter();
                impMestViewfilter.ORDER_DIRECTION = "DESC";
                impMestViewfilter.ORDER_FIELD = "MODIFY_TIME";
                impMestViewfilter.AGGR_IMP_MEST_ID = this.aggrImpMestId;
                var apiResult = new BackendAdapter(paramCommon).GetRO<List<V_HIS_IMP_MEST_2>>(HisRequestUriStore.HIS_IMP_MEST_GETVIEW, ApiConsumers.MosConsumer, impMestViewfilter, paramCommon);
                if (apiResult != null)
                {
                    var data = (List<V_HIS_IMP_MEST_2>)apiResult.Data;
                    if (data != null)
                    {
                        gridControlImpMestReq.DataSource = data;
                        rowCountImpM = data.Count;
                        dataTotalImpM = apiResult.Param.Count ?? 0;
                    }
                }
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        List<V_HIS_MEDI_STOCK> _MediStockSelecteds = new List<V_HIS_MEDI_STOCK>();
        private void InitComboMediStock()
        {
            try
            {
                CommonParam param = new CommonParam();
                lstMestRoom = new List<V_HIS_MEST_ROOM>();
                long departmentId = WorkPlace.WorkPlaceSDO.FirstOrDefault(x => x.RoomId == this.currentModule.RoomId).DepartmentId;
                if (departmentId > 0)
                {
                    MOS.Filter.HisMestRoomViewFilter hisMestRoomFilter = new MOS.Filter.HisMestRoomViewFilter();
                    lstMestRoom = new BackendAdapter(param).Get<List<V_HIS_MEST_ROOM>>(HisRequestUriStore.HIS_MEST_ROOM_GETVIEW, ApiConsumers.MosConsumer, hisMestRoomFilter, param).Where(p => p.DEPARTMENT_ID == departmentId).GroupBy(p => p.MEDI_STOCK_ID).Select(g => g.FirstOrDefault()).ToList();
                    if (lstMestRoom != null && lstMestRoom.Count > 0)
                    {
                        List<long> mediStockIds = lstMestRoom.Select(p => p.MEDI_STOCK_ID).ToList();
                        _MediStockSelecteds = BackendDataWorker.Get<V_HIS_MEDI_STOCK>().Where(p => mediStockIds.Contains(p.ID)
                            && p.IS_CABINET != 1
                            && p.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE
                            && p.IS_BLOOD != 1
                            ).ToList();
                    }

                    cboMediStock.Properties.DataSource = _MediStockSelecteds;
                    cboMediStock.Properties.DisplayMember = "MEDI_STOCK_NAME";
                    cboMediStock.Properties.ValueMember = "ID";
                    DevExpress.XtraGrid.Columns.GridColumn col2 = cboMediStock.Properties.View.Columns.AddField("MEDI_STOCK_NAME");
                    col2.VisibleIndex = 1;
                    col2.Width = 200;
                    col2.Caption = "Tất cả";
                    cboMediStock.Properties.PopupFormWidth = 200;
                    cboMediStock.Properties.View.OptionsView.ShowColumnHeaders = true;
                    cboMediStock.Properties.View.OptionsSelection.MultiSelect = true;
                    GridCheckMarksSelection gridCheckMark = cboMediStock.Properties.Tag as GridCheckMarksSelection;
                    if (gridCheckMark != null)
                    {
                        gridCheckMark.SelectAll(cboMediStock.Properties.DataSource);
                    }

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
