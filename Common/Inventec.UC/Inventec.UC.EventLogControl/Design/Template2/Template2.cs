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
using Inventec.UC.EventLogControl.Base;
using System.Collections;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraPrinting;
using DevExpress.Utils.Menu;
using Inventec.Desktop.Common.Message;
using Inventec.Core;
using HIS.Desktop.Utility;

namespace Inventec.UC.EventLogControl.Design.Template2
{
    internal partial class Template2 : UserControl, IFormCallBack
    {
        private List<SDA.EFMODEL.DataModels.SDA_EVENT_LOG> ListEventLog = new List<SDA.EFMODEL.DataModels.SDA_EVENT_LOG>();
        private ProcessHasException _HasException;
        Data.DataInit currentData;
        string uriSdaEventLogGet = "api/SdaEventLog/Get";

        //internal string typeCodeFind = "Từ khóa tìm kiếm";//Set lại giá trị trong resource
        internal string typeCodeFind__KeyWork = "Từ khóa tìm kiếm";//Set lại giá trị trong resource
        internal string typeCodeFind = "Từ khóa tìm kiếm";//Set lại giá trị trong resource
        internal string typeCodeFind__MaBN = "Mã Bệnh nhân";//Set lại giá trị trong resource
        internal string typeCodeFind__MaDT = "Mã Điều trị";//Set lại giá trị trong resource
        internal string typeCodeFind__MaPX = "Mã phiếu xuất";//Set lại giá trị trong resource
        internal string typeCodeFind__MaPN = "Mã phiếu nhập";//Set lại giá trị trong resource
        internal string typeCodeFind__MaYL = "Mã y lệnh";//Set lại giá trị trong resource
        internal string typeCodeFind__SoQDT = "Số quyết định thầu";//Set lại giá trị trong resource

        internal string typeCodeFind__KeyWork_InDate = "Trong ngày";//Set lại giá trị trong resource
        internal string typeCodeFind_InDate = "Trong ngày";//Set lại giá trị trong resource
        internal string typeCodeFind__InMonth = "Trong tháng";//Set lại giá trị trong resource
        internal string typeCodeFind__InTime = "Khoảng ngày";//Set lại giá trị trong resource

        int rowCount = 0;
        int dataTotal = 0;
        int startPage = 0;


        public Template2(Data.DataInit data)
        {
            InitializeComponent();
            try
            {
                ApiConsumerStore.SdaConsumer = data.sdaComsumer;
                ApiConsumerStore.UriElasticSearchServer = data.UriElasticSearchServer;
                GlobalStore.NumPageSize = data.pageNum <= 0 ? 100 : data.pageNum;
                this.currentData = data;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void FillDataToControl()
        {
            try
            {
                WaitingManager.Show();
                int pagingSize = 0;
                if (ucPaging.pagingGrid != null)
                {
                    pagingSize = ucPaging.pagingGrid.PageSize;
                }
                else
                {
                    pagingSize = (int)GlobalStore.NumPageSize;
                }

                GridPaging(new CommonParam(0, pagingSize));
                CommonParam param = new CommonParam();
                param.Limit = rowCount;
                param.Count = dataTotal;
                ucPaging.Init(GridPaging, param, pagingSize, this.gridControlEventLog);
                gridControlEventLog.RefreshDataSource();
                txtKeyWord.Focus();
                txtKeyWord.SelectAll();
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                WaitingManager.Hide();
            }
        }

        private void GridPaging(object param)
        {
            try
            {
                startPage = ((CommonParam)param).Start ?? 0;
                int limit = ((CommonParam)param).Limit ?? 0;
                CommonParam paramCommon = new CommonParam(startPage, limit);
                ApiResultObject<List<SDA.EFMODEL.DataModels.SDA_EVENT_LOG>> apiResult = null;
                SDA.Filter.SdaEventLogFilter filter = new SDA.Filter.SdaEventLogFilter();
                if (dtTime.EditValue == null || dtTime.EditValue.ToString() == "")
                {
                    WaitingManager.Hide();
                    if (DevExpress.XtraEditors.XtraMessageBox.Show(Resources.ResourceMessage.Ngaythangkhongduocbotrong, Resources.ResourceMessage.ThongBao, System.Windows.Forms.MessageBoxButtons.OK) == System.Windows.Forms.DialogResult.OK)
                        return;
                }
                SetFilter(ref filter);
                gridViewEventLog.BeginUpdate();

                if (currentData != null && !String.IsNullOrEmpty(ApiConsumerStore.UriElasticSearchServer))
                {
                    Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData("BEGIN CALL elasticsearch with index:" + ApiConsumerStore.EventLogIndex + "____UriElasticSearchServer:" + ApiConsumerStore.UriElasticSearchServer + " ____filter:", filter));

                    ElasticSearchGet elasticSearchGet = new ElasticSearchGet();
                    apiResult = elasticSearchGet.GetEventLog(filter, startPage, limit);
                }
                else
                {
                    Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData("BEGIN CALL " + uriSdaEventLogGet + " api filter:", filter));

                    apiResult = new Inventec.Common.Adapter.BackendAdapter
                        (paramCommon).GetRO<List<SDA.EFMODEL.DataModels.SDA_EVENT_LOG>>
                        (uriSdaEventLogGet, ApiConsumerStore.SdaConsumer, filter, paramCommon);
                    Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => apiResult), apiResult));
                }

