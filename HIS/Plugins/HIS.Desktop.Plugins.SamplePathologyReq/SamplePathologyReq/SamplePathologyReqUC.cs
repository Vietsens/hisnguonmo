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
using Inventec.UC.Paging;
using DevExpress.XtraGrid.Columns;
using DevExpress.Utils;
using MOS.EFMODEL.DataModels;
using HIS.Desktop.LocalStorage.BackendData;
using Inventec.Common.Controls.EditorLoader;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using Inventec.Core;
using HIS.Desktop.LocalStorage.ConfigApplication;
using Inventec.Desktop.Common.Message;
using MOS.Filter;
using MOS.SDO;
using HIS.Desktop.ApiConsumer;
using Inventec.Common.Adapter;
using HIS.Desktop.Controls.Session;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.Data;
using System.Collections;
using DevExpress.XtraGrid.Views.Base;
using HIS.Desktop.Plugins.SamplePathologyReq.Resources;
using System.Resources;
using Inventec.Desktop.Common.LanguageManager;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.DXErrorProvider;
using HIS.Desktop.Plugins.SamplePathologyReq.ADO;

namespace HIS.Desktop.Plugins.SamplePathologyReq.SamplePathologyReq
{
    public partial class SamplePathologyReqUC : UserControlBase
    {
        #region Declare
        V_HIS_ROOM room = null;
        GridColumn lastColumn = null;
        ToolTipControlInfo lastInfo = null;
        int rowCount = 0;
        int dataTotal = 0;
        int startPage = 0;
        int limit = 0;
        int lastRowHandle = -1;
        V_HIS_SERVICE_REQ currentVServiceReq = new V_HIS_SERVICE_REQ();
        Inventec.Core.ApiResultObject<List<HIS_SERVICE_REQ>> apiResult;
        Inventec.Desktop.Common.Modules.Module currentModule;
        List<HIS_SERVICE_REQ> LstHisServiceReq = null;
        CPA.WCFClient.CallPatientClient.CallPatientClientManager clienttManager = null;
        #endregion

        #region Contructor
        public SamplePathologyReqUC()
        {
            InitializeComponent();
        }

