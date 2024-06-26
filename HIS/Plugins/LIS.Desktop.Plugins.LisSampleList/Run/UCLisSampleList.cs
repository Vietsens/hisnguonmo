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
using Inventec.Common.Controls.EditorLoader;
using LIS.EFMODEL.DataModels;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.ConfigApplication;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using LIS.Filter;
using MOS.EFMODEL.DataModels;
using Inventec.Common.Adapter;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Controls.Session;
using DevExpress.Data;
using System.Collections;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using LIS.Desktop.Plugins.LisSampleList.Config;
using DevExpress.XtraEditors;
using System.IO;
using System.Security.Cryptography;
using LIS.Desktop.Plugins.LisSampleList.Base;
using System.Net;
using Newtonsoft.Json;

namespace LIS.Desktop.Plugins.LisSampleList.Run
{
    public partial class UCLisSampleList : UserControlBase
    {
        private Inventec.Desktop.Common.Modules.Module currentModule;
        private int rowCount = 0;
        private int dataTotal = 0;
        private int startPage = 0;
        private int limit = 0;
        V_HIS_ROOM room = null;
        UnicodeEncoding ByteConverter = new UnicodeEncoding();
        RSACryptoServiceProvider RSA = new RSACryptoServiceProvider();
        byte[] plaintext;
        byte[] encryptedtext;


        public UCLisSampleList(Inventec.Desktop.Common.Modules.Module currentModule)
            : base(currentModule)
        {
            InitializeComponent();
            // TODO: Complete member initialization
            this.currentModule = currentModule;
        }

