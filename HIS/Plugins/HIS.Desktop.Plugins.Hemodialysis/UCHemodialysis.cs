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
using HIS.Desktop.Utility;
using HIS.Desktop.LocalStorage.ConfigApplication;
using Inventec.Core;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using Inventec.Common.Adapter;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Controls.Session;
using Inventec.Desktop.Common.Message;
using DevExpress.Data;
using System.Collections;
using DevExpress.XtraGrid.Views.Base;
using Inventec.Common.Controls.EditorLoader;
using DevExpress.XtraEditors;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.Plugins.Hemodialysis.ADO;
using AutoMapper;
using HIS.Desktop.LocalStorage.LocalData;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using DevExpress.Utils;
using DevExpress.XtraGrid.Columns;
using HIS.Desktop.ADO;
using Inventec.Common.Logging;

namespace HIS.Desktop.Plugins.Hemodialysis
{
    public partial class UCHemodialysis : UserControlBase
    {
        private int patientStart = 0;
        private int patientLimit = 0;
        private int patientRowCount = 0;
        private int patientTotalData = 0;

        private int oldStart = 0;
        private int oldLimit = 0;
        private int oldRowCount = 0;
        private int oldTotalData = 0;

        ToolTipControlInfo lastInfo = null;
        GridColumn lastColumn = null;
        int lastRowHandle = -1;

        private List<ServiceReq8ADO> currentListData = new List<ServiceReq8ADO>();

        private ServiceReq8ADO currentServiceReq = null;
        private HIS_SERVICE_REQ currentPrescription = null;

        public UCHemodialysis(Inventec.Desktop.Common.Modules.Module module)
            : base(module)
        {
            InitializeComponent();
        }