        public SamplePathologyReqUC(Inventec.Desktop.Common.Modules.Module currentModule)
            : base(currentModule)
        {
            InitializeComponent();
            try
            {
                this.currentModule = currentModule;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SamplePathologyReqUC_Load(object sender, EventArgs e)
        {
            try
            {
                if (room == null)
                {
                    room = BackendDataWorker.Get<V_HIS_ROOM>().FirstOrDefault(o => o.ID == currentModule.RoomId);
                }

                cboIsNotInDept.SelectedIndex = 1;
                loadcboStatus();
                LoadDefaultData();
                this.gridControl1.ToolTipController = this.toolTipController1;
                SetCaptionByLanguageKey();
                FillDataToGridControl();
                txtGateNumber.Text = this.room.ROOM_CODE;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        #endregion


        #region private

        private void loadcboStatus()
        {
            try
            {
                List<ComboADO> status = new List<ComboADO>();
                //status.Add(new HIS.Desktop.Plugins.SampleCollectionRoom.ComboADO(999, "Chưa trả kết quả"));
                status.Add(new ComboADO(0, "Tất cả"));
                status.Add(new ComboADO(1, "Chưa lấy mẫu"));
                status.Add(new ComboADO(2, "Đã lấy mẫu"));
                status.Add(new ComboADO(3, "Đang xử lý"));
                status.Add(new ComboADO(4, "Hoàn thành"));
                //status.Add(new HIS.Desktop.Plugins.SampleCollectionRoom.ComboADO(IMSys.DbConfig.LIS_RS.LIS_SAMPLE_STT.ID__TRA_KQ, "Trả kết quả"));
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("statusName", "Trạng thái", 50, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("statusName", "id", columnInfos, true, 50);
                ControlEditorLoader.Load(cboServiceReqSttId, status, controlEditorADO);

                cboServiceReqSttId.EditValue = status[1].id;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }

        private void LoadDefaultData()
        {
            try
            {
                txtServiceReqCode.Focus();
                txtServiceReqCode.SelectAll();
                dtIntructionDateFrom.EditValue = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(Inventec.Common.DateTime.Get.StartDay() ?? 0);
                dtIntructionDateTo.EditValue = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(Inventec.Common.DateTime.Get.EndDay() ?? 0);
                gridControl1.DataSource = null;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void FillDataToGridControl()
        {
            try
            {
                txtServiceReqCode.Focus();
                txtServiceReqCode.SelectAll();

                FillDataToGridSample(new CommonParam(0, (int)ConfigApplications.NumPageSize));

                CommonParam param = new CommonParam();
                param.Limit = rowCount;
                param.Count = dataTotal;
                ucPaging1.Init(FillDataToGridSample, param, (int)ConfigApplications.NumPageSize, this.gridControl1);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }

        private void FillDataToGridSample(object param)
        {
            try
            {
                WaitingManager.Show();
                startPage = ((CommonParam)param).Start ?? 0;
                limit = ((CommonParam)param).Limit ?? 0;
                CommonParam paramCommon = new CommonParam(startPage, limit);

                gridControl1.DataSource = null;

                HisServiceReqFilter filter = new HisServiceReqFilter();
                if (cboIsNotInDept.SelectedIndex == 1)
                {
                    filter.IS_NOT_IN_DEBT = true;
                }
                else if (cboIsNotInDept.SelectedIndex == 2)
                {
                    filter.IS_NOT_IN_DEBT = false;
                }
                if (!String.IsNullOrWhiteSpace(txtServiceReqCode.Text) || !string.IsNullOrEmpty(txtBlock.Text) || !String.IsNullOrWhiteSpace(txtTreatmentCode.Text))
                {
                    if (!String.IsNullOrWhiteSpace(txtServiceReqCode.Text))
                    {
                        string code = txtServiceReqCode.Text.Trim();
                        if (code.Length < 12 && checkDigit(code))
                        {
                            code = string.Format("{0:000000000000}", Convert.ToInt64(code));
                            txtServiceReqCode.Text = code;
                        }
                        filter.SERVICE_REQ_CODE__EXACT = code;
                    }

                    if (!string.IsNullOrEmpty(txtBlock.Text))
                    {
                        filter.BLOCK__EXACT = txtBlock.Text.Trim();
                    }

                    if (!String.IsNullOrWhiteSpace(txtTreatmentCode.Text))
                    {
                        string code = txtTreatmentCode.Text.Trim();
                        if (code.Length < 12 && checkDigit(code))
                        {
                            code = string.Format("{0:000000000000}", Convert.ToInt64(code));
                            txtTreatmentCode.Text = code;
                        }
                        filter.TREATMENT_CODE__EXACT = code;
                    }
                }
                else
                {
                    if (!string.IsNullOrEmpty(txtKeyWord.Text))
                    {
                        filter.KEY_WORD = txtKeyWord.Text.Trim();
                    }

                    if (dtIntructionDateFrom != null && dtIntructionDateFrom.DateTime != DateTime.MinValue)
                    {
                        filter.INTRUCTION_DATE_FROM = Inventec.Common.TypeConvert.Parse.ToInt64(Convert.ToDateTime(dtIntructionDateFrom.EditValue).ToString("yyyyMMdd") + "000000");
                    }
                    else
                    {
                        filter.INTRUCTION_DATE_FROM = null;
                    }
                    if (dtIntructionDateTo != null && dtIntructionDateTo.DateTime != DateTime.MinValue)
                    {
                        filter.INTRUCTION_DATE_TO = Inventec.Common.TypeConvert.Parse.ToInt64(Convert.ToDateTime(dtIntructionDateTo.EditValue).ToString("yyyyMMdd") + "235959");
                    }
                    else
                    {
                        filter.INTRUCTION_DATE_TO = null;
                    }
                }

                filter.SERVICE_REQ_TYPE_ID = IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__GPBL;
                filter.EXECUTE_ROOM_ID = this.currentModule.RoomId;
                filter.ORDER_DIRECTION = "DESC";
                filter.ORDER_FIELD = "MODIFY_TIME";

                //Tất cả 0
                //Chưa lấy mẫu 1
                //Đã lấy mẫu 2
                //Đang xử lý 3
                //Hoàn thành 4
                if ((long)cboServiceReqSttId.EditValue == 0)
                {
                    filter.SERVICE_REQ_STT_ID = null;
                }
                else if ((long)cboServiceReqSttId.EditValue == 1)
                {
                    filter.SERVICE_REQ_STT_ID = IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_STT.ID__CXL;
                    filter.HAS_SAMPLED = false;
                }
                else if ((long)cboServiceReqSttId.EditValue == 2)
                {
                    filter.SERVICE_REQ_STT_ID = IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_STT.ID__CXL;
                    filter.HAS_SAMPLED = true;
                }
                else if ((long)cboServiceReqSttId.EditValue == 3)
                {
                    filter.SERVICE_REQ_STT_ID = IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_STT.ID__DXL;
                }
                else if ((long)cboServiceReqSttId.EditValue == 4)
                {
                    filter.SERVICE_REQ_STT_ID = IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_STT.ID__HT;
                }

                Inventec.Common.Logging.LogSystem.Info("HisServiceReqFilter: " + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => filter), filter));

                apiResult = new ApiResultObject<List<HIS_SERVICE_REQ>>();
                gridView1.BeginUpdate();
                apiResult = new BackendAdapter(paramCommon).GetRO<List<HIS_SERVICE_REQ>>(HisRequestUriStore.HIS_SERVICE_REQ_GET, ApiConsumers.MosConsumer, filter, paramCommon);
                if (apiResult != null)
                {
                    WaitingManager.Hide();
                    var data = (List<HIS_SERVICE_REQ>)apiResult.Data;
                    if (data != null)
                    {
                        AutoMapper.Mapper.CreateMap<HIS_SERVICE_REQ, ServiceReqADO>();
                        gridView1.GridControl.DataSource = AutoMapper.Mapper.Map<List<ServiceReqADO>>(data);
                        rowCount = (data == null ? 0 : data.Count);
                        dataTotal = (apiResult.Param == null ? 0 : apiResult.Param.Count ?? 0);
                        if (data.Count == 1)
                        {
                            gridView1.FocusedRowHandle = 0;
                        }
                    }
                    #region Process has exception
                    SessionManager.ProcessTokenLost((CommonParam)param);
                    #endregion
                }
                gridView1.EndUpdate();
                WaitingManager.Hide();

            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }


        private void toolTipController1_GetActiveObjectInfo(object sender, ToolTipControllerGetActiveObjectInfoEventArgs e)
        {
            try
            {
                if (e.Info == null && e.SelectedControl == gridControl1)
                {
                    DevExpress.XtraGrid.Views.Grid.GridView view = gridControl1.FocusedView as DevExpress.XtraGrid.Views.Grid.GridView;
                    GridHitInfo info = view.CalcHitInfo(e.ControlMousePosition);
                    if (info.InRowCell)
                    {
                        if (lastRowHandle != info.RowHandle || lastColumn != info.Column)
                        {
                            lastColumn = info.Column;
                            lastRowHandle = info.RowHandle;
                            string text = "";
                            if (info.Column.FieldName == "STATUS")
                            {
                                var busyCount = ((ServiceReqADO)view.GetRow(lastRowHandle));
                                if (busyCount.SERVICE_REQ_STT_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_STT.ID__CXL && busyCount.IS_SAMPLED != 1)
                                {
                                    text = "Chưa lấy mẫu";
                                }
                                else if (busyCount.SERVICE_REQ_STT_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_STT.ID__CXL && busyCount.IS_SAMPLED == 1)
                                {
                                    text = "Đã lấy mẫu";
                                }

                                else if (busyCount.SERVICE_REQ_STT_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_STT.ID__DXL)
                                {
                                    text = "Đang xử lý";
                                }

                                else if (busyCount.SERVICE_REQ_STT_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_STT.ID__HT)
                                {
                                    text = "Hoàn thành";
                                }
                            }
                            else if (info.Column.FieldName == "Call")
                            {
                                text = "Gọi bệnh nhân";
                            }
                            else if (info.Column.FieldName == "GET_SAMPLE")
                            {
                                var data = ((ServiceReqADO)view.GetRow(lastRowHandle));
                                if (data.SERVICE_REQ_STT_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_STT.ID__CXL && data.IS_SAMPLED != 1)
                                {
                                    text = "Lấy mẫu";
                                }
                                else if (data.SERVICE_REQ_STT_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_STT.ID__CXL && data.IS_SAMPLED == 1)
                                {
                                    text = "Hủy lấy mẫu";
                                }
                            }
                            else if (info.Column.FieldName == "PRINT")
                            {
                                text = "In mẫu giải phẫu bệnh";
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

        private bool checkDigit(string s)
        {
            bool result = false;
            try
            {
                for (int i = 0; i < s.Length; i++)
                {
                    if (char.IsDigit(s[i]) == true) result = true;
                    else result = false;
                }
                return result;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                return result;
            }
        }
        #endregion

        private void txtServiceReqCode_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (!String.IsNullOrWhiteSpace(txtServiceReqCode.Text))
                {
                    string code = txtServiceReqCode.Text.Trim();
                    if (code.Length < 12 && checkDigit(code))
                    {
                        code = string.Format("{0:000000000000}", Convert.ToInt64(code));
                        txtServiceReqCode.Text = code;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }

        private void txtTreatmentCode_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (!String.IsNullOrWhiteSpace(txtTreatmentCode.Text))
                {
                    string code = txtTreatmentCode.Text.Trim();
                    if (code.Length < 12 && checkDigit(code))
                    {
                        code = string.Format("{0:000000000000}", Convert.ToInt64(code));
                        txtTreatmentCode.Text = code;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtServiceReqCode_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    btnSearch_Click(null, null);
                    txtBlock.Focus();
                    txtBlock.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }

        private void txtBlock_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    btnSearch_Click(null, null);
                    txtTreatmentCode.Focus();
                    txtTreatmentCode.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtTreatmentCode_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    btnSearch_Click(null, null);
                    txtKeyWord.Focus();
                    txtKeyWord.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtKeyWord_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    btnSearch_Click(null, null);
                    cboServiceReqSttId.Focus();
                    cboServiceReqSttId.ShowPopup();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboServiceReqSttId_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    btnSearch_Click(null, null);
                    dtIntructionDateFrom.Focus();
                    dtIntructionDateFrom.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void dtIntructionDateFrom_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    dtIntructionDateTo.Focus();
                    dtIntructionDateTo.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void dtIntructionDateTo_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (btnSearch.Enabled)
                    {
                        btnSearch.Focus();
                        e.Handled = true;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            try
            {
                FillDataToGridControl();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }

        public void barSave_ItemClick()
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

        public void bbtnSearch_ItemClick()
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

        public void bbtnF2_ItemClick()
        {
            try
            {
                txtServiceReqCode.Focus();
                txtServiceReqCode.SelectAll();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }

        public void bbtnF3_ItemClick()
        {
            try
            {
                txtKeyWord.Focus();
                txtKeyWord.SelectAll();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }

        public void bbtnCall_ItemClick()
        {
            try
            {
                btnCall_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public void bbtnCallBack_ItemClick()
        {
            try
            {
                btnCallBack_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void btnCall_Click(object sender, EventArgs e)
        {
            try
            {
                CreateThreadCallPatient();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }

        private void btnCallBack_Click(object sender, EventArgs e)
        {
            try
            {
                if (!btnCallBack.Enabled)
                    return;
                CreateThreadReCallPatient();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridView1_CustomRowCellEdit(object sender, DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs e)
        {
            try
            {
                GridView View = sender as GridView;
                if (e.RowHandle >= 0)
                {
                    var ServiceReqStt = Inventec.Common.TypeConvert.Parse.ToInt64((gridView1.GetRowCellValue(e.RowHandle, "SERVICE_REQ_STT_ID")).ToString());
                    var data = (ServiceReqADO)gridView1.GetRow(e.RowHandle);
                    if (data == null) return;

                    if (e.Column.FieldName == "GET_SAMPLE")
                    {
                        if (ServiceReqStt == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_STT.ID__CXL && data.IS_SAMPLED != 1)
                        {
                            e.RepositoryItem = RepbtnSample;
                        }
                        else if (ServiceReqStt == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_STT.ID__CXL && data.IS_SAMPLED == 1)
                        {
                            e.RepositoryItem = RepbbtnUnSample;
                        }
                        else
                        {
                            e.RepositoryItem = RepbbtnDisable;
                        }
                    }
                    else if (e.Column.FieldName == "BLOCK")
                    {
                        if (ServiceReqStt != IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_STT.ID__HT) //  && data.IS_SAMPLED != 1
                        {
                            e.RepositoryItem = repositoryItemTextEditBlockE;
                        }
                        else
                        {
                            e.RepositoryItem = repositoryItemTextEditBlockD;
                        }
                    }
                    else if (e.Column.FieldName == "TDL_INSTRUCTION_NOTE")
                    {
                        if (ServiceReqStt == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_STT.ID__CXL && data.IS_SAMPLED != 1)
                        {
                            e.RepositoryItem = repositoryItemTextEditNoteE;
                        }
                        else
                        {
                            e.RepositoryItem = repositoryItemTextEditNoteD;
                        }
                    }
                    else if (e.Column.FieldName == "Call")
                    {
                        e.RepositoryItem = RepbtnCall;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }

        private void gridView1_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)
                {
                    HIS_SERVICE_REQ data = (ServiceReqADO)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    if (data != null)
                    {
                        if (e.Column.FieldName == "STT")
                        {
                            e.Value = e.ListSourceRowIndex + 1 + startPage;
                        }
                        else if (e.Column.FieldName == "STATUS")
                        {
                            //Chưa lấy mẫu - màu trăng
                            //Đã lấy mẫu - vàng
                            //Đang xử lý - màu xanh lá
                            //Hoàn thành - màu xanh da trời
                            long statusId = data.SERVICE_REQ_STT_ID;
                            if (statusId == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_STT.ID__CXL && data.IS_SAMPLED != 1)
                            {
                                e.Value = imageListIcon.Images[0];
                            }
                            else if (statusId == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_STT.ID__CXL && data.IS_SAMPLED == 1)
                            {
                                e.Value = imageListIcon.Images[1];
                            }
                            else if (statusId == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_STT.ID__DXL)
                            {
                                e.Value = imageListIcon.Images[2];
                            }
                            else if (statusId == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_STT.ID__HT)
                            {
                                e.Value = imageListIcon.Images[3];
                            }
                        }
                        else if (e.Column.FieldName == "SAMPLER_STR")
                        {
                            e.Value = data.SAMPLER_LOGINNAME + " - " + data.SAMPLER_USERNAME;
                        }
                        else if (e.Column.FieldName == "REQUEST_STR")
                        {
                            e.Value = data.REQUEST_LOGINNAME + " - " + data.REQUEST_USERNAME;
                        }
                        else if (e.Column.FieldName == "REQUEST_DEPARTMENT_STR")
                        {
                            var Department = BackendDataWorker.Get<HIS_DEPARTMENT>().Where(o => o.ID == data.REQUEST_DEPARTMENT_ID).FirstOrDefault();

                            e.Value = Department != null ? Department.DEPARTMENT_NAME : null;
                        }
                        else if (e.Column.FieldName == "TDL_PATIENT_DOB_STR")
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToDateString(data.TDL_PATIENT_DOB);
                        }
                        else if (e.Column.FieldName == "INTRUCTION_TIME_STR")
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(data.INTRUCTION_TIME);
                        }
                        else if (e.Column.FieldName == "SAMPLE_TIME_STR")
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(data.SAMPLE_TIME ?? 0);
                        }
                        else if (e.Column.FieldName == "CREATE_TIME_STR")
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(data.CREATE_TIME ?? 0);
                        }
                        else if (e.Column.FieldName == "MODIFY_TIME_STR")
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(data.MODIFY_TIME ?? 0);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }

        private void RepbtnSample_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                WaitingManager.Show();
                bool result = false;
                CommonParam param = new CommonParam();
                var row = (ServiceReqADO)gridView1.GetFocusedRow();
                if (!string.IsNullOrEmpty(row.BLOCK))
                {
                    var resultData = new BackendAdapter(param).Post<HIS_SERVICE_REQ>(HisRequestUriStore.HIS_SERVICE_REQ_PAAN_TAKE_SAMPLE, ApiConsumers.MosConsumer, row.ID, param);
                    if (resultData != null)
                    {
                        result = true;
                        FillDataToGridControl();
                        gridView1.RefreshData();
                    }

                    #region Hien thi message thong bao
                    MessageManager.Show(this.ParentForm, param, result);
                    #endregion

                    #region Neu phien lam viec bi mat, phan mem tu dong logout va tro ve trang login
                    SessionManager.ProcessTokenLost(param);
                    #endregion
                }
                else
                {
                    MessageBox.Show(ResourceMessage.ChuaNhapBlock);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }

        private void RepbbtnUnSample_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                WaitingManager.Show();
                bool result = false;
                CommonParam param = new CommonParam();
                var row = (ServiceReqADO)gridView1.GetFocusedRow();
                var resultData = new BackendAdapter(param).Post<HIS_SERVICE_REQ>(HisRequestUriStore.HIS_SERVICE_REQ_PAAN_CANCEL_SAMPLE, ApiConsumers.MosConsumer, row.ID, param);
                if (resultData != null)
                {
                    result = true;
                    FillDataToGridControl();
                    gridView1.RefreshData();
                }

                #region Hien thi message thong bao
                MessageManager.Show(this.ParentForm, param, result);
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

        private void repbtnPrint_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                PrintProcess(PrintType.LAY_MAU_GIAI_PHAU_BENH);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }

        internal enum PrintType
        {
            LAY_MAU_GIAI_PHAU_BENH,
        }

        void PrintProcess(PrintType printType)
        {
            try
            {
                Inventec.Common.RichEditor.RichEditorStore richEditorMain = new Inventec.Common.RichEditor.RichEditorStore(HIS.Desktop.ApiConsumer.ApiConsumers.SarConsumer, HIS.Desktop.LocalStorage.ConfigSystem.ConfigSystems.URI_API_SAR, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetLanguage(), HIS.Desktop.LocalStorage.Location.PrintStoreLocation.PrintTemplatePath);

                switch (printType)
                {
                    case PrintType.LAY_MAU_GIAI_PHAU_BENH:
                        richEditorMain.RunPrintTemplate(MPS.Processor.Mps000428.PDO.Mps000428PDO.PrintTypeCode.Mps000428, DelegateRunPrinter);
                        break;
                    default:
                        break;
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        bool DelegateRunPrinter(string printTypeCode, string fileName)
        {
            bool result = false;
            try
            {
                switch (printTypeCode)
                {
                    case MPS.Processor.Mps000428.PDO.Mps000428PDO.PrintTypeCode.Mps000428:
                        LoadBieuMauLayMauGiaiPhauBenh(printTypeCode, fileName, ref result);
                        break;
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

            return result;
        }

        private void LoadBieuMauLayMauGiaiPhauBenh(string printTypeCode, string fileName, ref bool result)
        {
            try
            {
                var row = (ServiceReqADO)gridView1.GetFocusedRow();
                WaitingManager.Show();
                currentVServiceReq = new V_HIS_SERVICE_REQ();
                CommonParam param = new CommonParam();
                HisServiceReqFilter filter = new HisServiceReqFilter();
                filter.ID = row.ID;
                currentVServiceReq = new BackendAdapter(param).Get<List<V_HIS_SERVICE_REQ>>(HisRequestUriStore.HIS_SERVICE_REQ_GETVIEW, ApiConsumers.MosConsumer, filter, param).FirstOrDefault();
                MPS.Processor.Mps000428.PDO.Mps000428PDO mps000428RDO = new MPS.Processor.Mps000428.PDO.Mps000428PDO(currentVServiceReq);
                WaitingManager.Hide();

                MPS.ProcessorBase.Core.PrintData PrintData = null;
                if (HIS.Desktop.LocalStorage.LocalData.GlobalVariables.CheDoInChoCacChucNangTrongPhanMem == 2)
                {
                    PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000428RDO, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, "");
                }
                else
                {
                    PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000428RDO, MPS.ProcessorBase.PrintConfig.PreviewType.ShowDialog, "");
                }
                PrintData.ShowPrintLog = (MPS.ProcessorBase.PrintConfig.DelegateShowPrintLog)CallModuleShowPrintLog;
                result = MPS.MpsPrinter.Run(PrintData);
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }

        private void CallModuleShowPrintLog(string printTypeCode, string uniqueCode)
        {
            try
            {
                if (!String.IsNullOrWhiteSpace(printTypeCode) && !String.IsNullOrWhiteSpace(uniqueCode))
                {
                    //goi modul
                    HIS.Desktop.ADO.PrintLogADO ado = new HIS.Desktop.ADO.PrintLogADO(printTypeCode, uniqueCode);

                    List<object> listArgs = new List<object>();
                    listArgs.Add(ado);

                    HIS.Desktop.ModuleExt.PluginInstanceBehavior.ShowModule("Inventec.Desktop.Plugins.PrintLog", this.currentModule.RoomId, this.currentModule.RoomTypeId, listArgs);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        List<ServiceReqADO> vhisderreq = new List<ServiceReqADO>();
        private void gridView1_CellValueChanged(object sender, CellValueChangedEventArgs e)
        {
            try
            {
                ServiceReqADO vHisService = (ServiceReqADO)gridView1.GetFocusedRow();

                if (e.Column.FieldName == "BLOCK")
                {
                    if (Encoding.UTF8.GetByteCount(vHisService.BLOCK) > 30)
                    {
                        vHisService.ErrorMessageBlock = "Trường block đang nhập quá 30 ký tự cho phép"; ;
                        vHisService.ErrorTypeBlock = ErrorType.Warning;
                    }
                    else
                    {
                        vHisService.ErrorMessageBlock = "";
                        vHisService.ErrorTypeBlock = ErrorType.None;
                    }

                    var focus = (ServiceReqADO)gridView1.GetFocusedRow();
                    vhisderreq.Add(focus);
                    btnSave.Focus();
                }
                if (e.Column.FieldName == "TDL_INSTRUCTION_NOTE")
                {
                    if (Encoding.UTF8.GetByteCount(vHisService.TDL_INSTRUCTION_NOTE) > 3000)
                    {
                        vHisService.ErrorMessageTDN = "Trường ghi chú đang nhập quá 3000 ký tự cho phép"; ;
                        vHisService.ErrorTypeTDN = ErrorType.Warning;
                    }
                    else
                    {
                        vHisService.ErrorMessageTDN = "";
                        vHisService.ErrorTypeTDN = ErrorType.None;

                        var focus = (ServiceReqADO)gridView1.GetFocusedRow();
                        ProcessUpdateNote(focus);
                    }

                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ProcessUpdateNote(HIS_SERVICE_REQ sample)
        {
            try
            {
                CommonParam param = new CommonParam();
                bool result = false;
                PaanIntructionNoteSDO input = new PaanIntructionNoteSDO();
                input.ServiceReqId = sample.ID;
                input.IntructionNote = sample.TDL_INSTRUCTION_NOTE;

                WaitingManager.Show();
                var rs = new BackendAdapter(param).Post<HIS_SERVICE_REQ>(HisRequestUriStore.HIS_SERVICE_REQ_PAAN_INTRUCTION_NOTE, ApiConsumers.MosConsumer, input, param);
                if (rs != null)
                {
                    result = true;
                }
                else
                {
                    result = false;
                    FillDataToGridControl();
                }
                Inventec.Common.Logging.LogSystem.Info("input Note: " + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => input), input));
                Inventec.Common.Logging.LogSystem.Info("output Note: " + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => rs), rs));
                WaitingManager.Hide();
                MessageManager.Show(this.ParentForm, param, result);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void btnSave_Click(object sender, EventArgs e)
        {
            btnSave.Focus();
            var focus_ = (List<ServiceReqADO>)gridView1.DataSource;
            ProcessUpdateBlock(focus_);
        }

        private void ProcessUpdateBlock(List<ServiceReqADO> sample)
        {
            try
            {
                if (sample.Exists(o => !string.IsNullOrEmpty(o.ErrorMessageBlock) || !string.IsNullOrEmpty(o.ErrorMessageTDN)))
                    return;
                WaitingManager.Show();       
                List<PaanBlockSDO> listinput = new List<PaanBlockSDO>();

                CommonParam param_ = new CommonParam();
                HisServiceReqFilter filter = new HisServiceReqFilter();
                filter.IDs = sample.Select(o => o.ID).ToList();
                var check = new BackendAdapter(param_).Get<List<V_HIS_SERVICE_REQ>>(HisRequestUriStore.HIS_SERVICE_REQ_GET, ApiConsumers.MosConsumer, filter, param_);

                foreach (var item in sample)
                {
                    if (!string.IsNullOrEmpty(item.BLOCK))
                    {
                        if (check.Where(o => o.ID == item.ID && o.BLOCK != item.BLOCK) != null && check.Where(o => o.ID == item.ID && o.BLOCK != item.BLOCK).Count() > 0)
                        {
                            PaanBlockSDO input = new PaanBlockSDO();
                            input.ServiceReqId = item.ID;
                            input.Block = item.BLOCK;
                            listinput.Add(input);
                        }
                    }
                }
                if (listinput != null && listinput.Count > 0)
                {
                    bool result = false;
                    CommonParam param = new CommonParam();
                    var rs = new BackendAdapter(param).Post<List<HIS_SERVICE_REQ>>("/api/HisServiceReq/ListPaanBlock", ApiConsumers.MosConsumer, listinput, param);
                    if (rs != null)
                    {
                        result = true;
                    }
                    else
                    {
                        result = false;
                    }
                    FillDataToGridControl();
                    Inventec.Common.Logging.LogSystem.Info("output Block: " + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => rs), rs));
                    WaitingManager.Hide();
                    MessageManager.Show(this.ParentForm, param, result);
                }
                Inventec.Common.Logging.LogSystem.Info("input Block: " + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => listinput), listinput));
                
                WaitingManager.Hide();

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SetCaptionByLanguageKey()
        {
            try
            {
                if (this.currentModule != null && !String.IsNullOrEmpty(this.currentModule.text))
                {
                    this.Text = this.currentModule.text;
                }

                ////Khoi tao doi tuong resource
                Resources.ResourceLanguageManager.LanguageResource = new ResourceManager("HIS.Desktop.Plugins.SamplePathologyReq.Resources.Lang", typeof(HIS.Desktop.Plugins.SamplePathologyReq.SamplePathologyReq.SamplePathologyReqUC).Assembly);

                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.layoutControl1.Text = Inventec.Common.Resource.Get.Value("UC_SamplePathologyReq.layoutControl1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl2.Text = Inventec.Common.Resource.Get.Value("UC_SamplePathologyReq.layoutControl2.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());

                this.gridColumn1.Caption = Inventec.Common.Resource.Get.Value("UC_SamplePathologyReq.gridColumn1.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn2.Caption = Inventec.Common.Resource.Get.Value("UC_SamplePathologyReq.gridColumn2.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn3.Caption = Inventec.Common.Resource.Get.Value("UC_SamplePathologyReq.gridColumn3.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn4.Caption = Inventec.Common.Resource.Get.Value("UC_SamplePathologyReq.gridColumn4.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn5.Caption = Inventec.Common.Resource.Get.Value("UC_SamplePathologyReq.gridColumn5.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn6.Caption = Inventec.Common.Resource.Get.Value("UC_SamplePathologyReq.gridColumn6.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn7.Caption = Inventec.Common.Resource.Get.Value("UC_SamplePathologyReq.gridColumn7.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn8.Caption = Inventec.Common.Resource.Get.Value("UC_SamplePathologyReq.gridColumn8.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn9.Caption = Inventec.Common.Resource.Get.Value("UC_SamplePathologyReq.gridColumn9.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn10.Caption = Inventec.Common.Resource.Get.Value("UC_SamplePathologyReq.gridColumn10.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn11.Caption = Inventec.Common.Resource.Get.Value("UC_SamplePathologyReq.gridColumn11.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn12.Caption = Inventec.Common.Resource.Get.Value("UC_SamplePathologyReq.gridColumn12.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn13.Caption = Inventec.Common.Resource.Get.Value("UC_SamplePathologyReq.gridColumn13.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn14.Caption = Inventec.Common.Resource.Get.Value("UC_SamplePathologyReq.gridColumn14.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn15.Caption = Inventec.Common.Resource.Get.Value("UC_SamplePathologyReq.gridColumn15.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn16.Caption = Inventec.Common.Resource.Get.Value("UC_SamplePathologyReq.gridColumn16.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn17.Caption = Inventec.Common.Resource.Get.Value("UC_SamplePathologyReq.gridColumn17.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn18.Caption = Inventec.Common.Resource.Get.Value("UC_SamplePathologyReq.gridColumn18.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn19.Caption = Inventec.Common.Resource.Get.Value("UC_SamplePathologyReq.gridColumn19.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn20.Caption = Inventec.Common.Resource.Get.Value("UC_SamplePathologyReq.gridColumn20.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn21.Caption = Inventec.Common.Resource.Get.Value("UC_SamplePathologyReq.gridColumn21.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn22.Caption = Inventec.Common.Resource.Get.Value("UC_SamplePathologyReq.gridColumn22.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn23.Caption = Inventec.Common.Resource.Get.Value("UC_SamplePathologyReq.gridColumn23.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());

                this.txtServiceReqCode.Properties.NullValuePrompt = Inventec.Common.Resource.Get.Value("UC_SamplePathologyReq.txtServiceReqCode.Properties.NullValuePrompt", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.txtBlock.Properties.NullValuePrompt = Inventec.Common.Resource.Get.Value("UC_SamplePathologyReq.txtBlock.Properties.NullValuePrompt", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.txtTreatmentCode.Properties.NullValuePrompt = Inventec.Common.Resource.Get.Value("UC_SamplePathologyReq.txtTreatmentCode.Properties.NullValuePrompt", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.txtKeyWord.Properties.NullValuePrompt = Inventec.Common.Resource.Get.Value("UC_SamplePathologyReq.txtKeyWord.Properties.NullValuePrompt", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.txtGateNumber.Properties.NullValuePrompt = Inventec.Common.Resource.Get.Value("UC_SamplePathologyReq.txtGateNumber.Properties.NullValuePrompt", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.txtStepNumber.Properties.NullValuePrompt = Inventec.Common.Resource.Get.Value("UC_SamplePathologyReq.txtStepNumber.Properties.NullValuePrompt", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());

                this.btnSearch.Text = Inventec.Common.Resource.Get.Value("UC_SamplePathologyReq.btnSearch.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnCall.Text = Inventec.Common.Resource.Get.Value("UC_SamplePathologyReq.btnCall.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnCallBack.Text = Inventec.Common.Resource.Get.Value("UC_SamplePathologyReq.btnCallBack.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridView1_CustomRowError(object sender, Inventec.Desktop.CustomControl.RowColumnErrorEventArgs e)
        {
            try
            {
                var index = this.gridView1.GetDataSourceRowIndex(e.RowHandle);
                if (index < 0)
                {
                    e.Info.ErrorType = ErrorType.None;
                    e.Info.ErrorText = "";
                    return;
                }
                var listDatas = this.gridControl1.DataSource as List<ServiceReqADO>;
                var row = listDatas[index];
                if (e.ColumnName == "BLOCK")
                {
                    if (row.ErrorTypeBlock == ErrorType.Warning)
                    {
                        e.Info.ErrorType = (ErrorType)(row.ErrorTypeBlock);
                        e.Info.ErrorText = (string)(row.ErrorMessageBlock);
                    }
                    else
                    {
                        e.Info.ErrorType = (ErrorType)(ErrorType.None);
                        e.Info.ErrorText = "";
                    }
                }
                if (e.ColumnName == "TDL_INSTRUCTION_NOTE")
                {
                    if (row.ErrorTypeTDN == ErrorType.Warning)
                    {
                        e.Info.ErrorType = (ErrorType)(row.ErrorTypeTDN);
                        e.Info.ErrorText = (string)(row.ErrorMessageTDN);
                    }
                    else
                    {
                        e.Info.ErrorType = (ErrorType)(ErrorType.None);
                        e.Info.ErrorText = "";
                    }
                }
            }
            catch (Exception ex)
            {
                try
                {
                    e.Info.ErrorType = (ErrorType)(ErrorType.None);
                    e.Info.ErrorText = "";
                }
                catch { }

                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridView1_CustomRowColumnError(object sender, Inventec.Desktop.CustomControl.RowColumnErrorEventArgs e)
        {
            try
            {
                if (e.ColumnName == "BLOCK")
                {
                    this.gridView1_CustomRowError(sender, e);
                }
                if (e.ColumnName == "TDL_INSTRUCTION_NOTE")
                {
                    this.gridView1_CustomRowError(sender, e);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        //private void repositoryItemTextEditNoteE_EditValueChanged(object sender, EventArgs e)
        //{
        //    try
        //    {
        //        // var focus = (ServiceReqADO)gridView1.GetFocusedRow();
        //        btnSave.Focus();
        //    }
        //    catch (Exception ex)
        //    {
        //        Inventec.Common.Logging.LogSystem.Error(ex);
        //    }
        //}

    }
}