        private void UCLisSampleList_Load(object sender, EventArgs e)
        {
            try
            {
                //SetCaptionByLanguageKey();
                HisConfigCFG.LoadConfig();
                LoadDataComboFilter();
                ResetControl();
                FillDataToGridControl();
                this.txtBarcode.Focus();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadDataComboFilter()
        {
            try
            {
                InitDataCboLisSampleStt();
                InitDataCboLisSampleResult();
                InitDataCboSignStt();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void InitDataCboLisSampleStt()
        {
            try
            {
                List<ADO.ComboADO> status = new List<ADO.ComboADO>();
                status.Add(new ADO.ComboADO(999, "Chưa trả kết quả"));
                status.Add(new ADO.ComboADO(0, "Tất cả"));
                status.Add(new ADO.ComboADO(IMSys.DbConfig.LIS_RS.LIS_SAMPLE_STT.ID__CHUA_LM, "Chưa lấy mẫu"));
                status.Add(new ADO.ComboADO(IMSys.DbConfig.LIS_RS.LIS_SAMPLE_STT.ID__DA_LM, "Đã lấy mẫu"));
                status.Add(new ADO.ComboADO(IMSys.DbConfig.LIS_RS.LIS_SAMPLE_STT.ID__CO_KQ, "Có kết quả và đã duyệt"));
                status.Add(new ADO.ComboADO(998, "Có kết quả và chưa duyệt"));
                status.Add(new ADO.ComboADO(IMSys.DbConfig.LIS_RS.LIS_SAMPLE_STT.ID__TRA_KQ, "Trả kết quả"));
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("statusName", "Trạng thái", 50, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("statusName", "id", columnInfos, true, 50);
                controlEditorADO.ImmediatePopup = true;
                ControlEditorLoader.Load(cboLisSampleStt, status, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void InitDataCboLisSampleResult()
        {
            try
            {
                List<LIS_SERVICE_RESULT> lstServiceResult = BackendDataWorker.Get<LIS_SERVICE_RESULT>();
                if (lstServiceResult == null) lstServiceResult = new List<LIS_SERVICE_RESULT>();
                lstServiceResult = lstServiceResult.Where(o => o.IS_ACTIVE == (short)1).ToList();
                LIS_SERVICE_RESULT ss = new LIS_SERVICE_RESULT();
                ss.ID = 0;
                ss.SERVICE_RESULT_CODE = "00";
                ss.SERVICE_RESULT_NAME = "Tất cả";
                lstServiceResult.Insert(0, ss);
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("SERVICE_RESULT_NAME", "", 100, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("SERVICE_RESULT_NAME", "ID", columnInfos, false, 150);
                controlEditorADO.ImmediatePopup = true;
                ControlEditorLoader.Load(cboServiceResult, lstServiceResult, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void InitDataCboSignStt()
        {
            try
            {
                List<ADO.ComboADO> status = new List<ADO.ComboADO>();
                status.Add(new ADO.ComboADO(0, "Tất cả"));
                status.Add(new ADO.ComboADO(1, "Hoàn thành ký"));
                status.Add(new ADO.ComboADO(2, "Chưa hoàn thành ký"));
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("statusName", "Trạng thái", 100, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("statusName", "id", columnInfos, true, 100);
                controlEditorADO.ImmediatePopup = true;
                ControlEditorLoader.Load(cboSignStt, status, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ResetControl()
        {
            try
            {
                this.txtBarcode.Text = null;
                this.txtKeyWord.Text = null;
                this.txtPatientCode.Text = null;
                this.txtServiceReqCode.Text = null;
                this.txtTreatmentCode.Text = null;
                this.dtResultApprovalTimeFrom.EditValue = null;
                this.dtResultApprovalTimeTo.EditValue = null;
                this.dtResultTimeFrom.EditValue = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0, 0, 0);
                this.dtResultTimeTo.EditValue = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 23, 59, 59);
                this.cboLisSampleStt.EditValue = IMSys.DbConfig.LIS_RS.LIS_SAMPLE_STT.ID__TRA_KQ;//Trả kết quả
                this.cboServiceResult.EditValue = 0;//tất cả
                this.cboSignStt.EditValue = 0;//tất cả
                this.chkOnce.Checked = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void FillDataToGridControl()
        {
            try
            {
                int pageSize = 0;
                if (ucPaging1.pagingGrid != null)
                {
                    pageSize = ucPaging1.pagingGrid.PageSize;
                }
                else
                {
                    pageSize = (int)ConfigApplications.NumPageSize;
                }

                FillDataToGridSample(new CommonParam(0, pageSize));

                CommonParam param = new CommonParam();
                param.Limit = rowCount;
                param.Count = dataTotal;
                ucPaging1.Init(FillDataToGridSample, param, pageSize, gridControlLisSample);
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

                LisSampleView1Filter lisSampleFilter = new LisSampleView1Filter();
                if (room == null)
                {
                    room = BackendDataWorker.Get<V_HIS_ROOM>().FirstOrDefault(o => o.ID == currentModule.RoomId);
                }
                if (room != null)
                {
                    lisSampleFilter.EXECUTE_ROOM_CODE__EXACT = room.ROOM_CODE;
                }

                if (!String.IsNullOrEmpty(this.txtServiceReqCode.Text))
                {
                    string code = this.txtServiceReqCode.Text.Trim();
                    if (code.Length < 12)
                    {
                        long num = 0;
                        if (long.TryParse(code, out num))
                        {
                            code = string.Format("{0:000000000000}", num);
                        }
                    }
                    lisSampleFilter.SERVICE_REQ_CODE__EXACT = code;
                    this.txtServiceReqCode.Text = code;
                }
                else if (!String.IsNullOrWhiteSpace(this.txtBarcode.Text))
                {
                    lisSampleFilter.BARCODE__EXACT = this.txtBarcode.Text.Trim();
                }
                else if (!String.IsNullOrWhiteSpace(this.txtTreatmentCode.Text))
                {
                    string code = this.txtTreatmentCode.Text.Trim();
                    if (code.Length < 12)
                    {
                        long num = 0;
                        if (long.TryParse(code, out num))
                        {
                            code = string.Format("{0:000000000000}", num);
                        }
                    }
                    lisSampleFilter.TREATMENT_CODE__EXACT = code;
                    this.txtTreatmentCode.Text = code;
                }
                else
                {
                    if (!String.IsNullOrWhiteSpace(this.txtPatientCode.Text))
                    {
                        string code = this.txtPatientCode.Text.Trim();
                        if (code.Length < 10)
                        {
                            long num = 0;
                            if (long.TryParse(code, out num))
                            {
                                code = string.Format("{0:0000000000}", num);
                            }
                        }
                        lisSampleFilter.PATIENT_CODE__EXACT = code;
                        this.txtPatientCode.Text = code;
                    }

                    if (!String.IsNullOrEmpty(this.txtKeyWord.Text))
                    {
                        lisSampleFilter.PATIENT_NAME = this.txtKeyWord.Text.Trim();
                    }

                    if (dtResultApprovalTimeFrom != null && dtResultApprovalTimeFrom.DateTime != DateTime.MinValue)
                        lisSampleFilter.RESULT_APPROVAL_TIME_FROM = Inventec.Common.TypeConvert.Parse.ToInt64(dtResultApprovalTimeFrom.DateTime.ToString("yyyyMMddHHmm") + "00");
                    if (dtResultApprovalTimeTo != null && dtResultApprovalTimeTo.DateTime != DateTime.MinValue)
                        lisSampleFilter.RESULT_APPROVAL_TIME_TO = Inventec.Common.TypeConvert.Parse.ToInt64(dtResultApprovalTimeTo.DateTime.ToString("yyyyMMddHHmm") + "59");

                    if (dtResultTimeFrom != null && dtResultTimeFrom.DateTime != DateTime.MinValue)
                        lisSampleFilter.RESULT_TIME_FROM = Inventec.Common.TypeConvert.Parse.ToInt64(dtResultTimeFrom.DateTime.ToString("yyyyMMddHHmm") + "00");
                    if (dtResultTimeTo != null && dtResultTimeTo.DateTime != DateTime.MinValue)
                        lisSampleFilter.RESULT_TIME_TO = Inventec.Common.TypeConvert.Parse.ToInt64(dtResultTimeTo.DateTime.ToString("yyyyMMddHHmm") + "59");

                    lisSampleFilter.ORDER_FIELD = "BARCODE_TIME";
                    lisSampleFilter.ORDER_DIRECTION = "DESC";

                    if (chkGroup.Checked)
                    {
                        lisSampleFilter.IS_AGGREGATION = true;
                    }
                    else if (chkOnce.Checked)
                    {
                        lisSampleFilter.IS_AGGREGATION = false;
                    }

                    if (cboServiceResult.EditValue != null)
                    {
                        long id = Inventec.Common.TypeConvert.Parse.ToInt64(cboServiceResult.EditValue.ToString());
                        if (id != 0)
                            lisSampleFilter.SERVICE_RESULT_ID = id;
                    }

                    if (cboSignStt.EditValue != null)
                    {
                        long id = Inventec.Common.TypeConvert.Parse.ToInt64(cboSignStt.EditValue.ToString());
                        if (id == 1)//Hoàn thành ký
                        {
                            lisSampleFilter.IS_SIGNING_RESULT_FINISHED = true;
                        }
                        else if (id == 2)//Chưa Hoàn thành ký
                        {
                            lisSampleFilter.IS_SIGNING_RESULT_FINISHED = false;
                        }
                    }

                    //Tất cả 0
                    //Chưa lấy mẫu 2
                    //Đã lấy mẫu 3
                    //Đã có kết quả
                    //Đã trả kết quả
                    //Filter yeu cau chua lấy mẫu
                    if (cboLisSampleStt.EditValue != null)
                    {
                        long id = Inventec.Common.TypeConvert.Parse.ToInt64(cboLisSampleStt.EditValue.ToString());
                        //Chưa lấy mẫu
                        if (id == 1)
                        {
                            lisSampleFilter.SAMPLE_STT_IDs = new List<long>()
                            {
                                IMSys.DbConfig.LIS_RS.LIS_SAMPLE_STT.ID__CHUA_LM,
                                IMSys.DbConfig.LIS_RS.LIS_SAMPLE_STT.ID__TU_CHOI
                            };
                        }
                        //Đã lấy mẫu
                        else if (id == 2)
                        {
                            lisSampleFilter.SAMPLE_STT_IDs = new List<long>()
                            {
                                IMSys.DbConfig.LIS_RS.LIS_SAMPLE_STT.ID__DA_LM,
                                IMSys.DbConfig.LIS_RS.LIS_SAMPLE_STT.ID__CHAP_NHAN
                            };
                        }
                        //Có kết quả và đã duyệt
                        else if (id == 3)
                        {
                            lisSampleFilter.SAMPLE_STT_IDs = new List<long>()
                            {
                                IMSys.DbConfig.LIS_RS.LIS_SAMPLE_STT.ID__CO_KQ,
                                IMSys.DbConfig.LIS_RS.LIS_SAMPLE_STT.ID__DUYET_KQ
                            };
                        }//đã trả kết quả
                        else if (id == 4)
                        {
                            lisSampleFilter.SAMPLE_STT_ID = IMSys.DbConfig.LIS_RS.LIS_SAMPLE_STT.ID__TRA_KQ;
                        }
                        else if (id == 999) //Chưa trả kq
                        {
                            List<long> sampleSttIds = new List<long>();
                            sampleSttIds.Add(IMSys.DbConfig.LIS_RS.LIS_SAMPLE_STT.ID__CHUA_LM);//chưa lay mau
                            sampleSttIds.Add(IMSys.DbConfig.LIS_RS.LIS_SAMPLE_STT.ID__DA_LM);
                            sampleSttIds.Add(IMSys.DbConfig.LIS_RS.LIS_SAMPLE_STT.ID__CO_KQ);
                            sampleSttIds.Add(IMSys.DbConfig.LIS_RS.LIS_SAMPLE_STT.ID__TU_CHOI);
                            sampleSttIds.Add(IMSys.DbConfig.LIS_RS.LIS_SAMPLE_STT.ID__CHAP_NHAN);
                            sampleSttIds.Add(IMSys.DbConfig.LIS_RS.LIS_SAMPLE_STT.ID__DUYET_KQ);
                            lisSampleFilter.SAMPLE_STT_IDs = sampleSttIds;
                        }//Có kết quả và chưa duyệt
                        else if (id == 998)
                        {
                            lisSampleFilter.SAMPLE_STT_ID = IMSys.DbConfig.LIS_RS.LIS_SAMPLE_STT.ID__CO_KQ;
                        }
                        //Tất cả
                        else
                        {
                            lisSampleFilter.SAMPLE_STT_ID = null;
                        }
                    }

                    lisSampleFilter.IS_ANTIBIOTIC_RESISTANCE = false;
                }

                var apiResult = new BackendAdapter(paramCommon).GetRO<List<V_LIS_SAMPLE_1>>("api/LisSample/GetView1", ApiConsumers.LisConsumer, lisSampleFilter, paramCommon);
                gridControlLisSample.DataSource = null;
                if (apiResult != null)
                {
                    WaitingManager.Hide();
                    var data = (List<V_LIS_SAMPLE_1>)apiResult.Data;
                    if (data != null)
                    {
                        gridControlLisSample.DataSource = data;
                        rowCount = (data == null ? 0 : data.Count);
                        dataTotal = (apiResult.Param == null ? 0 : apiResult.Param.Count ?? 0);
                    }
                }

                #region Process has exception
                SessionManager.ProcessTokenLost((CommonParam)param);
                #endregion
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                WaitingManager.Hide();
            }
        }

        private void gridViewLisSample_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)
                {
                    V_LIS_SAMPLE_1 data = (V_LIS_SAMPLE_1)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    if (data != null)
                    {
                        if (e.Column.FieldName == "STT")
                        {
                            e.Value = e.ListSourceRowIndex + 1 + startPage;
                        }
                        else if (e.Column.FieldName == gc_PatientName.FieldName)
                        {
                            e.Value = data.LAST_NAME + " " + data.FIRST_NAME;
                        }
                        else if (e.Column.FieldName == gc_PatientBob.FieldName)
                        {
                            if (data.DOB.HasValue)
                            {
                                if (data.IS_HAS_NOT_DAY_DOB == 1)
                                {
                                    e.Value = data.DOB.ToString().Substring(0, 4);
                                }
                                else
                                    e.Value = Inventec.Common.DateTime.Convert.TimeNumberToDateString(data.DOB ?? 0);
                            }
                            else
                                e.Value = "";
                        }
                        else if (e.Column.FieldName == gc_SampleTime.FieldName)
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(data.SAMPLE_TIME ?? 0);
                        }
                        else if (e.Column.FieldName == gc_ResultApprovalTime.FieldName)
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeStringWithoutSecond(data.RESULT_APPROVAL_TIME ?? 0);
                        }
                        else if (e.Column.FieldName == gc_CreateTime.FieldName)
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(data.CREATE_TIME ?? 0);
                        }
                        else if (e.Column.FieldName == gc_ModifyTime.FieldName)
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(data.MODIFY_TIME ?? 0);
                        }
                        else if (e.Column.FieldName == gc_SampleSender.FieldName)
                        {
                            List<string> sampleSender = new List<string>();
                            if (!String.IsNullOrWhiteSpace(data.SAMPLE_SENDER_CODE))
                            {
                                sampleSender.Add(data.SAMPLE_SENDER_CODE);
                            }

                            if (!String.IsNullOrWhiteSpace(data.SAMPLE_SENDER))
                            {
                                sampleSender.Add(data.SAMPLE_SENDER);
                            }

                            e.Value = string.Join(" - ", sampleSender);
                        }
                        else if (e.Column.FieldName == gc_GenderName.FieldName)
                        {
                            if (!String.IsNullOrWhiteSpace(data.GENDER_NAME))
                            {
                                e.Value = data.GENDER_NAME;
                            }
                            else if (!String.IsNullOrWhiteSpace(data.GENDER_CODE))
                            {
                                e.Value = data.GENDER_CODE == "01" ? "Nữ" : "Nam";
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewLisSample_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                btnExportExcel.Enabled = false;
                GridView grd = sender as GridView;
                if (grd != null)
                {
                    int[] selectRow = grd.GetSelectedRows();
                    if (selectRow.Length > 0)
                    {
                        btnExportExcel.Enabled = true;
                        btnSyncResult.Enabled = true;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewLisSample_MouseDown(object sender, MouseEventArgs e)
        {
            try
            {
                DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                if (view != null)
                {
                    GridHitInfo info = view.CalcHitInfo(e.Location);
                    if (info != null && info.Column != null && info.HitTest == GridHitTest.RowCell)
                    {
                        if (info.Column.FieldName == gc_Print.FieldName)
                        {
                            V_LIS_SAMPLE_1 row = (V_LIS_SAMPLE_1)view.GetRow(info.RowHandle);
                            ProcessPrintRow(row);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
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

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            try
            {
                ResetControl();
                FillDataToGridControl();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnExportExcel_Click(object sender, EventArgs e)
        {
            try
            {
                if (!btnExportExcel.Enabled)
                {
                    return;
                }

                List<V_LIS_SAMPLE_1> listSelected = new List<V_LIS_SAMPLE_1>();
                int[] selectRow = gridViewLisSample.GetSelectedRows();
                foreach (var item in selectRow)
                {
                    var selected = (V_LIS_SAMPLE_1)gridViewLisSample.GetRow(item);
                    if (selected != null)
                    {
                        listSelected.Add(selected);
                    }
                }

                if (listSelected != null && listSelected.Count > 0)
                {
                    ProcessExportExcel(listSelected);
                }
                else
                {
                    MessageBox.Show("Bạn chưa chọn mẫu");
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public void SearchData()
        {
            btnSearch_Click(null, null);
        }

        public void RefreshData()
        {
            btnRefresh_Click(null, null);
        }

        private void txtBarcode_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (!String.IsNullOrWhiteSpace(this.txtBarcode.Text))
                    {
                        FillDataToGridControl();
                    }
                    else
                    {
                        SendKeys.Send("{TAB}");
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
                    if (!String.IsNullOrWhiteSpace(this.txtPatientCode.Text))
                    {
                        FillDataToGridControl();
                    }
                    else
                    {
                        SendKeys.Send("{TAB}");
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
                    if (!String.IsNullOrWhiteSpace(this.txtServiceReqCode.Text))
                    {
                        FillDataToGridControl();
                    }
                    else
                    {
                        SendKeys.Send("{TAB}");
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
                    if (!String.IsNullOrWhiteSpace(this.txtTreatmentCode.Text))
                    {
                        FillDataToGridControl();
                    }
                    else
                    {
                        SendKeys.Send("{TAB}");
                    }
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
                    if (!String.IsNullOrWhiteSpace(this.txtKeyWord.Text))
                    {
                        FillDataToGridControl();
                    }
                    else
                    {
                        SendKeys.Send("{TAB}");
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnSyncResult_Click(object sender, EventArgs e)
        {
            string saveFileName = Path.GetTempFileName();
            saveFileName = saveFileName.Replace("tmp", "xlsx");
            try
            {
                if (!string.IsNullOrEmpty(HisConfigCFG.SyncResult) && HisConfigCFG.SyncResult.Contains("|"))
                {
                    string[] inforData = HisConfigCFG.SyncResult.Split('|');
                    if (inforData.Length == 4)
                    {
                        var requestURL = inforData[0];
                        var account = inforData[1];
                        var password = inforData[2];
                        var publicKey = inforData[3];

                        List<V_LIS_SAMPLE_1> listSelected = new List<V_LIS_SAMPLE_1>();
                        int[] selectRow = gridViewLisSample.GetSelectedRows();
                        foreach (var item in selectRow)
                        {
                            var selected = (V_LIS_SAMPLE_1)gridViewLisSample.GetRow(item);
                            if (selected != null)
                            {
                                listSelected.Add(selected);
                            }
                        }
                        if (listSelected != null && listSelected.Count > 0)
                        {
                            #region ProcessExcel

                            string fileName = System.IO.Path.Combine(Application.StartupPath + "\\Tmp\\Exp\\", "KetQuaXetNghiem.xlsx");
                            if (!File.Exists(fileName))
                            {
                                XtraMessageBox.Show("Không tìm thấy file template theo đường dẫn " + fileName);
                                return;
                            }

                            WaitingManager.Show();
                            bool success = false;
                            Inventec.Common.FlexCellExport.ProcessSingleTag singleTag = new Inventec.Common.FlexCellExport.ProcessSingleTag();
                            Inventec.Common.FlexCellExport.Store store = new Inventec.Common.FlexCellExport.Store();
                            Inventec.Common.FlexCellExport.ProcessObjectTag objectTag = new Inventec.Common.FlexCellExport.ProcessObjectTag();

                            store.ReadTemplate(System.IO.Path.GetFullPath(fileName));
                            store.SetCommonFunctions();

                            objectTag.AddObjectData(store, "ExportResult", listSelected);

                            success = store.OutFile(saveFileName);
                            MessageManager.Show(this.ParentForm, null, success);
                            if (!success)
                                return;
                            #endregion
                            //saveFileName = "C:\\Users\\Admin\\Downloads\\File trả kết quả 19.10.xlsx";
                            FileStream fs = new FileStream(saveFileName, FileMode.Open, FileAccess.Read);
                            byte[] data = new byte[fs.Length];
                            fs.Read(data, 0, data.Length);
                            fs.Close();
                            Dictionary<string, object> postParameters = new Dictionary<string, object>();
                            // Add your parameters here  
                            //"application/octet-stream"
                            //"application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"
                            postParameters.Add("file", new FormUpload.FileParameter(data, Path.GetFileName(saveFileName), "application/octet-stream"));
                            string userAgent = "someone";
                            string headerkey = "Token";
                            //string headerkey = "Authorization";
                            ADO.LisSampleInput ado = new ADO.LisSampleInput();
                            ado.account = account;
                            ado.password = password;
                            string textJson = JsonConvert.SerializeObject(ado);
                            Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => inforData), inforData));
                            Inventec.Common.Logging.LogSystem.Debug("_____textJson : " + textJson);

                            string headervalue = Base.Utils.Encrypt(textJson, publicKey);
                            Inventec.Common.Logging.LogSystem.Info("_____headervalue : " + headervalue);
                            HttpWebResponse webResponse = FormUpload.MultipartFormPost(requestURL, userAgent, postParameters, headerkey, headervalue);
                            // Process response  
                            Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => webResponse), webResponse));
                            StreamReader responseReader = new StreamReader(webResponse.GetResponseStream());
                            string res = responseReader.ReadToEnd();
                            webResponse.Close();
                            var resultJSON = JsonConvert.DeserializeObject<ADO.LisSampleResult>(res);
                            WaitingManager.Hide();
                            Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => res), res));
                           
                            if (resultJSON.code == "200")
                            {
                                string link = !string.IsNullOrEmpty(resultJSON.linkSignPage) ? "Đường dẫn: " + resultJSON.linkSignPage : "";
                                if (string.IsNullOrEmpty(link))
                                {
                                    XtraMessageBox.Show( resultJSON.message,"Mã " + resultJSON.code);
                                    return;
                                }
                                else
                                {
                                    var resultDialog = XtraMessageBox.Show( resultJSON.message + "\n" + link + "\n" + "Bạn có muốn truy cập đường dẫn trên không?","Mã " + resultJSON.code, MessageBoxButtons.YesNo,MessageBoxIcon.None,MessageBoxDefaultButton.Button1);
                                    if (resultDialog == DialogResult.Yes)
                                    {
                                        System.Diagnostics.Process.Start(resultJSON.linkSignPage);
                                    }                                   
                                }
                            }
                            else
                            {
                                XtraMessageBox.Show("Mã: " + resultJSON.code + ", " + resultJSON.message,"Thông báo");
                                return;
                            }

                        }
                    }
                    else
                    {
                        XtraMessageBox.Show("Cấu hình hệ thống LIS.Desktop.Plugins.LisSampleList.ConnectionInfo khai báo không chính xác. vui lòng kiểm tra lại");
                        return;
                    }
                }
                else
                {
                    XtraMessageBox.Show("Cấu hình hệ thống LIS.Desktop.Plugins.LisSampleList.ConnectionInfo khai báo không chính xác. vui lòng kiểm tra lại");
                    return;
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            finally
            {
                if (File.Exists(saveFileName))
                    File.Delete(saveFileName);
            }
        }     
        
    }
}
