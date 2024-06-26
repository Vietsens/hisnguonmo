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
using Inventec.Common.Logging;
using MOS.Filter;
using HIS.Desktop.LocalStorage.ConfigApplication;
using Inventec.Common.Adapter;
using HIS.Desktop.ApiConsumer;
using DevExpress.Utils;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using DevExpress.XtraGrid.Columns;
using DevExpress.Data;
using MOS.SDO;
using System.Collections;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraBars;
using Inventec.Desktop.Common.Message;
using Inventec.UC.TreatmentRecord.Data;
using Inventec.UC.TreatmentRecord.Delegate;
using MOS.EFMODEL.DataModels;

namespace Inventec.UC.TreatmentRecord
{
    public partial class UCTreatmentList : UserControl
    {
        #region Declared
        int rowCount = 0;
        int dataTotal = 0;
        ToolTipControlInfo lastInfo = null;
        int lastRowHandle = -1;
        GridColumn lastColumn = null;
        HisTreatmentViewByPatientTypeIdFilter dataFilter = null;
        GridViewOnClickGetRow GridViewOnClickGetRow_Click;
        PopupMenuShowingForGrid popupMenuShowingForGrid;
        CheckSelection CheckSelection;
        PopupMenu menu;
        InitData data;
        #endregion

        #region Contructor
        public UCTreatmentList()
        {
            InitializeComponent();
        }
        public UCTreatmentList(Data.InitData data)
        {
            InitializeComponent();
            this.data = data;
            this.popupMenuShowingForGrid = data.PopupMenuShowing_Click;
            this.GridViewOnClickGetRow_Click = data.GridViewOnClickGetRow_Click;
            this.CheckSelection = data.CheckSelection;
        }