                Inventec.Common.Logging.LogSystem.Debug("END CALL api with filter");
                if (apiResult != null)
                {
                    var listExpMest = apiResult.Data;
                    if (listExpMest != null && listExpMest.Count > 0)
                    {
                        gridControlEventLog.DataSource = listExpMest;
                        rowCount = (listExpMest == null ? 0 : listExpMest.Count);
                        dataTotal = (apiResult.Param == null ? 0 : apiResult.Param.Count ?? 0);
                    }
                    else
                    {
                        gridControlEventLog.DataSource = null;
                        rowCount = (listExpMest == null ? 0 : listExpMest.Count);
                        dataTotal = (apiResult.Param == null ? 0 : apiResult.Param.Count ?? 0);
                    }
                }
                gridViewEventLog.EndUpdate();

                #region Process has exception
                //SessionManager.ProcessTokenLost(paramCommon);
                #endregion
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetFilter(ref SDA.Filter.SdaEventLogFilter filter)
        {
            try
            {
                filter.ORDER_FIELD = "CREATE_TIME";
                filter.ORDER_DIRECTION = "DESC";
                Inventec.Common.Logging.LogSystem.Debug("txtKeyWord.Text: " + txtKeyWord.Text);
                //if (dtTime.EditValue != null && dtTime.DateTime != DateTime.MinValue)
                //{
                //    filter.CREATE_DATE_TO = Inventec.Common.TypeConvert.Parse.ToInt64(Convert.ToDateTime((dtTime.EditValue ?? "").ToString()).ToString("yyyyMMdd") + "235959");
                //}

                if (this.typeCodeFind == typeCodeFind__KeyWork || txtKeyWord.Text == null || txtKeyWord.Text == "")
                {
                    filter.KEY_WORD = txtKeyWord.Text.Trim();
                }
                else if (this.typeCodeFind == typeCodeFind__MaBN)
                {
                    string key = "";
                    if (!String.IsNullOrWhiteSpace(txtKeyWord.Text) && txtKeyWord.Text.Contains("PATIENT_CODE"))
                    {
                        key = txtKeyWord.Text.Trim();
                    }
                    else
                    {
                        key = string.Format("{0:0000000000}", Convert.ToInt64(txtKeyWord.Text.Trim()));
                        txtKeyWord.Text = "PATIENT_CODE:" + " " + key;
                    }

                    filter.DESCRIPTION = txtKeyWord.Text;
                }
                else if (this.typeCodeFind == typeCodeFind__MaDT)
                {
                    string key = "";
                    if (!String.IsNullOrWhiteSpace(txtKeyWord.Text) && txtKeyWord.Text.Contains("TREATMENT_CODE"))
                    {
                        key = txtKeyWord.Text.Trim();
                    }
                    else
                    {
                        key = string.Format("{0:000000000000}", Convert.ToInt64(txtKeyWord.Text.Trim()));
                        txtKeyWord.Text = "TREATMENT_CODE:" + " " + key;
                    }
                    filter.KEY_WORD = txtKeyWord.Text;
                }
                else if (this.typeCodeFind == typeCodeFind__MaPN)
                {
                    string key = "";
                    if (!String.IsNullOrWhiteSpace(txtKeyWord.Text) && txtKeyWord.Text.Contains("IMP_MEST_CODE"))
                    {
                        key = txtKeyWord.Text.Trim();
                    }
                    else
                    {
                        key = string.Format("{0:000000000000}", Convert.ToInt64(txtKeyWord.Text.Trim()));
                        txtKeyWord.Text = "IMP_MEST_CODE:" + " " + key;
                    }

                    filter.KEY_WORD = txtKeyWord.Text;
                }
                else if (this.typeCodeFind == typeCodeFind__MaPX)
                {
                    string key = "";
                    if (!String.IsNullOrWhiteSpace(txtKeyWord.Text) && txtKeyWord.Text.Contains("EXP_MEST_CODE"))
                    {
                        key = txtKeyWord.Text.Trim();
                    }
                    else
                    {
                        key = string.Format("{0:000000000000}", Convert.ToInt64(txtKeyWord.Text.Trim()));
                        txtKeyWord.Text = "EXP_MEST_CODE:" + " " + key;
                    }

                    filter.KEY_WORD = txtKeyWord.Text;
                }
                else if (this.typeCodeFind == typeCodeFind__MaYL)
                {
                    string key = "";
                    if (!String.IsNullOrWhiteSpace(txtKeyWord.Text) && txtKeyWord.Text.Contains("SERVICE_REQ_CODE"))
                    {
                        key = txtKeyWord.Text.Trim();
                    }
                    else
                    {
                        key = string.Format("{0:000000000000}", Convert.ToInt64(txtKeyWord.Text.Trim()));
                        txtKeyWord.Text = "SERVICE_REQ_CODE:" + " " + key;
                    }

                    filter.KEY_WORD = txtKeyWord.Text;
                }
                else if (this.typeCodeFind == typeCodeFind__SoQDT)
                {
                    string key = "";
                    if (!String.IsNullOrWhiteSpace(txtKeyWord.Text) && txtKeyWord.Text.Contains("BID_NUMBER"))
                    {
                        key = txtKeyWord.Text.Trim();
                    }
                    else
                    {
                        key = txtKeyWord.Text.Trim();
                        txtKeyWord.Text = "BID_NUMBER:" + " " + key;
                    }

                    filter.KEY_WORD = txtKeyWord.Text;
                }

                if (this.typeCodeFind__KeyWork_InDate == this.typeCodeFind_InDate
                    && dtTime.EditValue != null && dtTime.DateTime != DateTime.MinValue)
                {
                    filter.EVENT_DATE = Inventec.Common.TypeConvert.Parse.ToInt64(
                    Convert.ToDateTime(dtTime.EditValue).ToString("yyyyMMdd") + "000000");
                }
                else if (this.typeCodeFind__KeyWork_InDate == typeCodeFind__InMonth
                    && dtTime.EditValue != null && dtTime.DateTime != DateTime.MinValue)
                {
                    filter.EVENT_MONTH = Inventec.Common.TypeConvert.Parse.ToInt64(
                    Convert.ToDateTime(dtTime.EditValue).ToString("yyyyMM") + "00000000");
                }
                else if (this.typeCodeFind__KeyWork_InDate == typeCodeFind__InTime
                    && dtTime.EditValue != null && dtTime.DateTime != DateTime.MinValue
                    && dtDateCome.EditValue != null && dtDateCome.DateTime != DateTime.MinValue)
                {
                    filter.EVENT_DATE_FROM = Inventec.Common.TypeConvert.Parse.ToInt64(
                    Convert.ToDateTime(dtTime.EditValue).ToString("yyyyMMdd") + "000000");
                    filter.EVENT_DATE_TO = Inventec.Common.TypeConvert.Parse.ToInt64(
                    Convert.ToDateTime(dtDateCome.EditValue).ToString("yyyyMMdd") + "000000");
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void Template2_Load(object sender, EventArgs e)
        {
            this.typeCodeFind = typeCodeFind__KeyWork;
            if (!String.IsNullOrEmpty(this.currentData.expMestCode))
            {
                btnCodeFind.Text = typeCodeFind__MaPX;
                //this.typeCodeFind = typeCodeFind__MaPX;
                txtKeyWord.Text = this.currentData.expMestCode;
            }
            else if (!String.IsNullOrEmpty(this.currentData.impMestCode))
            {
                btnCodeFind.Text = typeCodeFind__MaPN;
                //this.typeCodeFind = typeCodeFind__MaPN;
                txtKeyWord.Text = this.currentData.impMestCode;
            }
            else if (!String.IsNullOrEmpty(this.currentData.patientCode))
            {
                btnCodeFind.Text = typeCodeFind__MaBN;
                //this.typeCodeFind = typeCodeFind__MaBN;
                txtKeyWord.Text = this.currentData.patientCode;
            }
            else if (!String.IsNullOrEmpty(this.currentData.treatmentCode))
            {
                btnCodeFind.Text = typeCodeFind__MaDT;
                // this.typeCodeFind = typeCodeFind__MaDT;
                txtKeyWord.Text = this.currentData.treatmentCode;
            }
            else if (!String.IsNullOrEmpty(this.currentData.serviceRequestCode))
            {
                btnCodeFind.Text = typeCodeFind__MaYL;
                // this.typeCodeFind = typeCodeFind__MaYL;
                txtKeyWord.Text = this.currentData.serviceRequestCode;
            }
            else if (!String.IsNullOrEmpty(this.currentData.bidNumber))
            {
                btnCodeFind.Text = typeCodeFind__SoQDT;
                // this.typeCodeFind = typeCodeFind__MaYL;
                txtKeyWord.Text = this.currentData.bidNumber;
            }
            // InitTypeFind();
            InitTypeFind();
            InitFindDate();
            this.MeShow();
        }

        private void InitTypeFind()
        {
            try
            {

                DXPopupMenu menu = new DXPopupMenu();
                DXMenuItem itemKeyworkCode = new DXMenuItem(typeCodeFind__KeyWork, new EventHandler(btnCodeFind_Click));
                itemKeyworkCode.Tag = "KeyWord";
                menu.Items.Add(itemKeyworkCode);

                DXMenuItem itemPatientCode = new DXMenuItem(typeCodeFind__MaBN, new EventHandler(btnCodeFind_Click));
                itemPatientCode.Tag = "patientCode";
                menu.Items.Add(itemPatientCode);

                DXMenuItem itemTreatmentCode = new DXMenuItem(typeCodeFind__MaDT, new EventHandler(btnCodeFind_Click));
                itemTreatmentCode.Tag = "treatmentCode";
                menu.Items.Add(itemTreatmentCode);

                DXMenuItem itemExpMestCodeCode = new DXMenuItem(typeCodeFind__MaPX, new EventHandler(btnCodeFind_Click));
                itemExpMestCodeCode.Tag = "expMestCode";
                menu.Items.Add(itemExpMestCodeCode);

                DXMenuItem itemImpMestCode = new DXMenuItem(typeCodeFind__MaPN, new EventHandler(btnCodeFind_Click));
                itemImpMestCode.Tag = "impMestCode";
                menu.Items.Add(itemImpMestCode);

                DXMenuItem itemServiceRequestCode = new DXMenuItem(typeCodeFind__MaYL, new EventHandler(btnCodeFind_Click));
                itemServiceRequestCode.Tag = "serviceRequestCode";
                menu.Items.Add(itemServiceRequestCode);

                DXMenuItem itemBidNumber = new DXMenuItem(typeCodeFind__SoQDT, new EventHandler(btnCodeFind_Click));
                itemBidNumber.Tag = "BidNumber";
                menu.Items.Add(itemBidNumber);

                btnCodeFind.DropDownControl = menu;

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnCodeFind_Click(object sender, EventArgs e)
        {
            try
            {
                var btnMenuCodeFind = sender as DXMenuItem;
                btnCodeFind.Text = btnMenuCodeFind.Caption;
                this.typeCodeFind = btnMenuCodeFind.Caption;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnExportExcel_Click(object sender, EventArgs e)
        {
            try
            {
                SaveFileDialog saveFile = new SaveFileDialog();
                saveFile.Filter = "Excel file|*.xlsx|All file|*.*";
                if (saveFile.ShowDialog() == DialogResult.OK)
                {
                    gridViewEventLog.ExportToXlsx(saveFile.FileName);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewEventLog_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.ListSourceRowIndex >= 0 && e.IsGetData && e.Column.UnboundType != DevExpress.Data.UnboundColumnType.Bound)
                {
                    var data = (SDA.EFMODEL.DataModels.SDA_EVENT_LOG)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    if (data != null)
                    {
                        if (e.Column.FieldName == "STT")
                        {
                            try
                            {
                                e.Value = e.ListSourceRowIndex + 1 + (ucPaging.pagingGrid != null ? ((ucPaging.pagingGrid.CurrentPage - 1) * (ucPaging.pagingGrid.PageSize)) : 0);
                            }
                            catch (Exception ex)
                            {
                                Inventec.Common.Logging.LogSystem.Warn("Set value cho STT grid sdaeventlog: " + ex);
                            }
                        }
                        else if (e.Column.FieldName == "EVENT_TIME_DISPLAY")
                        {
                            try
                            {
                                e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(data.EVENT_TIME ?? 0);
                            }
                            catch (Exception ex)
                            {
                                Inventec.Common.Logging.LogSystem.Error("Set value cho CREATE_TIME grid Sda Event Log: " + ex);
                            }
                        }
                        else if (e.Column.FieldName == "LOG_CREATE_TIME")
                        {
                            try
                            {
                                e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(data.CREATE_TIME ?? 0);
                            }
                            catch (Exception ex)
                            {
                                Inventec.Common.Logging.LogSystem.Error("Set value cho CREATE_TIME grid Sda Event Log: " + ex);
                            }
                        }
                        else if (e.Column.FieldName == "LOG_MODIFY_TIME")
                        {
                            try
                            {
                                e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(data.MODIFY_TIME ?? 0);
                            }
                            catch (Exception ex)
                            {
                                Inventec.Common.Logging.LogSystem.Error("Set value cho MODIFY_TIME grid SdaEventLog: " + ex);
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

        private void btnCodeFind_TextChanged(object sender, EventArgs e)
        {
            try
            {
                txtKeyWord.Text = "";
                txtKeyWord.Focus();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnPrevious_Click(object sender, EventArgs e)
        {
            try
            {
                if (dtTime.EditValue != null && dtTime.DateTime != DateTime.MinValue && !String.IsNullOrWhiteSpace(cboDayAndMonth.Text))
                {
                    var currentdate = dtTime.DateTime;
                    if (this.typeCodeFind__KeyWork_InDate == this.typeCodeFind_InDate)
                        dtTime.EditValue = currentdate.AddDays(-1);
                    else
                        dtTime.EditValue = currentdate.AddMonths(-1);

                    btnSearch_Click(null, null);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnNext_Click(object sender, EventArgs e)
        {
            try
            {
                if (dtTime.EditValue != null && dtTime.DateTime != DateTime.MinValue && !String.IsNullOrWhiteSpace(cboDayAndMonth.Text))
                {
                    var currentdate = dtTime.DateTime;
                    if (this.typeCodeFind__KeyWork_InDate == this.typeCodeFind_InDate)
                        dtTime.EditValue = currentdate.AddDays(1);
                    else
                        dtTime.EditValue = currentdate.AddMonths(1);

                    btnSearch_Click(null, null);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void bbtnSearch_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                btnSearch_Click(null,null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void Reset() 
        {
            try
            {
                SetDefaultValueControl();
                btnCodeFind.Text = typeCodeFind__KeyWork;
                DateTime timeFrom = DateTime.Now;
                //dtTimeFrom.EditValue = timeFrom;
                dtTime.EditValue = DateTime.Now;
                txtKeyWord.Text = "";
                txtKeyWord.Focus();
                txtKeyWord.SelectAll();
                this.typeCodeFind = typeCodeFind__KeyWork;
                this.FillDataToControl();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Debug(ex);
            }

        }

        private void bbtnReset_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                Reset();


                //SetDefaultValueControl();
                //btnCodeFind.Text = typeCodeFind__KeyWork;
                //DateTime timeFrom = DateTime.Now;
                ////dtTimeFrom.EditValue = timeFrom;
                //dtTime.EditValue = DateTime.Now;
                //txtKeyWord.Text = "";
                //txtKeyWord.Focus();
                //txtKeyWord.SelectAll();
                //this.typeCodeFind = typeCodeFind__KeyWork;
                //this.FillDataToControl();
            }
            catch (Exception ex)
            {
                 Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void BtnSearch() 
        {
            try
            {
                btnSearch_Click(null,null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Debug(ex);
            }
        }

        private void BtnRefreshs() 
        {
            try
            {
                Reset();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void InitFindDate()
        {
            try
            {
                DXPopupMenu menu = new DXPopupMenu();
                DXMenuItem itemInDateCode = new DXMenuItem(typeCodeFind__KeyWork_InDate, new EventHandler(cboDayAndMonth_Click));
                itemInDateCode.Tag = "InDate";
                menu.Items.Add(itemInDateCode);

                DXMenuItem itemInMonth = new DXMenuItem(typeCodeFind__InMonth, new EventHandler(cboDayAndMonth_Click));
                itemInMonth.Tag = "InMonth";
                menu.Items.Add(itemInMonth);

                DXMenuItem itemInTime = new DXMenuItem(typeCodeFind__InTime, new EventHandler(cboDayAndMonth_Click));
                itemInTime.Tag = "InTime";
                menu.Items.Add(itemInTime);

                cboDayAndMonth.DropDownControl = menu;

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void FormatDtIntructionDate()
        {
            try
            {
                layoutDateCome.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                layoutControlItem7.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                layoutControlItem12.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                if (this.typeCodeFind__KeyWork_InDate == this.typeCodeFind_InDate)
                {
                    dtTime.Properties.VistaCalendarViewStyle = DevExpress.XtraEditors.VistaCalendarViewStyle.Default;
                    dtTime.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Custom;
                    dtTime.Properties.DisplayFormat.FormatString = "dd/MM/yyyy";
                    dtTime.Properties.EditFormat.FormatType = DevExpress.Utils.FormatType.Custom;
                    dtTime.Properties.EditFormat.FormatString = "dd/MM/yyyy";
                    dtTime.Properties.EditMask = "dd/MM/yyyy";
                    dtTime.Properties.Mask.EditMask = "dd/MM/yyyy";
                }
                else if (this.typeCodeFind__KeyWork_InDate == this.typeCodeFind__InMonth)
                {
                    dtTime.Properties.VistaCalendarViewStyle = DevExpress.XtraEditors.VistaCalendarViewStyle.YearView;
                    dtTime.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Custom;
                    dtTime.Properties.DisplayFormat.FormatString = "MM/yyyy";
                    dtTime.Properties.EditFormat.FormatType = DevExpress.Utils.FormatType.Custom;
                    dtTime.Properties.EditFormat.FormatString = "MM/yyyy";
                    dtTime.Properties.EditMask = "MM/yyyy";
                    dtTime.Properties.Mask.EditMask = "MM/yyyy";
                }
                else 
                {
                    
                    layoutDateCome.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                    layoutControlItem7.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                    layoutControlItem12.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                    dtTime.Properties.VistaCalendarViewStyle = DevExpress.XtraEditors.VistaCalendarViewStyle.Default;
                    dtTime.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Custom;
                    dtTime.Properties.DisplayFormat.FormatString = "dd/MM/yyyy";
                    dtTime.Properties.EditFormat.FormatType = DevExpress.Utils.FormatType.Custom;
                    dtTime.Properties.EditFormat.FormatString = "dd/MM/yyyy";
                    dtTime.Properties.EditMask = "dd/MM/yyyy";
                    dtTime.Properties.Mask.EditMask = "dd/MM/yyyy";
                    dtDateCome.Properties.VistaCalendarViewStyle = DevExpress.XtraEditors.VistaCalendarViewStyle.Default;
                    dtDateCome.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Custom;
                    dtDateCome.Properties.DisplayFormat.FormatString = "dd/MM/yyyy";
                    dtDateCome.Properties.EditFormat.FormatType = DevExpress.Utils.FormatType.Custom;
                    dtDateCome.Properties.EditFormat.FormatString = "dd/MM/yyyy";
                    dtDateCome.Properties.EditMask = "dd/MM/yyyy";
                    dtDateCome.Properties.Mask.EditMask = "dd/MM/yyyy";
                    dtDateCome.EditValue = DateTime.Now;
                    dtTime.EditValue = DateTime.Now;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboDayAndMonth_Click(object sender, EventArgs e)
        {
            try
            {
                var btnMenuCodeFind = sender as DXMenuItem;
                cboDayAndMonth.Text = btnMenuCodeFind.Caption;
                this.typeCodeFind__KeyWork_InDate = btnMenuCodeFind.Caption;

                FormatDtIntructionDate();

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        void LoadDefaultData()
        {
            try
            {
                layoutDateCome.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                layoutControlItem7.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                layoutControlItem12.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                btnCodeFind.Text = typeCodeFind__KeyWork;
                cboDayAndMonth.Text = typeCodeFind__KeyWork_InDate;
                FormatDtIntructionDate();
                dtTime.DateTime = DateTime.Now;
               
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        
    }
}
