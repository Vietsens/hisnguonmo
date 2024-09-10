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
using HIS.UC.ListDepositRequest.ADO;
using DevExpress.XtraGrid.Columns;
using DevExpress.Data;
using MOS.EFMODEL.DataModels;
using System.Collections;
using DevExpress.XtraGrid.Views.Base;
using HIS.Desktop.LocalStorage.ConfigApplication;
using Inventec.Core;
using HIS.Desktop.Controls.Session;
using Inventec.Common.Logging;
using Inventec.Desktop.Common.Message;
using Inventec.Common.Adapter;
using HIS.Desktop.ApiConsumer;
using MOS.Filter;
using DevExpress.XtraGrid.Views.Grid;
using System.Resources;
using Inventec.Desktop.Common.LanguageManager;
using HIS.Desktop.LocalStorage.BackendData;
using DevExpress.XtraBars;
using HIS.Desktop.ADO;

namespace HIS.UC.ListDepositRequest
{
    internal partial class UC_ListDepositRequest : UserControl
    {
        ListDepositRequestInitADO initADO = null;

        Grid_CustomUnboundColumnData gridView_CustomUnboundColumnData = null;
        Grid_RowCellClick gridView_RowCellClick = null;
        Grid_KeyUp gridView_KeyUp = null;
        EventHandle btnDelete_Click = null;
        EventHandle btnPrint_Click = null;
        EventHandle btnQR_Click = null;
        Grid_CustomRowCellEdit gridView_CustomRowCellEdit = null;
        Grid_RowCellStyle gridView_RowCellStyle = null;
        private int _rowCount = 0;
        private int _dataTotal = 0;
        int pageSize = 0;
        int startPage = 0;
        bool isShowSearchPanel;
        bool isShowPagingPanel;
        bool visible;

