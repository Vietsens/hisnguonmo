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
using DevExpress.XtraGrid.Views.Base;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using DevExpress.Utils.Menu;
using Inventec.Common.LocalStorage.SdaConfig;

namespace Inventec.UC.EventLogControl.Design.Template3
{
    public partial class Template3 : UserControl
    {
        private int rowCount = 0;
        private int dataTotal = 0;
        private int startPage = 0;
        private Data.DataInit3 Data;
        private ProcessHasException HasException;
        private List<Data.DataGrid> ListEventLog = new List<Data.DataGrid>();
        private string typeCodeFind = Resources.ResourceMessage.typeCodeFind__KeyWork;

        private Dictionary<string, string> Generate;
        private Dictionary<string, string> Generate2;
        private List<string> listName;
        private List<string> fieldname;

        #region ctor
        public Template3()
        {
            InitializeComponent();
        }

        public Template3(Data.DataInit3 Data)
            : this()
        {
            this.Data = Data;
            Inventec.UC.EventLogControl.Base.ApiConsumerStore.SdaConsumer = new Inventec.Common.WebApiClient.ApiConsumer(Data.SdaUri, Data.AppCode);
        }
        #endregion

        #region Load
        private void Template3_Load(object sender, EventArgs e)
        {
            try
            {
                SetCaptionByLanguageKey();
                string key = " ";
                this.MeShow();
                this.InitTypeFind();
                if (this.Data.Filter.Split(':').Count() == 2)
                {
                    key = this.Data.Filter.Split(':')[0] + ":";
                }
                this.DropBtnSearch.Text = this.Generate[key];
                this.TxtKeyWord.Text = this.Data.Filter;
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
                //this.BtnExportExcel.Text = Resources.ResourceMessage.GetResourceMessage("INVENTEC_UC_EVENT_LOG_CONTROL__BTN_EXPORT_EXCEL");
                this.BtnRefresh.Text = Resources.ResourceMessage.GetResourceMessage("INVENTEC_UC_EVENT_LOG_CONTROL__BTN_REFRESH");
                this.BtnSearch.Text = Resources.ResourceMessage.GetResourceMessage("INVENTEC_UC_EVENT_LOG_CONTROL__BTN_SEARCH");
                this.Gc_AppCode.Caption = Resources.ResourceMessage.GetResourceMessage("INVENTEC_UC_EVENT_LOG_CONTROL__GC_APP_CODE");
                this.Gc_Description.Caption = Resources.ResourceMessage.GetResourceMessage("INVENTEC_UC_EVENT_LOG_CONTROL__GC_DESCRIPTION");
                this.Gc_EventTime.Caption = Resources.ResourceMessage.GetResourceMessage("INVENTEC_UC_EVENT_LOG_CONTROL__GC_EVENT_TIME");
                this.Gc_Ip.Caption = Resources.ResourceMessage.GetResourceMessage("INVENTEC_UC_EVENT_LOG_CONTROL__GC_IP");
                this.Gc_LogCreateTime.Caption = Resources.ResourceMessage.GetResourceMessage("INVENTEC_UC_EVENT_LOG_CONTROL__GC_LOG_CREATE_TIME");
                this.Gc_LoginName.Caption = Resources.ResourceMessage.GetResourceMessage("INVENTEC_UC_EVENT_LOG_CONTROL__GC_LOGIN_NAME");
                this.Gc_Stt.Caption = Resources.ResourceMessage.GetResourceMessage("INVENTEC_UC_EVENT_LOG_CONTROL__GC_STT");
                this.Gc_Stt.ToolTip = Resources.ResourceMessage.GetResourceMessage("INVENTEC_UC_EVENT_LOG_CONTROL__GC_STT_TOOL_TIP");
                this.LciDtTimeFrom.Text = Resources.ResourceMessage.GetResourceMessage("INVENTEC_UC_EVENT_LOG_CONTROL__LCI_DT_TIME_FROM");
                this.LciDtTimeTo.Text = Resources.ResourceMessage.GetResourceMessage("INVENTEC_UC_EVENT_LOG_CONTROL__LCI_DT_TIME_TO");
                this.TxtKeyWord.Properties.NullValuePrompt = Resources.ResourceMessage.GetResourceMessage("INVENTEC_UC_EVENT_LOG_CONTROL__TXT_KEY_WORD");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void InitTypeFind()
        {
            try
            {
                DXPopupMenu dXPopupMenu = new DXPopupMenu();
                string config = SdaConfigs.Get<string>("SDA_EVENT_LOG.CONFIG_GENERATE_MODULE");
                this.Generate = new Dictionary<string, string>();
                this.Generate2 = new Dictionary<string, string>();
                this.Generate.Clear();
                this.listName = new List<string>();
                this.fieldname = new List<string>();
                this.split(ref this.fieldname, ref this.listName, ref this.Generate, ref this.Generate2, config);
                foreach (string item2 in this.listName)
                {
                    DXMenuItem item = new DXMenuItem(item2, this.DropBtnSearch_Click);
                    dXPopupMenu.Items.Add(item);
                }
                this.DropBtnSearch.DropDownControl = dXPopupMenu;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SetDefaultValueControl()
        {
            try
            {
                Inventec.Common.DateTime.Get.StartDay();
                DtTimeFrom.EditValue = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(Inventec.Common.DateTime.Get.StartDay() ?? 0);
                DtTimeTo.EditValue = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(Inventec.Common.DateTime.Get.EndDay() ?? 0);
                if (this.Data.Filter != null)
                {
                    this.TxtKeyWord.Text = this.Data.Filter;
                }
                TxtKeyWord.Focus();
                TxtKeyWord.SelectAll();
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
                int pagingSize = ucPaging.pagingGrid != null ? ucPaging.pagingGrid.PageSize : (int)Data.PageNum;
                GridPaging(new CommonParam(0, pagingSize));
                CommonParam param = new CommonParam();
                param.Limit = rowCount;
                param.Count = dataTotal;
                ucPaging.Init(GridPaging, param, pagingSize, gridControlEventLog);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void GridPaging(object param)
        {
            try
            {
                WaitingManager.Show();
                startPage = ((CommonParam)param).Start ?? 0;
                int limit = ((CommonParam)param).Limit ?? 0;
                CommonParam paramCommon = new CommonParam(startPage, limit);

                SDA.Filter.SdaEventLogFilter filter = new SDA.Filter.SdaEventLogFilter();
                filter.ORDER_FIELD = "CREATE_TIME";
                filter.ORDER_DIRECTION = "DESC";

                try
                {
                    if (this.TxtKeyWord.Text.Length <= 12)
                    {
                        if (this.DropBtnSearch.Text == "Từ Khóa Tìm kiếm" || this.TxtKeyWord.Text == "" || this.TxtKeyWord == null)
                        {
                            this.DropBtnSearch.Text = "Từ Khóa Tìm kiếm";
                            filter.KEY_WORD = this.TxtKeyWord.Text.Trim();
                        }
                        else if (this.DropBtnSearch.Text == "Mã bệnh nhân")
                        {
                            string str = string.Format("{0:0000000000}", Convert.ToInt64(this.TxtKeyWord.Text));
                            this.TxtKeyWord.Text = "PATIENT_CODE: " + str;
                            filter.KEY_WORD = this.TxtKeyWord.Text;
                        }
                        else
                        {
                            string str = string.Format("{0:000000000000}", Convert.ToInt64(this.TxtKeyWord.Text));
                            this.TxtKeyWord.Text = this.Generate2[this.DropBtnSearch.Text] + " " + str;
                            filter.KEY_WORD = this.TxtKeyWord.Text;
                        }
                    }
                    else
                    {
                        filter.KEY_WORD = this.TxtKeyWord.Text;
                    }

                    if (DtTimeFrom.EditValue != null && DtTimeFrom.DateTime != DateTime.MinValue)
                    {
                        filter.EVENT_TIME_FROM = Inventec.Common.TypeConvert.Parse.ToInt64(Convert.ToDateTime((DtTimeFrom.EditValue ?? "").ToString()).ToString("yyyyMMddHHmm") + "00");
                    }
                    if (DtTimeTo.EditValue != null && DtTimeTo.DateTime != DateTime.MinValue)
                    {
                        filter.EVENT_TIME_TO = Inventec.Common.TypeConvert.Parse.ToInt64(Convert.ToDateTime((DtTimeTo.EditValue ?? "").ToString()).ToString("yyyyMMddHHmm") + "59");
                    }
                }
                catch (Exception ex)
                {
                    WaitingManager.Hide();
                    Inventec.Common.Logging.LogSystem.Error(ex);
                }

                //if (!String.IsNullOrEmpty(currentData.loginName))
                //{
                //    filter.LOGIN_NAME = currentData.loginName;
                //}
                var paramCount = new CommonParam();

                var dataEventLog = new Sda.EventLog.Get.SdaEventLogGet(paramCommon).Get(filter, ref paramCount);

                WaitingManager.Hide();
                if (dataEventLog != null)
                {
                    //int rowCount = dataEventLog.Total ?? 0;
                    //this.ListEventLog = ProcessDataGrid((List<SDA.EFMODEL.DataModels.SDA_EVENT_LOG>)dataEventLog.Data);

                    this.gridControlEventLog.BeginUpdate();
                    this.gridControlEventLog.DataSource = dataEventLog.Data;
                    rowCount = (ListEventLog == null ? 0 : ListEventLog.Count);
                    dataTotal = (paramCount == null ? 0 : paramCount.Count ?? 0);
                    this.gridControlEventLog.EndUpdate();
                }

                #region has exception
                if (HasException != null) HasException(paramCommon);
                #endregion
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        //private List<Data.DataGrid> ProcessDataGrid(List<SDA.EFMODEL.DataModels.SDA_EVENT_LOG> sdaEventLog)
        //{
        //    List<Data.DataGrid> result = new List<Data.DataGrid>();
        //    try
        //    {
        //        if (sdaEventLog != null && sdaEventLog.Count > 0)
        //        {
        //            foreach (var item in sdaEventLog)
        //            {
        //                Data.DataGrid data = new Data.DataGrid();
        //                Inventec.Common.Mapper.DataObjectMapper.Map<Data.DataGrid>(data, item);

        //                if (item.EVENT_TIME.HasValue)
        //                {
        //                    data.EVENT_TIME_DISPLAY = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(data.EVENT_TIME.Value);
        //                }

        //                if (item.CREATE_TIME.HasValue)
        //                {
        //                    data.LOG_CREATE_TIME = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(data.CREATE_TIME.Value);
        //                }

        //                if (item.MODIFY_TIME.HasValue)
        //                {
        //                    data.LOG_MODIFY_TIME = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(data.MODIFY_TIME.Value);
        //                }

        //                result.Add(data);
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Inventec.Common.Logging.LogSystem.Error(ex);
        //        result = new List<Data.DataGrid>();
        //    }
        //    return result;
        //}

        private void split(ref List<string> FieldName1, ref List<string> name1, ref Dictionary<string, string> generate, ref Dictionary<string, string> generate2, string config)
        {
            try
            {
                if (!string.IsNullOrEmpty(config))
                {
                    string[] array = config.Split('|');
                    for (int i = 0; i < array.Count(); i++)
                    {
                        string text = "";
                        string text2 = "";
                        this.splitFieldName(array[i], ref text, ref text2);
                        name1.Add(text);
                        FieldName1.Add(text2);
                        generate[text2] = text;
                        generate2[text] = text2;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void splitFieldName(string input, ref string name, ref string fieldName)
        {
            try
            {
                if (!string.IsNullOrEmpty(input))
                {
                    string[] array = input.Split('?');
                    if (array != null && array.Count() == 2)
                    {
                        name = array[0];
                        fieldName = array[1];
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        #endregion

        #region public method
        internal bool SetDeletgateHasException(ProcessHasException _HasException)
        {
            bool result = false;
            try
            {
                this.HasException = _HasException;
                if (this.HasException != null)
                {
                    result = true;
                }
                else
                {
                    Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => _HasException), _HasException));
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = false;
            }
            return result;
        }

        internal void MeShow()
        {
            try
            {
                SetDefaultValueControl();
                FillDataToGrid();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        internal void ShortcutButtonSearch()
        {
            try
            {
                BtnSearch_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        internal void ShortcutButtonRefresh()
        {
            try
            {
                BtnRefresh_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        #endregion

        #region Click
        private void BtnSearch_Click(object sender, EventArgs e)
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

        private void BtnRefresh_Click(object sender, EventArgs e)
        {
            try
            {
                this.DropBtnSearch.Text = this.listName[0];
                this.DtTimeFrom.EditValue = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(Inventec.Common.DateTime.Get.StartDay() ?? 0);
                this.DtTimeTo.EditValue = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(Inventec.Common.DateTime.Get.EndDay() ?? 0);
                this.TxtKeyWord.Text = "";
                this.TxtKeyWord.Focus();
                this.TxtKeyWord.SelectAll();
                FillDataToGrid();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void BtnExportExcel_Click(object sender, EventArgs e)
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

        private void DropBtnSearch_Click(object sender, EventArgs e)
        {
            try
            {
                DXMenuItem dXMenuItem = sender as DXMenuItem;
                this.DropBtnSearch.Text = dXMenuItem.Caption;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void barButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                BtnSearch_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void barButtonItem3_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                BtnRefresh_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        #endregion

        #region Event
        private void gridViewEventLog_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != DevExpress.Data.UnboundColumnType.Bound)
                {
                    var data = (SDA.EFMODEL.DataModels.SDA_EVENT_LOG)((System.Collections.IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    if (data != null)
                    {
                        if (e.Column.FieldName == "STT")
                        {
                            try
                            {
                                e.Value = e.ListSourceRowIndex + 1 + startPage;
                            }
                            catch (Exception ex)
                            {
                                Inventec.Common.Logging.LogSystem.Error("Set value cho STT grid sdaeventlog: " + ex);
                            }
                        }
                        else if (e.Column.FieldName == "EVENT_TIME_DISPLAY")
                        {
                            e.Value = (data.EVENT_TIME != null ? Inventec.Common.DateTime.Convert.TimeNumberToTimeString(data.EVENT_TIME.Value) : null);
                        }
                        else if (e.Column.FieldName == "LOG_CREATE_TIME")
                        {
                            e.Value = data.CREATE_TIME.HasValue ? Inventec.Common.DateTime.Convert.TimeNumberToTimeString(data.CREATE_TIME.Value) : null;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void TxtKeyWord_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
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

        private void DtTimeFrom_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    DtTimeTo.Focus();
                    DtTimeTo.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void DtTimeFrom_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == DevExpress.XtraEditors.PopupCloseMode.Normal)
                {
                    DtTimeTo.Focus();
                    DtTimeTo.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void DtTimeTo_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == DevExpress.XtraEditors.PopupCloseMode.Normal)
                {
                    BtnSearch.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void DtTimeTo_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    BtnSearch.Focus();
                    e.Handled = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        #endregion
    }
}
