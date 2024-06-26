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
using DevExpress.XtraEditors;
using DevExpress.XtraGrid.Views.Base;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.LocalStorage.ConfigApplication;
using HIS.Desktop.Plugins.ExpMestSaleCreate.ADO;
using HIS.Desktop.Utility;
using Inventec.Common.Controls.EditorLoader;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using Inventec.Desktop.Common.Modules;
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
using System.Threading;
namespace HIS.Desktop.Plugins.ExpMestSaleCreate.FormChoosePrescription
{
    public delegate void ChoosePrescription(List<V_HIS_SERVICE_REQ_11> listData);

    public partial class FormPrescription : FormBase
    {
        ChoosePrescription ChooseData;
        int rowCount = 0;
        int dataTotal = 0;
        int start = 0;
        Module module;
        List<V_HIS_SERVICE_REQ_11> ListSelectedData = new List<V_HIS_SERVICE_REQ_11>();

        public FormPrescription(Module module, ChoosePrescription choose)
            : base(module)
        {
            InitializeComponent();
            this.ChooseData = choose;
            try
            {
                this.module = module;
                string iconPath = System.IO.Path.Combine(HIS.Desktop.LocalStorage.Location.ApplicationStoreLocation.ApplicationStartupPath, System.Configuration.ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]);
                this.Icon = Icon.ExtractAssociatedIcon(iconPath);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            
        }