        private void UCHemodialysis_Load(object sender, EventArgs e)
        {
            try
            {
                WaitingManager.Show();
                this.SetDefaultControlValue();
                this.InitComboRepositoryStatus(BackendDataWorker.Get<HIS_SERVICE_REQ_STT>());
                gridControlPatient.ToolTipController = this.toolTipController1;
                FillDataToGridPatient();
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetDefaultControlValue()
        {
            try
            {
                txtKeywork.Text = "";
                dtInstructionDateFrom.DateTime = DateTime.Now;
                dtInstructionDateTo.DateTime = DateTime.Now;
                cboStatus.SelectedIndex = 1;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void FillDataToGridPatient()
        {
            int numPageSize;
            if (ucPagingPatient.pagingGrid != null)
            {
                numPageSize = ucPagingPatient.pagingGrid.PageSize;
            }
            else
            {
                numPageSize = ConfigApplicationWorker.Get<int>("CONFIG_KEY__NUM_PAGESIZE");
            }

            LoadPagingPatient(new CommonParam(0, numPageSize));

            CommonParam param = new CommonParam();
            param.Limit = this.patientRowCount;
            param.Count = this.patientTotalData;
            ucPagingPatient.Init(LoadPagingPatient, param, numPageSize);
        }

        private void LoadPagingPatient(object param)
        {
            try
            {
                this.currentServiceReq = null;
                this.patientStart = ((CommonParam)param).Start ?? 0;
                this.patientLimit = ((CommonParam)param).Limit ?? 0;
                currentListData = new List<ServiceReq8ADO>();
                CommonParam paramCommon = new CommonParam(this.patientStart, this.patientLimit);
                HisServiceReqView8Filter filter = new HisServiceReqView8Filter();
                filter.EXECUTE_ROOM_ID = this.currentModuleBase.RoomId;
                filter.KEY_WORD = txtKeywork.Text;
                filter.IS_KIDNEY = true;
                filter.ORDER_DIRECTION = "DESC";
                filter.ORDER_DIRECTION1 = "DESC";
                filter.ORDER_DIRECTION2 = "DESC";
                filter.ORDER_FIELD = "INTRUCTION_TIME";
                filter.ORDER_FIELD1 = "KIDNEY_SHIFT";
                filter.ORDER_FIELD2 = "MACHINE_NAME";
                filter.SERVICE_REQ_TYPE_IDs = new List<long>()
                {
                    IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__AN,
                    IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__CDHA,
                    IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__G,
                    IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__GPBL,
                    IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__KH,
                    IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__KHAC,
                    IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__NS,
                    IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__PHCN,
                    IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__PT,
                    IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__SA,
                    IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__TDCN,
                    IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__TT,
                    IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__XN,
                };
                if (dtInstructionDateFrom.EditValue != null && dtInstructionDateFrom.DateTime != DateTime.MinValue)
                {
                    filter.INTRUCTION_DATE_FROM = Convert.ToInt64(dtInstructionDateFrom.DateTime.ToString("yyyyMMdd") + "000000");
                }
                if (dtInstructionDateTo.EditValue != null && dtInstructionDateTo.DateTime != DateTime.MinValue)
                {
                    filter.INTRUCTION_DATE_TO = Convert.ToInt64(dtInstructionDateTo.DateTime.ToString("yyyyMMdd") + "000000");
                }
                if (cboStatus.SelectedIndex == 1)
                {
                    filter.SERVICE_REQ_STT_IDs = new List<long>()
                    {
                        IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_STT.ID__CXL,
                        IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_STT.ID__DXL
                    };
                }
                else if (cboStatus.SelectedIndex == 2)
                {
                    filter.SERVICE_REQ_STT_IDs = new List<long>()
                    {
                        IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_STT.ID__CXL
                    };
                }
                else if (cboStatus.SelectedIndex == 3)
                {
                    filter.SERVICE_REQ_STT_IDs = new List<long>()
                    {
                        IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_STT.ID__DXL
                    };
                }
                else if (cboStatus.SelectedIndex == 4)
                {
                    filter.SERVICE_REQ_STT_IDs = new List<long>()
                    {
                        IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_STT.ID__HT
                    };
                }

                var rs = new BackendAdapter(paramCommon).GetRO<List<V_HIS_SERVICE_REQ_8>>("api/HisServiceReq/GetView8", ApiConsumers.MosConsumer, filter, paramCommon);
                if (rs != null)
                {
                    if (rs.Data != null)
                    {
                        Mapper.CreateMap<V_HIS_SERVICE_REQ_8, ServiceReq8ADO>();
                        foreach (var item in rs.Data)
                        {
                            ServiceReq8ADO ado = Mapper.Map<ServiceReq8ADO>(item);
                            ado.CHANGE_SERVICE_REQ_STT_ID = ado.SERVICE_REQ_STT_ID;
                            this.currentListData.Add(ado);
                        }
                    }
                    this.patientRowCount = (currentListData == null ? 0 : currentListData.Count);
                    this.patientTotalData = (rs.Param == null ? 0 : rs.Param.Count ?? 0);
                }

                gridControlPatient.BeginUpdate();
                gridControlPatient.DataSource = currentListData;
                gridControlPatient.EndUpdate();

                FillDataToGriOldPres();

                #region Process has exception
                SessionManager.ProcessTokenLost(paramCommon);
                #endregion
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void FillDataToGriOldPres()
        {
            try
            {
                int numPageSize;
                if (ucPagingOldPres.pagingGrid != null)
                {
                    numPageSize = ucPagingOldPres.pagingGrid.PageSize;
                }
                else
                {
                    numPageSize = 10;
                }

                LoadPagingOldPres(new CommonParam(0, numPageSize));

                CommonParam param = new CommonParam();
                param.Limit = this.oldRowCount;
                param.Count = this.oldTotalData;
                ucPagingOldPres.Init(LoadPagingOldPres, param, numPageSize);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadPagingOldPres(object param)
        {
            try
            {
                this.currentPrescription = null;
                this.oldStart = ((CommonParam)param).Start ?? 0;
                this.oldLimit = ((CommonParam)param).Limit ?? 0;
                List<HIS_SERVICE_REQ> listData = new List<HIS_SERVICE_REQ>();
                CommonParam paramCommon = new CommonParam(this.oldStart, this.oldLimit);
                if (this.currentServiceReq != null)
                {
                    HisServiceReqFilter filter = new HisServiceReqFilter();
                    filter.ORDER_DIRECTION = "DESC";
                    filter.ORDER_FIELD = "INTRUCTION_TIME";
                    filter.TREATMENT_ID = this.currentServiceReq.TREATMENT_ID;
                    filter.SERVICE_REQ_TYPE_ID = IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__DONDT;
                    filter.IS_KIDNEY = true;
                    var rs = new BackendAdapter(paramCommon).GetRO<List<HIS_SERVICE_REQ>>("api/HisServiceReq/Get", ApiConsumers.MosConsumer, filter, paramCommon);
                    if (rs != null)
                    {
                        listData = (List<HIS_SERVICE_REQ>)rs.Data;
                        this.oldRowCount = (listData == null ? 0 : listData.Count);
                        this.oldTotalData = (rs.Param == null ? 0 : rs.Param.Count ?? 0);
                    }
                }
                gridControlOldPres.BeginUpdate();
                gridControlOldPres.DataSource = listData;
                gridControlOldPres.EndUpdate();
                LoadDataToGridDetial();

                #region Process has exception
                SessionManager.ProcessTokenLost(paramCommon);
                #endregion
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadDataToGridDetial()
        {
            try
            {
                List<MetyMatyADO> listData = new List<MetyMatyADO>();
                if (this.currentPrescription != null)
                {
                    HisExpMestMedicineFilter filter = new HisExpMestMedicineFilter();
                    filter.TDL_SERVICE_REQ_ID = this.currentPrescription.ID;
                    var medicines = new BackendAdapter(new CommonParam()).Get<List<HIS_EXP_MEST_MEDICINE>>("api/HisExpMestMedicine/Get", ApiConsumers.MosConsumer, filter, null);
                    if (medicines != null && medicines.Count > 0)
                    {
                        foreach (var item in medicines)
                        {
                            if (!item.TDL_MEDICINE_TYPE_ID.HasValue) continue;
                            MetyMatyADO ado = new MetyMatyADO(item, BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>().FirstOrDefault(o => o.ID == item.TDL_MEDICINE_TYPE_ID.Value));
                            listData.Add(ado);
                        }
                    }

                    HisServiceReqMetyFilter metyFilter = new HisServiceReqMetyFilter();
                    metyFilter.SERVICE_REQ_ID = this.currentPrescription.ID;
                    var metyReqs = new BackendAdapter(new CommonParam()).Get<List<HIS_SERVICE_REQ_METY>>("api/HisServiceReqMety/Get", ApiConsumers.MosConsumer, metyFilter, null);
                    if (metyReqs != null && metyReqs.Count > 0)
                    {
                        foreach (var item in metyReqs)
                        {
                            if (!item.MEDICINE_TYPE_ID.HasValue) continue;
                            MetyMatyADO ado = new MetyMatyADO(item, BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>().FirstOrDefault(o => o.ID == item.MEDICINE_TYPE_ID.Value));
                            listData.Add(ado);
                        }
                    }
                }


                gridControlPresDetail.BeginUpdate();
                gridControlPresDetail.DataSource = listData;
                gridControlPresDetail.EndUpdate();

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void InitComboRepositoryStatus(List<HIS_SERVICE_REQ_STT> data)
        {
            try
            {
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("SERVICE_REQ_STT_CODE", "", 100, 1));
                columnInfos.Add(new ColumnInfo("SERVICE_REQ_STT_NAME", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("SERVICE_REQ_STT_NAME", "ID", columnInfos, false, 350);
                ControlEditorLoader.Load(this.repositoryItemCboStatus__Enable, (data != null ? data.OrderBy(o => o.ID).ToList() : null), controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitComboRepositoryStatus(LookUpEdit cbo, List<HIS_SERVICE_REQ_STT> data)
        {
            try
            {
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("SERVICE_REQ_STT_CODE", "", 100, 1));
                columnInfos.Add(new ColumnInfo("SERVICE_REQ_STT_NAME", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("SERVICE_REQ_STT_NAME", "ID", columnInfos, false, 350);
                ControlEditorLoader.Load(cbo, (data != null ? data.OrderBy(o => o.ID).ToList() : null), controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtKeywork_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    dtInstructionDateFrom.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void dtInstructionDateFrom_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    dtInstructionDateTo.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void dtInstructionDataTo_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    cboStatus.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnFind_Click(object sender, EventArgs e)
        {
            try
            {
                if (!btnFind.Enabled) return;
                WaitingManager.Show();
                FillDataToGridPatient();
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewPatient_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)
                {
                    ServiceReq8ADO pData = (ServiceReq8ADO)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    if (e.Column.FieldName == "STT")
                    {
                        e.Value = e.ListSourceRowIndex + 1 + this.patientStart; //+ ((pagingGrid.CurrentPage - 1) * pagingGrid.PageSize);
                    }
                    else if (e.Column.FieldName == "IMG_STATUS")
                    {
                        try
                        {
                            if (pData.SERVICE_REQ_STT_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_STT.ID__CXL)
                            {
                                e.Value = imageListIcon.Images[0];
                            }
                            else if (pData.SERVICE_REQ_STT_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_STT.ID__DXL)
                            {
                                e.Value = imageListIcon.Images[1];
                            }
                            else if (pData.SERVICE_REQ_STT_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_STT.ID__HT)
                            {
                                e.Value = imageListIcon.Images[4];
                            }
                            else
                            {
                                e.Value = imageListIcon.Images[0];
                            }
                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Error(ex);
                        }
                    }
                    else if (e.Column.FieldName == "HAS_CABINET_PRE")
                    {
                        try
                        {
                            if (pData.KIDNEY_CABINET_SERVICE_REQ_ID.HasValue)
                            {
                                e.Value = imageCollection1.Images[0];
                            }
                            else
                            {
                                e.Value = imageCollection1.Images[2];
                            }
                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Error(ex);
                        }
                    }
                    else if (e.Column.FieldName == "DOB_YEAR")
                    {
                        try
                        {
                            e.Value = pData.TDL_PATIENT_DOB.ToString().Substring(0, 4);
                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Error(ex);
                        }
                    }
                    else if (e.Column.FieldName == "INTRUCTION_DATE_STR")
                    {
                        try
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToDateString(pData.INTRUCTION_DATE);
                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Error(ex);
                        }
                    }
                }

                gridControlPatient.RefreshDataSource();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewPatient_CustomRowCellEdit(object sender, CustomRowCellEditEventArgs e)
        {
            try
            {
                ServiceReq8ADO data = (ServiceReq8ADO)gridViewPatient.GetRow(e.RowHandle);
                if (data != null)
                {
                    if (e.Column.FieldName == "CREATE_CABINET_PRE")
                    {
                        if (data.SERVICE_REQ_STT_ID != IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_STT.ID__HT)
                        {
                            e.RepositoryItem = repositoryItemBtnCabinetPres__Disable;
                        }
                        else
                        {
                            e.RepositoryItem = repositoryItemBtnCabinetPres__Enable;
                        }
                    }
                    else if (e.Column.FieldName == "UPDATE_IN_PRES")
                    {
                        if (data.SERVICE_REQ_STT_ID != IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_STT.ID__CXL
                            || !data.EXECUTE_KIDNEY_SERVICE_REQ_ID.HasValue
                            || data.EXECUTE_KIDNEY_SERVICE_REQ_ID.Value <= 0)
                        {
                            e.RepositoryItem = repositoryItemButton_EditInPres_Disable;
                        }
                        else
                        {
                            e.RepositoryItem = repositoryItemButton_EditInPres_Enable;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewPatient_RowCellClick(object sender, RowCellClickEventArgs e)
        {
            try
            {
                //WaitingManager.Show();
                //this.currentServiceReq = (ServiceReq8ADO)gridViewPatient.GetFocusedRow();
                //FillDataToGriOldPres();
                //WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewPatient_ShownEditor(object sender, EventArgs e)
        {
            try
            {
                GridView view = sender as GridView;
                ServiceReq8ADO data = view.GetFocusedRow() as ServiceReq8ADO;
                if (view.FocusedColumn.FieldName == "CHANGE_SERVICE_REQ_STT_ID" && view.ActiveEditor is LookUpEdit)
                {
                    LookUpEdit editor = view.ActiveEditor as LookUpEdit;
                    if (data != null)
                    {
                        if (data.SERVICE_REQ_STT_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_STT.ID__CXL)
                        {
                            this.InitComboRepositoryStatus(editor, BackendDataWorker.Get<HIS_SERVICE_REQ_STT>());
                        }
                        else if (data.SERVICE_REQ_STT_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_STT.ID__DXL)
                        {
                            this.InitComboRepositoryStatus(editor, BackendDataWorker.Get<HIS_SERVICE_REQ_STT>());
                        }
                        else if (data.SERVICE_REQ_STT_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_STT.ID__HT)
                        {
                            this.InitComboRepositoryStatus(editor, BackendDataWorker.Get<HIS_SERVICE_REQ_STT>().Where(o => o.ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_STT.ID__DXL ||
                                o.ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_STT.ID__HT).ToList());
                        }
                        editor.ShowPopup();
                        //editor.EditValue = data.PATIENT_TYPE_ID;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewPatient_CellValueChanged(object sender, CellValueChangedEventArgs e)
        {
            try
            {
                if (e.Column.FieldName == "CHANGE_SERVICE_REQ_STT_ID")
                {
                    ServiceReq8ADO row = (ServiceReq8ADO)gridViewPatient.GetRow(e.RowHandle);
                    if (row != null && row.SERVICE_REQ_STT_ID != row.CHANGE_SERVICE_REQ_STT_ID)
                    {
                        CommonParam param = new CommonParam();
                        bool success = false;
                        //Finish Request
                        if ((row.SERVICE_REQ_STT_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_STT.ID__CXL
                            || row.SERVICE_REQ_STT_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_STT.ID__DXL)
                            && row.CHANGE_SERVICE_REQ_STT_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_STT.ID__HT)
                        {
                            WaitingManager.Show();
                            HIS_SERVICE_REQ rs = new BackendAdapter(param).Post<HIS_SERVICE_REQ>(HisRequestUriStore.HIS_SERVICE_REQ_FINISH, ApiConsumers.MosConsumer, row.ID, param);
                            if (rs != null)
                            {
                                success = true;
                                row.SERVICE_REQ_STT_ID = row.CHANGE_SERVICE_REQ_STT_ID;
                            }
                            else
                            {
                                success = false;
                                row.CHANGE_SERVICE_REQ_STT_ID = row.SERVICE_REQ_STT_ID;
                            }
                            WaitingManager.Hide();
                            MessageManager.Show(this.ParentForm, param, success);
                        }
                        //Start Request
                        else if (row.SERVICE_REQ_STT_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_STT.ID__CXL
                            && row.CHANGE_SERVICE_REQ_STT_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_STT.ID__DXL)
                        {
                            WaitingManager.Show();
                            HIS_SERVICE_REQ rs = new BackendAdapter(param).Post<HIS_SERVICE_REQ>(HisRequestUriStore.HIS_SERVICE_REQ_START, ApiConsumers.MosConsumer, row.ID, param);
                            if (rs != null)
                            {
                                success = true;
                                row.SERVICE_REQ_STT_ID = row.CHANGE_SERVICE_REQ_STT_ID;
                            }
                            else
                            {
                                success = false;
                                row.CHANGE_SERVICE_REQ_STT_ID = row.SERVICE_REQ_STT_ID;
                            } WaitingManager.Hide();
                            MessageManager.Show(this.ParentForm, param, success);
                        }
                        else if (row.SERVICE_REQ_STT_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_STT.ID__DXL
                            && row.CHANGE_SERVICE_REQ_STT_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_STT.ID__CXL)
                        {
                            WaitingManager.Show();
                            HIS_SERVICE_REQ rs = new BackendAdapter(param).Post<HIS_SERVICE_REQ>(HisRequestUriStore.HIS_SERVICE_REQ_UNSTART, ApiConsumers.MosConsumer, row.ID, param);
                            if (rs != null)
                            {
                                success = true;
                                row.SERVICE_REQ_STT_ID = row.CHANGE_SERVICE_REQ_STT_ID;
                            }
                            else
                            {
                                success = false;
                                row.CHANGE_SERVICE_REQ_STT_ID = row.SERVICE_REQ_STT_ID;
                            } WaitingManager.Hide();
                            MessageManager.Show(this.ParentForm, param, success);
                        }
                        else if (row.SERVICE_REQ_STT_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_STT.ID__HT
                            && row.CHANGE_SERVICE_REQ_STT_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_STT.ID__DXL)
                        {
                            WaitingManager.Show();
                            HIS_SERVICE_REQ rs = new BackendAdapter(param).Post<HIS_SERVICE_REQ>(HisRequestUriStore.HIS_SERVICE_REQ_UNFINISH, ApiConsumers.MosConsumer, row.ID, param);
                            if (rs != null)
                            {
                                success = true;
                                row.SERVICE_REQ_STT_ID = row.CHANGE_SERVICE_REQ_STT_ID;
                            }
                            else
                            {
                                success = false;
                                row.CHANGE_SERVICE_REQ_STT_ID = row.SERVICE_REQ_STT_ID;
                            } WaitingManager.Hide();
                            MessageManager.Show(this.ParentForm, param, success);
                        }
                    }

                    gridControlPatient.RefreshDataSource();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewOldPres_CustomUnboundColumnData(object sender, CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)
                {
                    HIS_SERVICE_REQ pData = (HIS_SERVICE_REQ)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    if (e.Column.FieldName == "INTRUCTION_TIME_STR")
                    {
                        e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(pData.INTRUCTION_TIME);
                    }
                    else if (e.Column.FieldName == "REQUEST_LOGINAME_STR")
                    {
                        e.Value = pData.REQUEST_LOGINNAME + " - " + pData.REQUEST_USERNAME;
                    }
                }

                gridControlOldPres.RefreshDataSource();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewOldPres_RowCellClick(object sender, RowCellClickEventArgs e)
        {
            try
            {
                WaitingManager.Show();
                this.currentPrescription = (HIS_SERVICE_REQ)gridViewOldPres.GetFocusedRow();
                this.LoadDataToGridDetial();
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewPresDetail_CustomUnboundColumnData(object sender, CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)
                {
                    MetyMatyADO pData = (MetyMatyADO)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    if (e.Column.FieldName == "STT")
                    {
                        e.Value = e.ListSourceRowIndex + 1;
                    }
                }

                gridControlPresDetail.RefreshDataSource();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewPresDetail_CustomRowCellEdit(object sender, CustomRowCellEditEventArgs e)
        {
            try
            {
                MetyMatyADO data = (MetyMatyADO)gridViewPresDetail.GetRow(e.RowHandle);
                if (data != null)
                {
                    if (e.Column.FieldName == "IN_PRES_CREATE")
                    {
                        if (this.currentServiceReq != null
                            && this.currentServiceReq.SERVICE_REQ_STT_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_STT.ID__CXL
                            && (!this.currentServiceReq.EXECUTE_KIDNEY_SERVICE_REQ_ID.HasValue || this.currentServiceReq.EXECUTE_KIDNEY_SERVICE_REQ_ID.Value <= 0)
                            && data.IsKidney
                            && data.IsRequest)
                        {
                            e.RepositoryItem = repositoryItemBtn_InPres;
                        }
                        else
                        {
                            e.RepositoryItem = repositoryItemBtn_InPres__Disable;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void repositoryItemBtnCabinetPres__Enable_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                ServiceReq8ADO row = (ServiceReq8ADO)gridViewPatient.GetFocusedRow();
                if (row == null)
                {
                    return;
                }
                if (!row.EXP_MEST_TEMPLATE_ID.HasValue)
                {
                    XtraMessageBox.Show("Yêu cầu chưa được gắn với gói vật tư", "Thông báo", DevExpress.Utils.DefaultBoolean.True);
                    return;
                }

                Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.AssignPrescriptionPK").FirstOrDefault();
                if (moduleData == null) Inventec.Common.Logging.LogSystem.Error("khong tim thay moduleLink = HIS.Desktop.Plugins.AssignPrescriptionPK");
                if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                {
                    WaitingManager.Show();

                    AssignPrescriptionEditADO editADO = null;
                    if (row.KIDNEY_CABINET_SERVICE_REQ_ID.HasValue && row.KIDNEY_CABINET_SERVICE_REQ_ID.Value > 0)
                    {
                        HisServiceReqFilter reqFilter = new HisServiceReqFilter();
                        reqFilter.ID = (long)row.KIDNEY_CABINET_SERVICE_REQ_ID.Value;
                        List<HIS_SERVICE_REQ> requests = new BackendAdapter(new CommonParam()).Get<List<HIS_SERVICE_REQ>>("api/HisServiceReq/Get", ApiConsumers.MosConsumer, reqFilter, null);
                        HIS_SERVICE_REQ req = requests != null ? requests.FirstOrDefault() : null;

                        HisExpMestFilter expFilter = new HisExpMestFilter();
                        expFilter.SERVICE_REQ_ID = reqFilter.ID;
                        List<HIS_EXP_MEST> exps = new BackendAdapter(new CommonParam()).Get<List<HIS_EXP_MEST>>("api/HisExpMest/Get", ApiConsumers.MosConsumer, expFilter, null);
                        HIS_EXP_MEST ex = exps != null ? exps.FirstOrDefault() : null;

                        editADO = new AssignPrescriptionEditADO(req, ex, null);
                        LogSystem.Debug(LogUtil.TraceData("editADO", editADO));
                    }
                    List<object> listArgs = new List<object>();
                    HIS.Desktop.ADO.AssignPrescriptionADO assignServiceADO = new HIS.Desktop.ADO.AssignPrescriptionADO(row.TREATMENT_ID, row.INTRUCTION_TIME, row.ID);
                    assignServiceADO.PatientDob = row.TDL_PATIENT_DOB;
                    assignServiceADO.PatientName = row.TDL_PATIENT_NAME;
                    assignServiceADO.GenderName = row.TDL_PATIENT_GENDER_NAME;
                    assignServiceADO.TreatmentCode = row.TDL_TREATMENT_CODE;
                    assignServiceADO.TreatmentId = row.TREATMENT_ID;
                    assignServiceADO.ExpMestTemplateId = row.EXP_MEST_TEMPLATE_ID ?? 0;
                    assignServiceADO.ServiceReqId = row.ID;
                    assignServiceADO.IsCabinet = true;
                    assignServiceADO.AssignPrescriptionEditADO = editADO;
                    listArgs.Add(assignServiceADO);
                    listArgs.Add(PluginInstance.GetModuleWithWorkingRoom(moduleData, this.currentModuleBase.RoomId, this.currentModuleBase.RoomTypeId));
                    var extenceInstance = PluginInstance.GetPluginInstance(PluginInstance.GetModuleWithWorkingRoom(moduleData, this.currentModuleBase.RoomId, this.currentModuleBase.RoomTypeId), listArgs);
                    if (extenceInstance == null) throw new ArgumentNullException("moduleData is null");
                    WaitingManager.Hide();
                    ((Form)extenceInstance).ShowDialog();
                    FillDataToGridPatient();
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void repositoryItemCboStatus__Enable_Closed(object sender, ClosedEventArgs e)
        {
            try
            {

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void repositoryItemBtn_InPres_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                MetyMatyADO row = (MetyMatyADO)gridViewPresDetail.GetFocusedRow();
                if (row != null && row.ReqMety != null)
                {
                    Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.AssignPrescriptionKidney").FirstOrDefault();
                    if (moduleData == null) Inventec.Common.Logging.LogSystem.Error("khong tim thay moduleLink = HIS.Desktop.Plugins.AssignPrescriptionKidney");
                    if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                    {
                        List<object> listArgs = new List<object>();
                        HIS.Desktop.ADO.AssignPrescriptionKidneyADO assignServiceADO = new HIS.Desktop.ADO.AssignPrescriptionKidneyADO();
                        assignServiceADO.ServiceReq = this.currentPrescription;
                        assignServiceADO.ServiceReqMety = row.ReqMety;
                        assignServiceADO.ServiceReqParentId = this.currentServiceReq.ID;
                        listArgs.Add(assignServiceADO);
                        listArgs.Add(PluginInstance.GetModuleWithWorkingRoom(moduleData, this.currentModuleBase.RoomId, this.currentModuleBase.RoomTypeId));
                        var extenceInstance = PluginInstance.GetPluginInstance(PluginInstance.GetModuleWithWorkingRoom(moduleData, this.currentModuleBase.RoomId, this.currentModuleBase.RoomTypeId), listArgs);
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

        private void toolTipController1_GetActiveObjectInfo(object sender, DevExpress.Utils.ToolTipControllerGetActiveObjectInfoEventArgs e)
        {
            try
            {
                if (e.Info == null && e.SelectedControl == gridControlPatient)
                {
                    GridView view = gridControlPatient.FocusedView as GridView;
                    GridHitInfo info = view.CalcHitInfo(e.ControlMousePosition);
                    if (info.InRowCell && (lastInfo == null || lastRowHandle != info.RowHandle || lastColumn != info.Column))
                    {
                        lastColumn = info.Column;
                        lastRowHandle = info.RowHandle;
                        string text = "";
                        var data = ((ServiceReq8ADO)view.GetRow(info.RowHandle));
                        if (info.Column.FieldName == "IMG_STATUS")
                        {
                            if (data.SERVICE_REQ_STT_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_STT.ID__CXL)
                            {
                                text = "Chưa xử lý";
                            }
                            else if (data.SERVICE_REQ_STT_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_STT.ID__DXL)
                            {
                                text = "Đang xử lý";
                            }

                            else if (data.SERVICE_REQ_STT_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_STT.ID__HT)
                            {
                                text = "Kết thúc";
                            }
                        }
                        else if (info.Column.FieldName == "HAS_CABINET_PRE")
                        {
                            if (data.KIDNEY_CABINET_SERVICE_REQ_ID.HasValue)
                            {
                                text = "Đã tạo gói vật tư chạy thận";
                            }
                            else
                            {
                                text = "Chưa tạo gói vật tư chạy thận";
                            }
                        }
                        lastInfo = new ToolTipControlInfo(new DevExpress.XtraGrid.GridToolTipInfo(view, new DevExpress.XtraGrid.Views.Base.CellToolTipInfo(info.RowHandle, info.Column, "Text")), text);
                    }
                    e.Info = lastInfo;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridControlPatient_Click(object sender, EventArgs e)
        {
            try
            {
                if (gridViewPatient.FocusedRowHandle < 0)
                {
                    return;
                }
                WaitingManager.Show();
                if (gridViewPatient.FocusedColumn.FieldName == "CHANGE_SERVICE_REQ_STT_ID"
                    || gridViewPatient.FocusedColumn.FieldName == "IMG_STATUS"
                    || gridViewPatient.FocusedColumn.FieldName == "HAS_CABINET_PRE"
                    || gridViewPatient.FocusedColumn.FieldName == "CREATE_CABINET_PRE"
                    || gridViewPatient.FocusedColumn.FieldName == "UPDATE_IN_PRES")
                {
                    this.currentServiceReq = null;
                }
                else
                {
                    this.currentServiceReq = (ServiceReq8ADO)gridViewPatient.GetFocusedRow();
                }
                FillDataToGriOldPres();
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public void BNT_FIND()
        {
            try
            {
                btnFind_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void repositoryItemButton_EditInPres_Enable_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                ServiceReq8ADO row = (ServiceReq8ADO)gridViewPatient.GetFocusedRow();
                if (row != null && row.EXECUTE_KIDNEY_SERVICE_REQ_ID.HasValue && row.EXECUTE_KIDNEY_SERVICE_REQ_ID.Value > 0)
                {
                    Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.AssignPrescriptionKidney").FirstOrDefault();
                    if (moduleData == null) Inventec.Common.Logging.LogSystem.Error("khong tim thay moduleLink = HIS.Desktop.Plugins.AssignPrescriptionKidney");
                    if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                    {
                        WaitingManager.Show();
                        List<object> listArgs = new List<object>();
                        AssignPrescriptionEditADO editADO = null;
                        HisServiceReqFilter reqFilter = new HisServiceReqFilter();
                        reqFilter.ID = (long)row.EXECUTE_KIDNEY_SERVICE_REQ_ID.Value;
                        List<HIS_SERVICE_REQ> requests = new BackendAdapter(new CommonParam()).Get<List<HIS_SERVICE_REQ>>("api/HisServiceReq/Get", ApiConsumers.MosConsumer, reqFilter, null);
                        HIS_SERVICE_REQ req = requests != null ? requests.FirstOrDefault() : null;

                        HisExpMestFilter expFilter = new HisExpMestFilter();
                        expFilter.SERVICE_REQ_ID = reqFilter.ID;
                        List<HIS_EXP_MEST> exps = new BackendAdapter(new CommonParam()).Get<List<HIS_EXP_MEST>>("api/HisExpMest/Get", ApiConsumers.MosConsumer, expFilter, null);
                        HIS_EXP_MEST ex = exps != null ? exps.FirstOrDefault() : null;

                        editADO = new AssignPrescriptionEditADO(req, ex, null);
                        LogSystem.Debug(LogUtil.TraceData("editADOIn", editADO));

                        HIS.Desktop.ADO.AssignPrescriptionKidneyADO assignServiceADO = new HIS.Desktop.ADO.AssignPrescriptionKidneyADO();                        
                        assignServiceADO.ServiceReqParentId = row.ID;
                        assignServiceADO.AssignPrescriptionEditADO = editADO;
                        listArgs.Add(assignServiceADO);
                        listArgs.Add(PluginInstance.GetModuleWithWorkingRoom(moduleData, this.currentModuleBase.RoomId, this.currentModuleBase.RoomTypeId));
                        var extenceInstance = PluginInstance.GetPluginInstance(PluginInstance.GetModuleWithWorkingRoom(moduleData, this.currentModuleBase.RoomId, this.currentModuleBase.RoomTypeId), listArgs);

                        if (extenceInstance == null) throw new ArgumentNullException("moduleData is null");
                        WaitingManager.Hide();
                        ((Form)extenceInstance).ShowDialog();
                        FillDataToGridPatient();
                    }
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
