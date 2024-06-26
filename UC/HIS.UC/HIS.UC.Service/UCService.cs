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
using MOS.EFMODEL.DataModels;
using HIS.UC.Service.ADO;
using DevExpress.Data;
using DevExpress.XtraGrid.Columns;
using System.Collections;
using DevExpress.XtraGrid.Views.Base;
using HIS.Desktop.ADO;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraEditors;
using HIS.UC.Service.Popup;
using DevExpress.XtraBars;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using Inventec.Common.Controls.EditorLoader;
using HIS.Desktop.LocalStorage.BackendData;
using Inventec.Core;
using MOS.Filter;
using Inventec.Common.Adapter;
using HIS.Desktop.ApiConsumer;

namespace HIS.UC.Service
{
    public partial class UCService : UserControl
    {
        ServiceInitADO initADO = null;

        Grid_CustomUnboundColumnData gridView_CustomUnboundColumnData = null;
        btn_Radio_Enable_Click1 btn_Radio_Enable_Click1 = null;
        btn_Radio_Enable_Click2 btn_Radio_Enable_Click2 = null;
        gridViewService_MouseDownMest gridViewService_MouseDownMest = null;
        Check__Enable_CheckedChanged Check__Enable_CheckedChanged = null;

        GridView_MouseRightClick gridView_MouseRightClick = null;
        HIS.UC.Service.ServiceADO currentAdo;
        DevExpress.XtraBars.BarManager barManager1;
        PopupMenuProcessor popupMenuProcessor;
        List<MOS.EFMODEL.DataModels.HIS_MEDICINE_TYPE_ACIN> currentMedicineTypeAcins;
        List<MOS.EFMODEL.DataModels.HIS_ACTIVE_INGREDIENT> currentActiveIngredients;
        gridView_CellValueChanged CellValueChanged = null;
        bool isShowSearchPanel;