        private void UCTreatmentList_Load(object sender, EventArgs e)
        {
            try
            {
                LoadDataFillGridTreatement();
                this.gridControlTreatmentList.ToolTipController = this.toolTipController1;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        #endregion

        #region Load data to grid
        private void LoadDataFillGridTreatement()
        {
            try
            {
                ucPagingTreatment(new CommonParam(0, Convert.ToInt32(ConfigApplications.NumPageSize)));

                CommonParam param = new CommonParam();
                param.Limit = rowCount;
                param.Count = dataTotal;
                ucPaging1.Init(ucPagingTreatment, param);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        internal void ucPagingTreatment(object param)
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Warn("bat dau goi api");
                WaitingManager.Show();
                int start = ((CommonParam)param).Start ?? 0;
                int limit = ((CommonParam)param).Limit ?? 0;
                CommonParam paramCommon = new CommonParam(start, limit);
                HisTreatmentViewByPatientTypeIdFilter filter = new HisTreatmentViewByPatientTypeIdFilter();
                SetTreatmentFilter(ref filter);
                gridControlTreatmentList.DataSource = null;
                var apiResult = new BackendAdapter(paramCommon).GetRO<List<MOS.SDO.HisTreatmentHeinSDO>>(HisRequestUriStore.HIS_TREATMENT_GETVIEW_PATIENT_TYPE_ID, ApiConsumers.MosConsumer, filter, paramCommon);
                if (apiResult != null)
                {
                    var dataGrid = (List<MOS.SDO.HisTreatmentHeinSDO>)apiResult.Data;
                    if (dataGrid != null)
                    {
                        CommonParam paramApproval = new CommonParam();
                        MOS.Filter.HisHeinApprovalFilter hisHeinApprovalFilter = new HisHeinApprovalFilter();
                        hisHeinApprovalFilter.TREATMENT_IDs = dataGrid.Select(s => s.ID).Distinct().ToList();
                        List<MOS.EFMODEL.DataModels.HIS_HEIN_APPROVAL> lsthisHeinApproval = new BackendAdapter(paramApproval).Get<List<HIS_HEIN_APPROVAL>>(HisRequestUriStore.HIS_HEIN_APPROVAL_GET, ApiConsumers.MosConsumer, hisHeinApprovalFilter, paramApproval);
                        if (this.data.cboTypeStatus == Store.GlobalStore.ComboTypeStatus.DA_DUYET)
                        {
                            List<MOS.SDO.HisTreatmentHeinSDO> lstData_TreatmentApproved = new List<MOS.SDO.HisTreatmentHeinSDO>();
                            if (lsthisHeinApproval != null && lsthisHeinApproval.Count > 0)
                            {
                                foreach (var item_treatment in dataGrid)
                                {
                                    var treatment_approvalSTT = lsthisHeinApproval.Where(o => o.TREATMENT_ID == item_treatment.ID).ToList();
                                    if (treatment_approvalSTT != null && treatment_approvalSTT.Count > 0)
                                    {
                                        lstData_TreatmentApproved.Add(item_treatment);
                                    }
                                }
                            }
                            gridControlTreatmentList.DataSource = lstData_TreatmentApproved;
                            rowCount = (lstData_TreatmentApproved == null ? 0 : lstData_TreatmentApproved.Count);
                        }
                        else if (this.data.cboTypeStatus == Store.GlobalStore.ComboTypeStatus.CHUA_DUYET)
                        {
                            List<MOS.SDO.HisTreatmentHeinSDO> lstData_TreatmentUnApproved = new List<MOS.SDO.HisTreatmentHeinSDO>();
                            foreach (var item_treatment in dataGrid)
                            {
                                if (lsthisHeinApproval != null && lsthisHeinApproval.Count > 0)
                                {
                                    var treatment_approvalSTT = lsthisHeinApproval.Where(o => o.TREATMENT_ID == item_treatment.ID).ToList();
                                    if (treatment_approvalSTT != null && treatment_approvalSTT.Count > 0)
                                    {
                                    }
                                    else
                                    {
                                        lstData_TreatmentUnApproved.Add(item_treatment);
                                    }
                                }
                                else
                                {
                                    lstData_TreatmentUnApproved.Add(item_treatment);
                                }
                            }
                            gridControlTreatmentList.DataSource = lstData_TreatmentUnApproved;
                            rowCount = (lstData_TreatmentUnApproved == null ? 0 : lstData_TreatmentUnApproved.Count);
                        }
                        else
                        {
                            gridControlTreatmentList.DataSource = dataGrid;
                            rowCount = (dataGrid == null ? 0 : dataGrid.Count);
                        }
                        dataTotal = (apiResult.Param == null ? 0 : apiResult.Param.Count ?? 0);
                        if (rowCount > 0)
                        {
                            gridViewTreatmentList.FocusedRowHandle = 0;
                            var row = (HisTreatmentHeinSDO)gridViewTreatmentList.GetFocusedRow();
                            this.GridViewOnClickGetRow_Click(row);
                        }

                    }
                }
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn("ket thuc goi api");
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                LogSystem.Error(ex);
            }
        }
        private void SetTreatmentFilter(ref HisTreatmentViewByPatientTypeIdFilter filter)
        {
            try
            {
                //TODO
                if (this.data != null)
                {
                    filter = this.data.filter;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        #endregion

        #region Event Grid View
        private void toolTipController1_GetActiveObjectInfo(object sender, DevExpress.Utils.ToolTipControllerGetActiveObjectInfoEventArgs e)
        {
            try
            {
                if (e.Info == null && e.SelectedControl == gridControlTreatmentList)
                {
                    DevExpress.XtraGrid.Views.Grid.GridView view = gridControlTreatmentList.FocusedView as DevExpress.XtraGrid.Views.Grid.GridView;
                    GridHitInfo info = view.CalcHitInfo(e.ControlMousePosition);
                    if (info.InRowCell)
                    {
                        if (lastRowHandle != info.RowHandle || lastColumn != info.Column)
                        {
                            lastColumn = info.Column;
                            lastRowHandle = info.RowHandle;

                            string text = "";
                            if (info.Column.FieldName == "IMG_STATUS")
                            {
                                string status_hein = (view.GetRowCellValue(lastRowHandle, "IS_LOCK_HEIN") ?? "").ToString();
                                string status_treatment = (view.GetRowCellValue(lastRowHandle, "IS_PAUSE") ?? "").ToString();
                                bool status_heinApproval = Inventec.Common.TypeConvert.Parse.ToBoolean((view.GetRowCellValue(lastRowHandle, "HasHeinApproval") ?? "false").ToString());

                                if (status_treatment != "")
                                {
                                    if (status_hein != "")
                                    {
                                        text = "Đã khóa bảo hiểm y tế";
                                    }
                                    else if (status_heinApproval == true)
                                    {
                                        text = "Đã duyệt giám định";
                                    }
                                    else
                                    {
                                        text = "Chưa khóa bảo hiểm y tế";
                                    }
                                }
                                else
                                {
                                    text = "Đang điều trị";
                                }
                                //short status_ispause = Inventec.Common.TypeConvert.Parse.ToInt16((view.GetRowCellValue(lastRowHandle, "IS_PAUSE") ?? "-1").ToString());
                                //decimal status_islock = Inventec.Common.TypeConvert.Parse.ToDecimal((view.GetRowCellValue(lastRowHandle, "IS_ACTIVE") ?? "-1").ToString());
                                //short status_islockhein = Inventec.Common.TypeConvert.Parse.ToInt16((view.GetRowCellValue(lastRowHandle, "IS_LOCK_HEIN") ?? "-1").ToString());
                                ////Status
                                ////1- dang dieu tri
                                ////2- da ket thuc
                                ////3- khóa hồ sơ
                                ////4- duyệt bhyt
                                //if (status_islockhein != IMSys.DbConfig.HIS_RS.HIS_TREATMENT.IS_LOCK_HEIN__TRUE)
                                //{
                                //    if (status_islock == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE)
                                //    {
                                //        if (status_ispause != IMSys.DbConfig.HIS_RS.HIS_TREATMENT.IS_PAUSE__TRUE)
                                //        {
                                //            text = "Đang điều trị";
                                //        }
                                //        else
                                //        {
                                //            text = "Kết thúc điều trị";
                                //        }
                                //    }
                                //    else
                                //    {
                                //        text = "Khóa hồ sơ";
                                //    }
                                //}
                                //else
                                //{
                                //    text = "Duyệt bảo hiểm y tế";
                                //}
                            }
                            lastInfo = new ToolTipControlInfo(new DevExpress.XtraGrid.GridToolTipInfo(view, new DevExpress.XtraGrid.Views.Base.CellToolTipInfo(info.RowHandle, info.Column, "Text")), text);
                        }
                        e.Info = lastInfo;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewTreatmentList_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)
                {
                    HisTreatmentHeinSDO treatmentRow = (MOS.SDO.HisTreatmentHeinSDO)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    if (treatmentRow != null)
                    {
                        if (e.Column.FieldName == "STT")
                        {
                            e.Value = e.ListSourceRowIndex + 1 + ((ucPaging1.pagingGrid.CurrentPage - 1) * ucPaging1.pagingGrid.PageSize);
                        }
                        else if (e.Column.FieldName == "IMG_STATUS")
                        {
                            if (this.data.moduleType == Store.GlobalStore.ModuleType.MIE)
                            {
                                try
                                {
                                    //DevExpress.Utils.ImageCollection images = new DevExpress.Utils.ImageCollection();
                                    //Trang thai
                                    //Trang: dang dieu tri (0)
                                    //Xanh la cay: da ket thuc (1)
                                    //Da cam: da duyet MIE (3)
                                    //Xanh da troi: da khoa MIE (2)
                                    if (treatmentRow.IS_PAUSE == IMSys.DbConfig.HIS_RS.HIS_TREATMENT.IS_PAUSE__TRUE)
                                    {
                                        if (treatmentRow.IS_LOCK_HEIN == IMSys.DbConfig.HIS_RS.HIS_TREATMENT.IS_LOCK_HEIN__TRUE)
                                        {
                                            e.Value = imageListMIE.Images[2];
                                        }
                                        else if (treatmentRow.HasHeinApproval == true)
                                        {
                                            e.Value = imageListMIE.Images[3];
                                        }
                                        else
                                        {
                                            e.Value = imageListMIE.Images[1];
                                        }
                                    }
                                    else
                                    {
                                        e.Value = imageListMIE.Images[0];
                                    }
                                }
                                catch (Exception ex)
                                {
                                    Inventec.Common.Logging.LogSystem.Warn("Loi set gia tri cho cot icon trang thai yeu cau dich vu IMG", ex);
                                }
                            }
                            else
                            {
                                try
                                {
                                    DevExpress.Utils.ImageCollection images = new DevExpress.Utils.ImageCollection();
                                    short status_ispause = Inventec.Common.TypeConvert.Parse.ToInt16((treatmentRow.IS_PAUSE ?? -1).ToString());
                                    decimal status_islock = Inventec.Common.TypeConvert.Parse.ToDecimal((treatmentRow.IS_ACTIVE ?? -1).ToString());
                                    short status_islockhein = Inventec.Common.TypeConvert.Parse.ToInt16((treatmentRow.IS_LOCK_HEIN ?? -1).ToString());
                                    //Status
                                    //1- dang dieu tri
                                    //2- da ket thuc
                                    //3- khóa hồ sơ
                                    //4- duyệt bhyt
                                    if (status_islockhein != IMSys.DbConfig.HIS_RS.HIS_TREATMENT.IS_LOCK_HEIN__TRUE)
                                    {
                                        if (status_islock == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE)
                                        {
                                            if (status_ispause != IMSys.DbConfig.HIS_RS.HIS_TREATMENT.IS_PAUSE__TRUE)
                                            {
                                                e.Value = imageListStatus.Images[0];
                                            }
                                            else
                                            {
                                                e.Value = imageListStatus.Images[1];
                                            }
                                        }
                                        else
                                        {
                                            e.Value = imageListStatus.Images[2];
                                        }
                                    }
                                    else
                                    {
                                        e.Value = imageListStatus.Images[3];
                                    }
                                }
                                catch (Exception ex)
                                {
                                    Inventec.Common.Logging.LogSystem.Warn("Loi set gia tri cho cot icon trang thai yeu cau dich vu IMG_STATUS", ex);
                                }
                            }
                        }
                        else if (e.Column.FieldName == "DOB_DISPLAY")
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToDateString(treatmentRow.DOB);
                        }
                        else if (e.Column.FieldName == "IN_COMING_TIME")
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(treatmentRow.IncomingTime);
                        }
                        else if (e.Column.FieldName == "OUT_COMING_TIME")
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(treatmentRow.OutgoingTime ?? 0);
                        }
                    }
                    else
                    {
                        e.Value = null;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewTreatmentList_PopupMenuShowing(object sender, DevExpress.XtraGrid.Views.Grid.PopupMenuShowingEventArgs e)
        {
            try
            {
                GridHitInfo hi = e.HitInfo;
                if (hi.InRowCell)
                {
                    var row = (MOS.SDO.HisTreatmentHeinSDO)((IList)gridControlTreatmentList.DataSource)[hi.RowHandle];
                    if (barManager1 == null)
                    {
                        barManager1 = new BarManager();
                        barManager1.Form = this;
                    }
                    this.popupMenuShowingForGrid(row, barManager1, menu);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewTreatmentList_Click(object sender, EventArgs e)
        {
            try
            {
                //var row = (MOS.SDO.HisTreatmentHeinSDO)gridViewTreatmentList.GetFocusedRow();
                //if (row != null)
                //{
                //    this.GridViewOnClickGetRow_Click(row);
                //}
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        //CheckSelection
        private void GetDataCheckSelection()
        {
            try
            {
                if (gridViewTreatmentList.RowCount > 0)
                {
                    List<HisTreatmentHeinSDO> lstTreatment = new List<HisTreatmentHeinSDO>();
                    for (int i = 0; i < gridViewTreatmentList.SelectedRowsCount; i++)
                    {
                        if (gridViewTreatmentList.GetSelectedRows()[i] >= 0)
                        {
                            lstTreatment.Add((HisTreatmentHeinSDO)gridViewTreatmentList.GetRow(gridViewTreatmentList.GetSelectedRows()[i]));
                        }
                    }
                    this.CheckSelection(lstTreatment);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        #endregion

        private void SetVisibleIndexColumns()
        {
            try
            {
                List<GridColumn> lstCol = new List<GridColumn>();
                lstCol = gridViewTreatmentList.Columns.ToList();
                if (lstCol != null && lstCol.Count > 0)
                {
                    //TODO
                }
                var column = new GridColumn();
                column.VisibleIndex = 1;
                gridViewTreatmentList.Columns.AddVisible("");
                gridViewTreatmentList.BeginDataUpdate();
                gridControlTreatmentList.BackColor = Color.Red;
                foreach (GridColumn item in gridViewTreatmentList.Columns)
                {
                    if (item == gridViewTreatmentList.Columns[""])
                    {

                    }
                }
                gridViewTreatmentList.EndDataUpdate();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewTreatmentList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            GetDataCheckSelection();
        }

        internal void RefeshData(Data.InitData data)
        {
            try
            {
                this.data = data;
                LoadDataFillGridTreatement();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewTreatmentList_RowCellClick(object sender, DevExpress.XtraGrid.Views.Grid.RowCellClickEventArgs e)
        {
            var row = (MOS.SDO.HisTreatmentHeinSDO)gridViewTreatmentList.GetFocusedRow();
            if (row != null)
            {
                this.GridViewOnClickGetRow_Click(row);
            }
        }
    }
}