        private void FormPrescription_Load(object sender, EventArgs e)
        {
            try
            {
                InitComboStatus();
                SetDefaultData();
                FillDataToGrid();
                timer1.Start();
               
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetDefaultData()
        {
            try
            {
                this.txtKeyword.Focus();
                this.txtKeyword.SelectAll();
                this.dtIntructionDatefrom.DateTime = DateTime.Now;
                this.dtIntructionDateTo.DateTime = DateTime.Now;
                this.txtKeyword.Text = "";
                this.txtPatientCode.Text = "";
                this.txtServiceReqCode.Text = "";
                this.txtTreatmentCode.Text = "";
                cboStatus.EditValue = StatusADO.Status.ChuaTao;
              
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void InitComboStatus()
        {
            try
            {
                List<StatusADO> status = new List<StatusADO>();
                status.Add(new StatusADO(StatusADO.Status.ChuaTao, "Chưa tạo phiếu xuất"));
                status.Add(new StatusADO(StatusADO.Status.DaTao, "Đã tạo phiếu xuất"));
                status.Add(new StatusADO(StatusADO.Status.TatCa, "Tất cả"));

                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("NAME", "", 100, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("NAME", "ID", columnInfos, false, 110);
                ControlEditorLoader.Load(cboStatus, status, controlEditorADO);
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
                FillDataToGrid();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void FillDataToGrid()
        {
            try
            {
                int pagingSize = ucPaging1.pagingGrid != null ? ucPaging1.pagingGrid.PageSize : (int)ConfigApplications.NumPageSize;
                FillDataToGridSearch(new CommonParam(0, pagingSize));

                CommonParam param = new CommonParam();
                param.Limit = rowCount;
                param.Count = dataTotal;
                ucPaging1.Init(FillDataToGridSearch, param, pagingSize, this.gridControlServiceReqSearch);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void FillDataToGridSearch(object param)
        {
            try
            {
                WaitingManager.Show();
                List<V_HIS_SERVICE_REQ_11> listData = new List<V_HIS_SERVICE_REQ_11>();
                gridControlServiceReqSearch.DataSource = null;
                start = ((CommonParam)param).Start ?? 0;
                var limit = ((CommonParam)param).Limit ?? 0;
                CommonParam paramCommon = new CommonParam(start, limit);
                HisServiceReqView11Filter filter = new HisServiceReqView11Filter();
                SetFilter(ref filter);
                //Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => filter), filter));
                var result = new Inventec.Common.Adapter.BackendAdapter(paramCommon).GetRO<List<V_HIS_SERVICE_REQ_11>>("api/HisServiceReq/GetView11", ApiConsumers.MosConsumer, filter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, paramCommon);
                if (result != null)
                {
                    rowCount = (result.Data == null ? 0 : result.Data.Count);
                    dataTotal = (result.Param == null ? 0 : result.Param.Count ?? 0);
                    if (result.Data != null && result.Data.Count > 0)
                    {
                        listData = result.Data;
                    }
                    else
                    {
                        listData = null;
                    }
                }

                gridControlServiceReqSearch.BeginUpdate();
                gridControlServiceReqSearch.DataSource = listData;
                gridControlServiceReqSearch.EndUpdate();
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetFilter(ref HisServiceReqView11Filter filter)
        {
            try
            {
                if (filter == null) filter = new HisServiceReqView11Filter();
                filter.ORDER_FIELD = "MODIFY_TIME";
                filter.ORDER_DIRECTION = "DESC";
                filter.SERVICE_REQ_TYPE_IDs = new List<long>()
                {
                    IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__DONDT,
                    IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__DONTT,
                    IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__DONK
                };
                filter.ROOM_ID_EQUAL_EXECUTE_OR_REQUEST_EQUAL_EXECUTE = module.RoomId;

                if (!String.IsNullOrEmpty(txtServiceReqCode.Text.Trim()))
                {
                    string code = txtServiceReqCode.Text.Trim();
                    try
                    {
                        code = string.Format("{0:000000000000}", Convert.ToInt64(code));
                    }
                    catch (Exception) { }
                    filter.SERVICE_REQ_CODE = code;
                    txtServiceReqCode.Text = code;
                }
                else if (!String.IsNullOrEmpty(txtTreatmentCode.Text.Trim()))
                {
                    string code = txtTreatmentCode.Text.Trim();
                    try
                    {
                        code = string.Format("{0:000000000000}", Convert.ToInt64(code));
                    }
                    catch (Exception) { }
                    filter.TDL_TREATMENT_CODE__EXACT = code;
                    txtTreatmentCode.Text = code;
                }
                else if (!string.IsNullOrEmpty(txtPatientCode.Text))
                {
                    string patientCode = txtPatientCode.Text.Trim();
                    try
                    {
                        patientCode = string.Format("{0:0000000000}", Convert.ToInt64(patientCode));
                    }
                    catch (Exception) { }
                    filter.TDL_PATIENT_CODE__EXACT = patientCode;
                    txtPatientCode.Text = patientCode;
                }

                filter.KEY_WORD = txtKeyword.Text.Trim();

                if (cboStatus.EditValue != null)
                {
                    StatusADO.Status stt = (StatusADO.Status)cboStatus.EditValue;
                    switch (stt)
                    {
                        case StatusADO.Status.DaTao:
                            filter.IS_HAS_EXP_MEST = true;
                            break;
                        case StatusADO.Status.ChuaTao:
                            filter.IS_HAS_EXP_MEST = false;
                            break;
                        case StatusADO.Status.TatCa:
                        default:
                            break;
                    }
                }

                if (dtIntructionDatefrom.EditValue != null && dtIntructionDatefrom.DateTime != DateTime.MinValue)
                    filter.INTRUCTION_DATE_FROM = Inventec.Common.TypeConvert.Parse.ToInt64(
                        dtIntructionDatefrom.DateTime.ToString("yyyyMMdd") + "000000");

                if (dtIntructionDateTo.EditValue != null && dtIntructionDateTo.DateTime != DateTime.MinValue)
                    filter.INTRUCTION_DATE_TO = Inventec.Common.TypeConvert.Parse.ToInt64(
                        dtIntructionDateTo.DateTime.ToString("yyyyMMdd") + "000000");
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
                    if (!String.IsNullOrEmpty(txtServiceReqCode.Text))
                    {
                        FillDataToGrid();
                    }
                    else
                    {
                        txtTreatmentCode.Focus();
                        txtTreatmentCode.SelectAll();
                    }
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
                    if (!String.IsNullOrEmpty(txtTreatmentCode.Text))
                    {
                        FillDataToGrid();
                    }
                    else
                    {
                        txtPatientCode.Focus();
                        txtPatientCode.SelectAll();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtPatientCode_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (!String.IsNullOrEmpty(txtPatientCode.Text))
                    {
                        FillDataToGrid();
                    }
                    else
                    {
                        txtKeyword.Focus();
                        txtKeyword.SelectAll();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtKeyword_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    FillDataToGrid();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewServiceReqSearch_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != DevExpress.Data.UnboundColumnType.Bound)
                {
                    var data = (V_HIS_SERVICE_REQ_11)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    if (data != null)
                    {
                        if (e.Column.FieldName == "STT")
                        {
                            try
                            {
                                e.Value = e.ListSourceRowIndex + 1 + (((ucPaging1.pagingGrid == null ? 0 : ucPaging1.pagingGrid.CurrentPage) - 1) * (ucPaging1.pagingGrid == null ? 0 : ucPaging1.pagingGrid.PageSize));
                            }
                            catch (Exception ex)
                            {
                                Inventec.Common.Logging.LogSystem.Error(ex);
                            }
                        }
                        else if (e.Column.FieldName == "REQUEST_NAME_STR")
                        {
                            try
                            {
                                e.Value = data.REQUEST_LOGINNAME + (String.IsNullOrEmpty(data.REQUEST_USERNAME) ? "" : " - " + data.REQUEST_USERNAME);
                            }
                            catch (Exception ex)
                            {
                                Inventec.Common.Logging.LogSystem.Error(ex);
                            }
                        }
                        else if (e.Column.FieldName == "INTRUCTION_TIME_STR")
                        {
                            try
                            {
                                e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(data.INTRUCTION_TIME);
                            }
                            catch (Exception ex)
                            {
                                Inventec.Common.Logging.LogSystem.Error(ex);
                            }
                        }
                        else if (e.Column.FieldName == "DOB_STR")
                        {
                            try
                            {
                                if (data.TDL_PATIENT_IS_HAS_NOT_DAY_DOB == (short)1)
                                {
                                    e.Value = data.TDL_PATIENT_DOB.ToString().Substring(0, 4);
                                }
                                else
                                    e.Value = Inventec.Common.DateTime.Convert.TimeNumberToDateString(data.TDL_PATIENT_DOB);
                            }
                            catch (Exception ex)
                            {
                                Inventec.Common.Logging.LogSystem.Error(ex);
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

        private void gridViewSelecteServiceReq_CustomUnboundColumnData(object sender, CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != DevExpress.Data.UnboundColumnType.Bound)
                {
                    var data = (V_HIS_SERVICE_REQ_11)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    if (data != null)
                    {
                        if (e.Column.FieldName == "STT")
                        {
                            try
                            {
                                e.Value = e.ListSourceRowIndex + 1;
                            }
                            catch (Exception ex)
                            {
                                Inventec.Common.Logging.LogSystem.Error(ex);
                            }
                        }
                        else if (e.Column.FieldName == "INTRUCTION_TIME_STR")
                        {
                            try
                            {
                                e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(data.INTRUCTION_TIME);
                            }
                            catch (Exception ex)
                            {
                                Inventec.Common.Logging.LogSystem.Error(ex);
                            }
                        }
                        else if (e.Column.FieldName == "DOB_STR")
                        {
                            try
                            {
                                if (data.TDL_PATIENT_IS_HAS_NOT_DAY_DOB == (short)1)
                                {
                                    e.Value = data.TDL_PATIENT_DOB.ToString().Substring(0, 4);
                                }
                                else
                                    e.Value = Inventec.Common.DateTime.Convert.TimeNumberToDateString(data.TDL_PATIENT_DOB);
                            }
                            catch (Exception ex)
                            {
                                Inventec.Common.Logging.LogSystem.Error(ex);
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

        private void repositoryItemBtnDelete_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                var row = (V_HIS_SERVICE_REQ_11)gridViewSelecteServiceReq.GetFocusedRow();
                if (row != null)
                {
                    ListSelectedData.Remove(row);
                    UpdateDataGridSelect();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void UpdateDataGridSelect()
        {
            try
            {
                gridControlSelecteServiceReq.BeginUpdate();
                gridControlSelecteServiceReq.DataSource = ListSelectedData;
                gridControlSelecteServiceReq.EndUpdate();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewServiceReqSearch_SelectionChanged(object sender, DevExpress.Data.SelectionChangedEventArgs e)
        {
            try
            {
                List<V_HIS_SERVICE_REQ_11> impMestCheckeds = new List<V_HIS_SERVICE_REQ_11>();
                int[] selectRows = gridViewServiceReqSearch.GetSelectedRows();
                if (selectRows != null && selectRows.Count() > 0)
                {
                    for (int i = 0; i < selectRows.Count(); i++)
                    {
                        impMestCheckeds.Add((V_HIS_SERVICE_REQ_11)gridViewServiceReqSearch.GetRow(selectRows[i]));
                    }
                }

                foreach (var item in impMestCheckeds)
                {
                    if (!ListSelectedData.Exists(o => o.ID == item.ID))
                    {
                        ListSelectedData.Add(item);
                    }
                }

                UpdateDataGridSelect();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnChoose_Click(object sender, EventArgs e)
        {
            try
            {
                if (ListSelectedData != null && ListSelectedData.Count > 0)
                {
                    if (ChooseData != null)
                    {
                        ChooseData(ListSelectedData);
                        this.Close();
                    }
                }
                else
                {
                    XtraMessageBox.Show("Bạn chưa chọn đơn nào!");
                    txtServiceReqCode.Focus();
                    txtServiceReqCode.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void barBtnFind_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            btnSearch_Click(null, null);
        }

        private void barBtnChoose_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            btnChoose_Click(null, null);
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            try
            {
                Thread thread = new System.Threading.Thread(new System.Threading.ThreadStart(focus));
                //thread.Priority = ThreadPriority.Highest;
                try
                {
                    thread.Start();
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                    thread.Abort();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        void focus()
        {
            try
            {
                if (this.InvokeRequired)
                {
                    this.Invoke(new MethodInvoker(delegate { dfFO(); }));
                    timer1.Dispose();
                }
                else
                {
                    dfFO();
                    timer1.Dispose();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        void dfFO()
        {
            txtKeyword.Focus();
            txtKeyword.SelectAll();
        }
    }
}