        public UCService(ServiceInitADO ado)
        {
            InitializeComponent();
            try
            {
                this.initADO = ado;
                this.gridView_CustomUnboundColumnData = ado.ServiceGrid_CustomUnboundColumnData;
                gridViewService_MouseDownMest = ado.gridViewService_MouseDownMest;
                btn_Radio_Enable_Click1 = ado.btn_Radio_Enable_Click1;
                btn_Radio_Enable_Click2 = ado.btn_Radio_Enable_Click2;
                this.gridView_MouseRightClick = ado.gridView_MouseRightClick;
                Check__Enable_CheckedChanged = ado.Check__Enable_CheckedChanged;
                CellValueChanged = ado.gridView_CellValueChanged;
                if (ado.IsShowSearchPanel.HasValue)
                {
                    this.isShowSearchPanel = ado.IsShowSearchPanel.Value;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public object GetDataGridView()
        {
            object result = null;
            try
            {
                result = (List<HIS.UC.Service.ServiceADO>)gridControlService.DataSource;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                return null;
            }
            return result;
        }

        private void UCService_Load(object sender, EventArgs e)
        {
            try
            {
                if (this.initADO != null)
                {
                    repositoryItemTextEditReadOnly.ReadOnly = true;
                    repositoryItemTextEditReadOnly.Enabled = false;
                    ProcessColumn();
                    InitHoatChatData();
                    FillDataIntoRoomCombo();
                    UpdateDataSource();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        void InitHoatChatData()
        {
            CommonParam param = new CommonParam();
            HisMedicineTypeAcinFilter medicineTypeAcinFilter = new HisMedicineTypeAcinFilter();
            //medicineTypeAcinFilter.MEDICINE_TYPE_IDs = mediTypeIds;//TODO
            this.currentMedicineTypeAcins = new BackendAdapter(param)
            .Get<List<MOS.EFMODEL.DataModels.HIS_MEDICINE_TYPE_ACIN>>("api/HisMedicineTypeAcin/Get", ApiConsumers.MosConsumer, medicineTypeAcinFilter, param);

            param = new CommonParam();
            HisActiveIngredientFilter activeIngredientFilter = new HisActiveIngredientFilter();
            //activeIngredientFilter.MEDICINE_TYPE_IDs = mediTypeIds;//TODO
            activeIngredientFilter.IS_ACTIVE = 1;
            this.currentActiveIngredients = new BackendAdapter(param)
            .Get<List<MOS.EFMODEL.DataModels.HIS_ACTIVE_INGREDIENT>>("api/HisActiveIngredient/Get", ApiConsumers.MosConsumer, activeIngredientFilter, param);
        }

        private void UpdateDataSource()
        {
            try
            {
                gridControlService.BeginUpdate();
                gridControlService.DataSource = this.initADO.ListService;
                gridControlService.EndUpdate();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ProcessColumn()
        {
            try
            {
                if (this.initADO.ListServiceColumn != null && this.initADO.ListServiceColumn.Count > 0)
                {
                    foreach (var item in this.initADO.ListServiceColumn)
                    {
                        GridColumn col = gridViewService.Columns.AddField(item.FieldName);
                        col.Visible = item.Visible;
                        col.VisibleIndex = item.VisibleIndex;
                        col.Width = item.ColumnWidth;
                        col.FieldName = item.FieldName;
                        col.OptionsColumn.AllowEdit = item.AllowEdit;
                        col.Caption = item.Caption;
                        col.ToolTip = item.Tooltip;
                        if (item.image != null)
                        {
                            col.Image = item.image;
                            col.ImageAlignment = StringAlignment.Center;
                        }
                        if (item.UnboundColumnType != null && item.UnboundColumnType != UnboundColumnType.Bound)
                            col.UnboundType = item.UnboundColumnType;
                        if (item.Format != null)
                        {
                            col.DisplayFormat.FormatString = item.Format.FormatString;
                            col.DisplayFormat.FormatType = item.Format.FormatType;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewService_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound && e.ListSourceRowIndex >= 0)
                {
                    var data = (ServiceADO)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
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

        internal void Reload(List<ServiceADO> data)
        {
            try
            {
                if (this.initADO != null)
                {
                    this.initADO.ListService = data;
                    UpdateDataSource();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewService_CustomRowCellEdit(object sender, DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs e)
        {
            try
            {
                GridView View = sender as GridView;
                if (e.RowHandle >= 0)
                {
                    var data = (ServiceADO)gridViewService.GetRow(e.RowHandle);
                    if (data != null)
                    {
                        if (e.Column.FieldName == "checkService")
                        {
                            if (data.isKeyChooseService)
                            {
                                e.RepositoryItem = repositoryItemCheck__Disable;
                            }
                            else
                            {
                                e.RepositoryItem = repositoryItemCheck__Enable;
                            }
                        }
                        if (e.Column.FieldName == "checkServiceNotUse")
                        {
                            if (data.isKeyChooseService)
                            {
                                e.RepositoryItem = repositoryItemCheck__Disable;
                            }
                            else
                            {
                                e.RepositoryItem = repositoryItemCheck2__Enable;
                            }
                        }
                        if (e.Column.FieldName == "checkWarning")
                        {
                            if (data.isKeyChooseService)
                            {
                                e.RepositoryItem = repositoryItemCheck_Warning_Dis;
                            }
                            else
                            {
                                e.RepositoryItem = repositoryItemCheck_Warning_En;
                            }
                        }
                        if (e.Column.FieldName == "radioService")
                        {
                            if (data.isKeyChooseService)
                            {
                                e.RepositoryItem = repositoryItemRadio_Enable;
                            }
                            else
                            {
                                e.RepositoryItem = repositoryItemRadio_Disable;
                            }

                        }
                        else if (e.Column.FieldName == "PRICE")
                        {
                            e.RepositoryItem = SpinEditPriceEnable;

                        }
                        else if (e.Column.FieldName == "VAT_RATIO")
                        {
                            e.RepositoryItem = SpinEditVatEnable;

                        }
                        else if (e.Column.FieldName == "ROOM_ID")
                        {
                            e.RepositoryItem = Res_CboRoom;
                        }
                        else if (e.Column.FieldName == "AMOUNT")
                        {
                            e.RepositoryItem = SpinEditAmountEnable;
                        }
                        if (e.Column.FieldName == "MIN_DURATION_STR")
                        {
                            if (data.isKeyChooseService)
                            {
                                e.RepositoryItem = repositoryItemSpinEdit__Min_Duration_Disable;
                            }
                            else
                            {
                                e.RepositoryItem = repositoryItemSpinEdit__Min_Duration_Enable;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void repositoryItemRadio_Enable_Click(object sender, EventArgs e)
        {
            try
            {
                //var row = (V_HIS_SERVICE)gridViewService.GetFocusedRow();
                var row1 = (ServiceADO)gridViewService.GetFocusedRow();
                foreach (var item in this.initADO.ListService)
                {
                    if (row1.ID > 0 || row1.ACTIVE_INGREDIENT_ID > 0)
                    {
                        if (item.ID == row1.ID && item.ACTIVE_INGREDIENT_ID == row1.ACTIVE_INGREDIENT_ID)
                        {
                            item.radioService = true;
                            item.AMOUNT = 1;
                            var servicePatyKsk = BackendDataWorker.Get<V_HIS_SERVICE_PATY>().Where(o => o.SERVICE_ID == item.ID && o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE && o.PATIENT_TYPE_CODE == HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>("MOS.HIS_PATIENT_TYPE.PATIENT_TYPE_CODE.KSK")).ToList();
                            if (servicePatyKsk != null && servicePatyKsk.Count > 0)
                            {
                                servicePatyKsk = servicePatyKsk.OrderByDescending(o => o.PRIORITY).ThenByDescending(o => o.ID).ToList();
                                item.PRICE_KSK = servicePatyKsk.First().PRICE;
                                item.VAT_RATIO_KSK = servicePatyKsk.First().VAT_RATIO;
                            }
                        }
                        else
                        {
                            item.radioService = false;
                            item.AMOUNT = 0;
                            item.PRICE_KSK = null;
                            item.VAT_RATIO_KSK = null;
                        }
                    }
                }

                gridControlService.RefreshDataSource();

                if (row1 != null && this.btn_Radio_Enable_Click1 != null)
                {
                    this.btn_Radio_Enable_Click1(row1);
                }
                if (row1 != null && this.btn_Radio_Enable_Click2 != null)
                {
                    this.btn_Radio_Enable_Click2(row1);
                }
                gridViewService.LayoutChanged();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewService_MouseDown(object sender, MouseEventArgs e)
        {
            try
            {
                var row = (ServiceADO)gridViewService.GetFocusedRow();
                if (row != null && this.gridViewService_MouseDownMest != null)
                {
                    this.gridViewService_MouseDownMest(sender, e);
                }
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridView_PopupMenuShowing(object sender, PopupMenuShowingEventArgs e)
        {
            try
            {
                currentAdo = (HIS.UC.Service.ServiceADO)gridViewService.GetFocusedRow();

                GridHitInfo hi = e.HitInfo;
                if (hi.InRowCell && this.gridView_MouseRightClick != null)
                {
                    if (this.barManager1 == null && this.gridView_MouseRightClick != null)
                        this.barManager1 = new DevExpress.XtraBars.BarManager();
                    this.barManager1.Form = this;
                    popupMenuProcessor = new PopupMenuProcessor(this.barManager1, GridView_MouseRightClick);
                    this.popupMenuProcessor.InitMenu();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void GridView_MouseRightClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                if ((e.Item is BarButtonItem) && this.currentAdo != null)
                {
                    var type = (PopupMenuProcessor.ItemType)e.Item.Tag;
                    switch (type)
                    {
                        case PopupMenuProcessor.ItemType.Copy:
                            {
                                this.gridView_MouseRightClick(this.currentAdo, e);
                                break;
                            }
                        case PopupMenuProcessor.ItemType.Paste:
                            {
                                this.gridView_MouseRightClick(this.currentAdo, e);
                                break;
                            }
                        default:
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void repositoryItemCheck__Enable_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                var row = (ServiceADO)gridViewService.GetFocusedRow();

                foreach (var item in this.initADO.ListService)
                {
                    if (row.ID > 0 || row.ACTIVE_INGREDIENT_ID > 0)
                    {
                        if (item.ID == row.ID && item.ACTIVE_INGREDIENT_ID == row.ACTIVE_INGREDIENT_ID)
                        {
                            if (row.checkService == false)
                            {
                                item.checkService = true;
                                item.checkServiceNotUse = false;
                                item.checkWarning = false;
                                //if (item.AMOUNT < 1)
                                item.AMOUNT = 1;

                                var lstRoom = BackendDataWorker.Get<V_HIS_SERVICE_ROOM>().Where(o => o.SERVICE_ID == item.ID && o.ROOM_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_ROOM_TYPE.ID__XL).ToList();
                                if (lstRoom != null && lstRoom.Count == 1)
                                {
                                    item.ROOM_ID = lstRoom.First().ROOM_ID;
                                }
                                var servicePatyKsk = BackendDataWorker.Get<V_HIS_SERVICE_PATY>().Where(o => o.SERVICE_ID == item.ID && o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE && o.PATIENT_TYPE_CODE == HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>("MOS.HIS_PATIENT_TYPE.PATIENT_TYPE_CODE.KSK")).ToList();
                                if (servicePatyKsk != null && servicePatyKsk.Count > 0)
                                {
                                    servicePatyKsk = servicePatyKsk.OrderByDescending(o => o.PRIORITY).ThenByDescending(o => o.ID).ToList();
                                    item.PRICE_KSK = servicePatyKsk.First().PRICE;
                                    item.VAT_RATIO_KSK = servicePatyKsk.First().VAT_RATIO;
                                }
                            }
                            else
                            {
                                item.checkService = false;
                                item.checkWarning = false;
                                item.AMOUNT = 0;
                                item.ROOM_ID = 0;
                                item.PRICE = 0;
                                item.VAT_RATIO = 0;
                                item.VAT_RATIO_KSK = null;
                                item.PRICE_KSK = null;
                            }
                        }

                    }

                }
                gridControlService.RefreshDataSource();
                if (row != null && this.Check__Enable_CheckedChanged != null)
                {
                    this.Check__Enable_CheckedChanged(row);
                }

                gridViewService.LayoutChanged();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void repositoryItemCheck2__Enable_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                var row = (ServiceADO)gridViewService.GetFocusedRow();

                foreach (var item in this.initADO.ListService)
                {
                    if (row.ID > 0)
                    {
                        if (item.ID == row.ID)
                        {
                            if (row.checkServiceNotUse == false)
                            {
                                item.checkServiceNotUse = true;
                                item.checkService = false;
                                item.checkWarning = false;
                            }
                            else
                            {
                                item.checkServiceNotUse = false;
                                item.checkService = false;
                                item.checkWarning = false;
                            }
                        }

                    }

                }

                gridControlService.RefreshDataSource();
                if (row != null && this.Check__Enable_CheckedChanged != null)
                {
                    this.Check__Enable_CheckedChanged(row);
                }


                gridViewService.LayoutChanged();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void FillDataIntoRoomCombo()
        {
            try
            {
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("ROOM_CODE", "", 100, 1));
                columnInfos.Add(new ColumnInfo("ROOM_NAME", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("ROOM_NAME", "ROOM_ID", columnInfos, false, 250);
                ControlEditorLoader.Load(Res_CboRoom, BackendDataWorker.Get<V_HIS_SERVICE_ROOM>(), controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void FillDataIntoRoomCombo(ServiceADO data, DevExpress.XtraEditors.GridLookUpEdit roomCombo)
        {
            try
            {
                var serviceRoomInBranchs = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<V_HIS_SERVICE_ROOM>();
                List<V_HIS_SERVICE_ROOM> serRoom = new List<V_HIS_SERVICE_ROOM>();

                if (serviceRoomInBranchs != null && serviceRoomInBranchs.Count > 0)
                {
                    serRoom = serviceRoomInBranchs.Where(o => data != null && o.SERVICE_ID == data.ID && o.ROOM_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_ROOM_TYPE.ID__XL).ToList();
                }

                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("ROOM_CODE", "", 100, 1));
                columnInfos.Add(new ColumnInfo("ROOM_NAME", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("ROOM_NAME", "ROOM_ID", columnInfos, false, 250);
                ControlEditorLoader.Load(roomCombo, serRoom, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewService_ShownEditor(object sender, EventArgs e)
        {
            try
            {
                DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                ServiceADO data = view.GetFocusedRow() as ServiceADO;
                if (view.FocusedColumn.FieldName == "ROOM_ID" && view.ActiveEditor is GridLookUpEdit)
                {
                    GridLookUpEdit editor = view.ActiveEditor as GridLookUpEdit;
                    FillDataIntoRoomCombo(data, editor);
                    editor.EditValue = data != null ? data.ROOM_ID : 0;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewService_CustomColumnDisplayText(object sender, CustomColumnDisplayTextEventArgs e)
        {

            try
            {
                if (e.Column.FieldName == "PRICE" || e.Column.FieldName == "VAT_RATIO")
                {
                    var val = String.Format("{0}", e.Value);
                    if (val == "0")
                        e.DisplayText = "";
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public object GetGridControl()
        {
            object Result = null;
            try
            {
                Result = this.gridControlService;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                return null;
            }
            return Result;
        }

        private void gridViewService_CellValueChanged(object sender, CellValueChangedEventArgs e)
        {

            try
            {

                if (CellValueChanged != null)
                {
                    CellValueChanged(sender, e);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void repositoryItemCheck_Warning_En_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                var row = (ServiceADO)gridViewService.GetFocusedRow();

                foreach (var item in this.initADO.ListService)
                {
                    if (row.ID > 0)
                    {
                        if (item.ID == row.ID)
                        {
                            if (row.checkWarning == false)
                            {
                                item.checkWarning = true;
                                item.checkServiceNotUse = false;
                                item.checkService = false;
                            }
                            else
                            {
                                item.checkWarning = false;
                                item.checkService = false;
                                item.checkServiceNotUse = false;
                            }
                        }

                    }

                }

                //if (row != null && this.Check__Enable_CheckedChanged != null)
                //{
                //    this.Check__Enable_CheckedChanged(row);
                //}


                gridControlService.RefreshDataSource();
                gridViewService.LayoutChanged();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

    }
}