        public UC_ListDepositRequest(ListDepositRequestInitADO ado)
        {
            InitializeComponent();
            try
            {
                this.initADO = ado;
                this.gridView_CustomUnboundColumnData = ado.ListDepositReqGrid_CustomUnboundColumnData;
                this.gridView_RowCellClick = ado.ListDepositReqGrid_RowCellClick;
                this.gridView_KeyUp = ado.ListDepositReqGrid_KeyUp;
                this.btnDelete_Click = ado._btnDelete_Click;
                this.btnPrint_Click = ado._btnPrint_Click;
                this.btnQR_Click = ado._btnQR_Click;
                this.gridView_CustomRowCellEdit = ado.ListDepositReqGrid_CustomRowCellEdit;
                this.gridView_RowCellStyle = ado.ListDepositReqGrid_RowCellStyle;
                this.gridViewDepositRequest.GridControl.MenuManager = ado.barManager;
                if (ado.IsShowSearchPanel.HasValue)
                {
                    this.isShowSearchPanel = ado.IsShowSearchPanel.Value;
                }
                if (ado.IsShowPagingPanel.HasValue)
                {
                    this.isShowPagingPanel = ado.IsShowPagingPanel.Value;
                }
                if (ado.visibleColumn.HasValue)
                {
                    this.visible = ado.visibleColumn.Value;
                }



            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void UC_ListDepositRequest_Load(object sender, EventArgs e)
        {
            try
            {
                SetCaptionByLanguageKey();
                if (this.initADO != null)
                {
                    ProcessColumn();
                    SetVisibleSearchPanel();
                    SetVisibleColumn();
                    //SetVisiblePagingPanel();
                    FilldataToGrid();
                    //UpdateDataSource();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetCaptionByLanguageKey()
        {
            try
            {
                ////Khoi tao doi tuong resource
                Resources.ResourceLanguageManager.LanguageResource = new ResourceManager("HIS.UC.ListDepositRequest.Resources.Lang", typeof(HIS.UC.ListDepositRequest.UC_ListDepositRequest).Assembly);

                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.layoutControl1.Text = Inventec.Common.Resource.Get.Value("UC_ListDepositRequest.layoutControl1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.STT.Caption = Inventec.Common.Resource.Get.Value("UC_ListDepositRequest.STT.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.colCode.Caption = Inventec.Common.Resource.Get.Value("UC_ListDepositRequest.colCode.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.colDelete.Caption = Inventec.Common.Resource.Get.Value("UC_ListDepositRequest.colDelete.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.colPrint.Caption = Inventec.Common.Resource.Get.Value("UC_ListDepositRequest.colPrint.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }


        internal ApiResultObject<List<V_HIS_DEPOSIT_REQ>> GetDatas(object param)
        {
            var result = new ApiResultObject<List<V_HIS_DEPOSIT_REQ>>();          
            try
            {
                startPage = ((CommonParam)param).Start ?? 0;
                var limit = ((CommonParam)param).Limit ?? 0;
                CommonParam paramCommon = new CommonParam(startPage, limit);
                HisDepositReqViewFilter filter = new HisDepositReqViewFilter();
                filter.TREATMENT_ID = this.initADO.ListDepositReq.FirstOrDefault().TREATMENT_ID;
                //result = ApiResultObject<this.initADO.ListDepositReq>();
                result = new BackendAdapter(paramCommon).GetRO<List<V_HIS_DEPOSIT_REQ>>
                    (HisRequestUriStore.HIS_DEPOSIT_REQ_GETVIEW, ApiConsumers.MosConsumer, filter, paramCommon);
            }
            catch (Exception ex)
            {
                
                result = null;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }


        private void FilldataToGrid()
        {
            try
            {
                gridViewDepositRequest.BeginUpdate();
                gridViewDepositRequest.GridControl.DataSource = this.initADO.ListDepositReq;
                gridViewDepositRequest.EndUpdate();
                gridViewDepositRequest.FocusedRowHandle = 0;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        //private void LoadData(object param)
        //{
        //    try
        //    {               
        //        gridControlDepositRequest.DataSource = null;
        //        var result = GetDatas(param);
        //        if (result == null) return;
        //        this.initADO.ListDepositReq = result.Data;
        //        _rowCount = this.initADO.ListDepositReq == null ? 0 : this.initADO.ListDepositReq.Count;
        //        _dataTotal = (result.Param == null ? 0 : result.Param.Count ?? 0);
        //        gridControlDepositRequest.BeginUpdate();
        //        gridControlDepositRequest.DataSource = this.initADO.ListDepositReq;
        //        gridControlDepositRequest.EndUpdate();
        //        gridViewDepositRequest.Focus();              
        //    }
        //    catch (Exception ex)
        //    {
        //        Inventec.Common.Logging.LogSystem.Error(ex);
        //    }
        //}

        //private void SetData()
        //{
        //    try
        //    {
                
        //        if (ucPaging1.pagingGrid != null)
        //        {
        //            pageSize = ucPaging1.pagingGrid.PageSize;
        //        }
        //        else
        //        {
        //            pageSize = (int)ConfigApplications.NumPageSize;
        //        }
        //        LoadData(new CommonParam(0, (int)pageSize));
        //        var param = new CommonParam
        //        {
        //            Limit = _rowCount,
        //            Count = _dataTotal
        //        };
        //        ucPaging1.Init(LoadData, param);
        //    }
        //    catch (Exception ex)
        //    {
        //        Inventec.Common.Logging.LogSystem.Error(ex);
        //    }
        //}
     
        //private void UpdateDataSource()
        //{
        //    try
        //    {
        //        SetData();               
        //    }
        //    catch (Exception ex)
        //    {
        //        Inventec.Common.Logging.LogSystem.Error(ex);
        //    }
        //}

        private void ProcessColumn()
        {
            try
            {
                if (true)
                {
                    
                }


                if (this.initADO.ListDepositReqColumn != null && this.initADO.ListDepositReqColumn.Count > 0)
                {
                    foreach (var item in this.initADO.ListDepositReqColumn)
                    {
                        GridColumn col = gridViewDepositRequest.Columns.AddField(item.FieldName);
                        col.Visible = item.Visible;
                        col.VisibleIndex = item.VisibleIndex;
                        col.Width = item.ColumnWidth;
                        col.FieldName = item.FieldName;
                        col.OptionsColumn.AllowEdit = item.AllowEdit;
                        col.Caption = item.Caption;
                        col.ColumnEdit = item.ColumnEdit;
                        if (item.UnboundColumnType != null && item.UnboundColumnType != UnboundColumnType.Bound)
                            col.UnboundType = item.UnboundColumnType;
                        if (item.Format != null)
                        {
                            col.DisplayFormat.FormatString = item.Format.FormatString;
                            col.DisplayFormat.FormatType = item.Format.FormatType;
                        }

                        col.OptionsColumn.ShowCaption = item.ShowCaption;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetVisibleSearchPanel()
        {
            try
            {
                if (this.isShowSearchPanel)
                {
                    layoutKeyword.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                }
                else
                {
                    layoutKeyword.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }


        private void SetVisibleColumn()
        {
            try
            {
                colDelete.Visible = visible;
                colPrint.Visible = visible;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        

        //private void SetVisiblePagingPanel()
        //{
        //    try
        //    {
        //        if (this.isShowPagingPanel)
        //        {
        //            layoutPaging.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
        //            //layoutPaging.Visible = false;
        //        }
        //        else
        //        {
        //            layoutPaging.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Inventec.Common.Logging.LogSystem.Error(ex);
        //    }
        //}

        private void gridViewDepositRequest_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound && e.ListSourceRowIndex >= 0)
                {
                    var data = (V_HIS_DEPOSIT_REQ)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    if (data != null && this.gridView_CustomUnboundColumnData != null)
                    {
                        this.gridView_CustomUnboundColumnData(data, e);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtKeyword_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                string strValue = (sender as DevExpress.XtraEditors.TextEdit).Text;
                SearchClick(strValue);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SearchClick(string strValue)
        {
            try
            {
                if (this.initADO != null && this.initADO.ListDepositReq != null)
                {
                    List<V_HIS_DEPOSIT_REQ> listData = new List<V_HIS_DEPOSIT_REQ>();
                    gridControlDepositRequest.DataSource = null;
                    if (!String.IsNullOrEmpty(strValue.Trim()))
                    {
                        string key = strValue.Trim().ToLower();

                        listData = this.initADO.ListDepositReq.Where(o => o.DEPOSIT_REQ_CODE.ToLower().Contains(key) || o.DESCRIPTION.ToLower().Contains(key) || o.DEPARTMENT_CODE.ToLower().Contains(key) || o.DEPARTMENT_NAME.ToLower().Contains(key) || o.AMOUNT.ToString().ToLower().Contains(key)).ToList();
                    }
                    else
                    {
                        listData = this.initADO.ListDepositReq;
                    }
                    gridControlDepositRequest.BeginUpdate();
                    gridControlDepositRequest.DataSource = listData;
                    gridControlDepositRequest.EndUpdate();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        internal void Reload(List<V_HIS_DEPOSIT_REQ> data)
        {
            try
            {
                if (this.initADO != null)
                {
                    this.initADO.ListDepositReq = data;
                    FilldataToGrid();
                    //UpdateDataSource();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public V_HIS_DEPOSIT_REQ GetSelectRow()
        {
            V_HIS_DEPOSIT_REQ result = null;
            try
            {
                var data = (V_HIS_DEPOSIT_REQ)gridViewDepositRequest.GetFocusedRow();
                if (data != null)
                {
                    result = (V_HIS_DEPOSIT_REQ)data;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                return null;
            }
            return result;
        }

        private void gridViewDepositRequest_RowClick(object sender, DevExpress.XtraGrid.Views.Grid.RowClickEventArgs e)
        {

        }

        private void gridViewDepositRequest_RowCellClick(object sender, DevExpress.XtraGrid.Views.Grid.RowCellClickEventArgs e)
        {
            try
            {
                var data = (V_HIS_DEPOSIT_REQ)gridViewDepositRequest.GetFocusedRow();

                if (this.gridView_RowCellClick != null)
                {
                    this.gridView_RowCellClick(data);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }

        private void gridViewDepositRequest_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    var data = (V_HIS_DEPOSIT_REQ)gridViewDepositRequest.GetFocusedRow();

                    if (this.gridView_RowCellClick != null)
                    {
                        this.gridView_RowCellClick(data);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnDeleteE_Click(object sender, EventArgs e)
        {
            try
            {
                
                    var data = (V_HIS_DEPOSIT_REQ)gridViewDepositRequest.GetFocusedRow();

                    if (btnDelete_Click != null)
                    {
                        this.btnDelete_Click(data);
                    }              
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnPrintE_Click(object sender, EventArgs e)
        {
            try
            {

                var data = (V_HIS_DEPOSIT_REQ)gridViewDepositRequest.GetFocusedRow();

                if (data != null)
                {
                    this.btnPrint_Click(data);
                }
                
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        List<HIS_CONFIG> listConfig = BackendDataWorker.Get<HIS_CONFIG>().Where(s => s.KEY.StartsWith("HIS.Desktop.Plugins.PaymentQrCode") && !string.IsNullOrEmpty(s.VALUE)).ToList();
        private void gridViewDepositRequest_CustomRowCellEdit(object sender, DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs e)
        {
            try
            {       
                GridView View = sender as GridView;
                if (e.RowHandle >= 0)
                {
                var creator = gridViewDepositRequest.GetRowCellValue(e.RowHandle, "CREATOR");
                var loginName = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
                    if (e.Column.FieldName == "DELETE")
                    {
                        if (loginName.Equals(creator))
                        {
                            e.RepositoryItem = btnDeleteE;
                        }
                        else
                        {
                        e.RepositoryItem = btnDeleteD;
                        }
                    }
                    if (e.Column.FieldName == "PRINT")
                    {
                        
                        e.RepositoryItem = btnPrintE;
                        
                    }
                    if (e.Column.FieldName == "QR")
                    {
                        
                        if(this.listConfig.Count > 0)
                        {
                            e.RepositoryItem = btnQR_E;
                        }
                        else
                        {
                            e.RepositoryItem = btnQR_D;
                        }
                    }

                }
                }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewDepositRequest_RowCellStyle(object sender, RowCellStyleEventArgs e)
        {
            try
            {
                var data = (V_HIS_DEPOSIT_REQ)gridViewDepositRequest.GetRow(e.RowHandle);
                    //trvService.GetDataRecordByNode(e.Node);
                if (data != null && data is V_HIS_DEPOSIT_REQ)
                {
                    if (gridView_RowCellStyle != null)
                    {
                        //e.Appearance.ForeColor=
                        gridView_RowCellStyle(data, e);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnQR_E_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                var data = (V_HIS_DEPOSIT_REQ)gridViewDepositRequest.GetFocusedRow();

                if (data != null)
                {
                    this.btnQR_Click(data);
                }
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
