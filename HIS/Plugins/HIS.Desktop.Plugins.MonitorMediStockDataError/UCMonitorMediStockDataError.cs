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
using HIS.Desktop.LocalStorage.BackendData;
using MOS.EFMODEL.DataModels;
using DevExpress.Utils;
using HIS.Desktop.Utilities.Extensions;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Repository;
using HIS.Desktop.Plugins.MonitorMediStockDataError.Config;
using Inventec.Desktop.Common.Message;
using HIS.Desktop.ApiConsumer;
using Inventec.Core;
using Inventec.Common.Adapter;
using HIS.Desktop.LocalStorage.ConfigApplication;
using HIS.Desktop.Plugins.MonitorMediStockDataError.ADO;
using DevExpress.XtraGrid.Views.Base;
using System.Collections;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using DevExpress.XtraGrid.Columns;
using MOS.Filter;
using System.Resources;
using Inventec.Desktop.Common.LanguageManager;
using DevExpress.XtraGrid.Views.Grid;

namespace HIS.Desktop.Plugins.MonitorMediStockDataError
{
    public partial class UCMonitorMediStockDataError : UserControlBase
    {

        #region derlare

        internal long roomId;
        internal long roomTypeId;
        int start = 0;
        int limit = 0;
        int rowCount = 0;
        int dataTotal = 0;
        Inventec.Desktop.Common.Modules.Module currentModule;
        List<V_HIS_STOCK_DATA_ERROR> lstDataErr = new List<V_HIS_STOCK_DATA_ERROR>();
        #endregion

        public UCMonitorMediStockDataError()
        {
            InitializeComponent();
        }
        public UCMonitorMediStockDataError(Inventec.Desktop.Common.Modules.Module moduleData)
            : base(moduleData)
        {
            InitializeComponent();
            try
            {
                this.currentModule = moduleData;
                this.roomId = moduleData.RoomId;
                this.roomTypeId = moduleData.RoomTypeId;
                SetCaptionByLanguageKey();
                Inventec.Common.Logging.LogSystem.Debug("Gọi module thành công");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>
        ///Hàm xét ngôn ngữ cho giao diện UCMonitorMediStockDataError
        /// </summary>
        private void SetCaptionByLanguageKey()
        {
            try
            {
                ////Khoi tao doi tuong resource
                Resources.ResourceLanguageManager.LanguageResource = new ResourceManager("HIS.Desktop.Plugins.MonitorMediStockDataError.Resources.Lang", typeof(UCMonitorMediStockDataError).Assembly);

                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.layoutControl1.Text = Inventec.Common.Resource.Get.Value("UCMonitorMediStockDataError.layoutControl1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn1.Caption = Inventec.Common.Resource.Get.Value("UCMonitorMediStockDataError.gridColumn1.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn2.Caption = Inventec.Common.Resource.Get.Value("UCMonitorMediStockDataError.gridColumn2.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn3.Caption = Inventec.Common.Resource.Get.Value("UCMonitorMediStockDataError.gridColumn3.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn4.Caption = Inventec.Common.Resource.Get.Value("UCMonitorMediStockDataError.gridColumn4.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn5.Caption = Inventec.Common.Resource.Get.Value("UCMonitorMediStockDataError.gridColumn5.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn6.Caption = Inventec.Common.Resource.Get.Value("UCMonitorMediStockDataError.gridColumn6.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn7.Caption = Inventec.Common.Resource.Get.Value("UCMonitorMediStockDataError.gridColumn7.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn8.Caption = Inventec.Common.Resource.Get.Value("UCMonitorMediStockDataError.gridColumn8.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn9.Caption = Inventec.Common.Resource.Get.Value("UCMonitorMediStockDataError.gridColumn9.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn10.Caption = Inventec.Common.Resource.Get.Value("UCMonitorMediStockDataError.gridColumn10.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn11.Caption = Inventec.Common.Resource.Get.Value("UCMonitorMediStockDataError.gridColumn11.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn12.Caption = Inventec.Common.Resource.Get.Value("UCMonitorMediStockDataError.gridColumn12.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn13.Caption = Inventec.Common.Resource.Get.Value("UCMonitorMediStockDataError.gridColumn13.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn14.Caption = Inventec.Common.Resource.Get.Value("UCMonitorMediStockDataError.gridColumn14.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnExport.Text = Inventec.Common.Resource.Get.Value("UCMonitorMediStockDataError.btnExport.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnSearch.Text = Inventec.Common.Resource.Get.Value("UCMonitorMediStockDataError.btnSearch.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.chkNearestDate.Properties.Caption = Inventec.Common.Resource.Get.Value("UCMonitorMediStockDataError.chkNearestDate.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.chkUnprocess.Properties.Caption = Inventec.Common.Resource.Get.Value("UCMonitorMediStockDataError.chkUnprocess.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.txtSearch.Properties.NullText = Inventec.Common.Resource.Get.Value("UCMonitorMediStockDataError.txtSearch.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.labelControl6.Text = Inventec.Common.Resource.Get.Value("UCMonitorMediStockDataError.labelControl6.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.labelControl5.Text = Inventec.Common.Resource.Get.Value("UCMonitorMediStockDataError.labelControl5.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.labelControl4.Text = Inventec.Common.Resource.Get.Value("UCMonitorMediStockDataError.labelControl4.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.labelControl3.Text = Inventec.Common.Resource.Get.Value("UCMonitorMediStockDataError.labelControl3.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.labelControl2.Text = Inventec.Common.Resource.Get.Value("UCMonitorMediStockDataError.labelControl2.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnCheck.Text = Inventec.Common.Resource.Get.Value("UCMonitorMediStockDataError.btnCheck.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnSaveConfig.Text = Inventec.Common.Resource.Get.Value("UCMonitorMediStockDataError.btnSaveConfig.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboMediStock.Properties.NullText = Inventec.Common.Resource.Get.Value("UCMonitorMediStockDataError.cboMediStock.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.labelControl1.Text = Inventec.Common.Resource.Get.Value("UCMonitorMediStockDataError.labelControl1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem2.Text = Inventec.Common.Resource.Get.Value("UCMonitorMediStockDataError.layoutControlItem2.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem7.Text = Inventec.Common.Resource.Get.Value("UCMonitorMediStockDataError.layoutControlItem7.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem14.Text = Inventec.Common.Resource.Get.Value("UCMonitorMediStockDataError.layoutControlItem14.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.IS_PROCESSED.Caption = Inventec.Common.Resource.Get.Value("UCMonitorMediStockDataError.gridColumn15.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }


        private void UCMonitorMediStockDataError_Load(object sender, EventArgs e)
        {
            try
            {
                HisConfig.LoadConfig();
                InitMediStockCheck();
                Inventec.Common.Logging.LogSystem.Debug("Gọi 1");
                InitComboMediStock();
                Inventec.Common.Logging.LogSystem.Debug("Gọi 2");
                DefaultDataToForm();
                Inventec.Common.Logging.LogSystem.Debug("Gọi 3");
                FillDataToGrid();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void DefaultDataToForm()
        {
            try
            {
                chkUnprocess.Checked = false;
                chkNearestDate.Checked = false;
                txtSearch.Properties.NullText = "Từ khóa tìm kiếm";
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
                int numPageSize = 0;
                if (ucPaging1.pagingGrid != null)
                {
                    numPageSize = ucPaging1.pagingGrid.PageSize;
                }
                else
                {
                    numPageSize = ConfigApplicationWorker.Get<int>("CONFIG_KEY__NUM_PAGESIZE");
                }

                FillDataToGridMeDiStockErr(new CommonParam(0, numPageSize));

                CommonParam param = new CommonParam();
                param.Limit = rowCount;
                param.Count = dataTotal;
                ucPaging1.Init(FillDataToGridMeDiStockErr, param, numPageSize, this.gridControlMediStockDataErr);

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void FillDataToGridMeDiStockErr(object param)
        {
            try
            {
                List<V_HIS_STOCK_DATA_ERROR> listData = new List<V_HIS_STOCK_DATA_ERROR>();
                gridControlMediStockDataErr.DataSource = null;
                start = ((CommonParam)param).Start ?? 0;
                limit = ((CommonParam)param).Limit ?? 10;
                CommonParam paramCommon = new CommonParam(start, limit);
                HisStockDataErrorViewFilter filter = new HisStockDataErrorViewFilter();
                filter.ORDER_FIELD = "MODIFY_TIME";
                filter.ORDER_DIRECTION = "DESC";
                filter.KEY_WORD = txtSearch.Text.Trim();
                if (chkNearestDate.Checked == true)
                {
                    filter.GET_LASTEST_DATE = true;
                }
                else
                {
                    filter.GET_LASTEST_DATE = null;
                }
                if (chkUnprocess.Checked == true)
                {
                    filter.IS_PROCESSED = false;
                }
                else
                {
                    filter.IS_PROCESSED = null;
                }
                if (filter.KEY_WORD == "") paramCommon = new CommonParam(start, limit);
                var result = new Inventec.Common.Adapter.BackendAdapter(paramCommon).GetRO<List<V_HIS_STOCK_DATA_ERROR>>(RequestUriStore.RequestUriStore.HIS_STOCK_DATA_ERROR_GETVIEW, ApiConsumers.MosConsumer, filter, paramCommon);
                if (result != null)
                {
                    gridControlMediStockDataErr.BeginUpdate();
                    gridControlMediStockDataErr.DataSource = null;
                    listData = (List<V_HIS_STOCK_DATA_ERROR>)result.Data;
                    rowCount = (listData == null ? 0 : listData.Count);
                    dataTotal = (result.Param == null ? 0 : result.Param.Count ?? 0);
                    gridControlMediStockDataErr.DataSource = listData;
                    gridControlMediStockDataErr.EndUpdate();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void InitComboMediStock()
        {
            try
            {
                List<string> dsMaKho = new List<string>();
                List<HIS_MEDI_STOCK> dsKho = new List<HIS_MEDI_STOCK>();
                if (!string.IsNullOrEmpty(HisConfig.MediStockCode))
                {
                    dsMaKho.AddRange(HisConfig.MediStockCode.Split(',').Select(x => x.Trim()).ToArray()); 
                }
                var listMediStocks = BackendDataWorker.Get<HIS_MEDI_STOCK>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE && o.IS_BLOOD != 1).ToList();
                if (dsMaKho != null && dsMaKho.Count > 0)
                {
                    var lstConfigMediCode = listMediStocks.Where(o => dsMaKho.Contains(o.MEDI_STOCK_CODE)).ToList();
                    GridCheckMarksSelection gridCheckMark = cboMediStock.Properties.Tag as GridCheckMarksSelection;
                    if (gridCheckMark != null)
                    {
                        gridCheckMark.SelectAll(lstConfigMediCode);
                    }
                }
                cboMediStock.Properties.DataSource = listMediStocks;
                cboMediStock.Properties.DisplayMember = "MEDI_STOCK_NAME";
                cboMediStock.Properties.ValueMember = "ID";
                DevExpress.XtraGrid.Columns.GridColumn col2 = cboMediStock.Properties.View.Columns.AddField("MEDI_STOCK_NAME");
                col2.OptionsFilter.AutoFilterCondition = DevExpress.XtraGrid.Columns.AutoFilterCondition.Contains;
                col2.VisibleIndex = 1;
                col2.Width = 350;
                col2.Caption = "Tất cả";
                cboMediStock.Properties.PopupFormWidth = 350;
                cboMediStock.Properties.View.OptionsView.ShowColumnHeaders = true;
                cboMediStock.Properties.View.OptionsSelection.MultiSelect = true;
                //cboMediStock.Properties.View.OptionsSelection.MultiSelectMode = GridMultiSelectMode.CheckBoxRowSelect;
                cboMediStock.Properties.View.OptionsSelection.ShowCheckBoxSelectorInColumnHeader = DefaultBoolean.True;
                cboMediStock.Properties.View.OptionsView.GroupDrawMode = DevExpress.XtraGrid.Views.Grid.GroupDrawMode.Office;
                cboMediStock.Properties.View.OptionsView.HeaderFilterButtonShowMode = DevExpress.XtraEditors.Controls.FilterButtonShowMode.SmartTag;
                cboMediStock.Properties.View.OptionsView.ShowAutoFilterRow = true;
                cboMediStock.Properties.View.OptionsView.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;

                //GridCheckMarksSelection gridCheckMark = cboMediStock.Properties.Tag as GridCheckMarksSelection;
                //if (gridCheckMark != null)
                //{
                //    gridCheckMark.SelectAll(cboMediStock.Properties.DataSource);
                //}
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        List<HIS_MEDI_STOCK> _MediStockSelecteds;
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
                _MediStockSelecteds = new List<HIS_MEDI_STOCK>();
                string mediStockName = "";
                foreach (HIS_MEDI_STOCK rv in (sender as GridCheckMarksSelection).Selection)
                {
                    if (rv != null)
                        _MediStockSelecteds.Add(rv);
                }
                if (_MediStockSelecteds != null && _MediStockSelecteds.Count > 0)
                {
                    foreach (var item in _MediStockSelecteds)
                    {
                        mediStockName += item.MEDI_STOCK_NAME + ", ";
                    }
                }
                mediStockName = mediStockName.Trim();
                mediStockName = mediStockName.TrimEnd(',');
                cboMediStock.Text = mediStockName;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboMediStock_CustomDisplayText(object sender, DevExpress.XtraEditors.Controls.CustomDisplayTextEventArgs e)
        {
            try
            {
                //StringBuilder sb = new StringBuilder();
                //GridCheckMarksSelection gridCheckMark = sender is GridLookUpEdit ? (sender as GridLookUpEdit).Properties.Tag as GridCheckMarksSelection : (sender as RepositoryItemGridLookUpEdit).Tag as GridCheckMarksSelection;
                //if (gridCheckMark == null) return;
                //foreach (MOS.EFMODEL.DataModels.HIS_MEDI_STOCK rv in gridCheckMark.Selection)
                //{
                //    if (sb.ToString().Length > 0) { sb.Append(", "); }
                //    sb.Append(rv.MEDI_STOCK_NAME.ToString());
                //}
                //e.DisplayText = sb.ToString();
                e.DisplayText = "";
                string mediStockName = "";
                if (_MediStockSelecteds != null && _MediStockSelecteds.Count > 0)
                {
                    foreach (var item in _MediStockSelecteds)
                    {
                        mediStockName += item.MEDI_STOCK_NAME + ", ";
                    }
                }
                mediStockName = mediStockName.Trim();
                mediStockName = mediStockName.TrimEnd(',');
                e.DisplayText = mediStockName.Trim();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnSaveConfig_Click(object sender, EventArgs e)
        {
            try
            {
                bool rs = false;
                string lstCode = "";
                CommonParam param = new CommonParam();
                HIS_CONFIG dataUpdate = new HIS_CONFIG();
                HIS_CONFIG success = new HIS_CONFIG();
                var configCode = BackendDataWorker.Get<HIS_CONFIG>().Where(o => o.KEY == HisConfig.HIS_STOCK_DATA_ERROR_MEDI_STOCK_CODE).FirstOrDefault();
                WaitingManager.Show();
                if (configCode != null)
                {
                    Inventec.Common.Mapper.DataObjectMapper.Map<HIS_CONFIG>(dataUpdate, configCode);
                    if (_MediStockSelecteds != null && _MediStockSelecteds.Count > 0)
                    {
                        foreach (var i in _MediStockSelecteds)
                        {
                            lstCode += i.MEDI_STOCK_CODE + ",";
                        }
                    }
                    lstCode = lstCode.Trim();
                    lstCode = lstCode.TrimEnd(',');
                    dataUpdate.VALUE = lstCode;
                    success = new BackendAdapter(param).Post<HIS_CONFIG>("api/HisConfig/Update", ApiConsumers.MosConsumer, dataUpdate, param);
                }

                WaitingManager.Hide();
                if (success != null)
                {
                    BackendDataWorker.Reset<HIS_CONFIG>();
                    Inventec.Common.LocalStorage.SdaConfig.ConfigLoader.Refresh();
                    rs = true;
                }
                MessageManager.Show(param, rs);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnCheck_Click(object sender, EventArgs e)
        {
            try
            {
                CommonParam param = new CommonParam();
                //HIS_STOCK_DATA_ERROR data = new HIS_STOCK_DATA_ERROR();
                WaitingManager.Show();
                var rsScan = new BackendAdapter(param).Post<bool>("api/HisStockDataError/Scan", ApiConsumers.MosConsumer, null, param);
                WaitingManager.Hide();
                MessageManager.Show(param, rsScan);
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
                FillDataToGrid();
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
                if (e.IsGetData && e.Column.UnboundType != DevExpress.Data.UnboundColumnType.Bound)
                {
                    var data = (V_HIS_STOCK_DATA_ERROR)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    if (data != null)
                    {
                        if (e.Column.FieldName == "CREATE_TIME_STR")
                        {
                            try
                            {
                                e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString((long)data.CREATE_TIME);
                            }
                            catch (Exception ex)
                            {
                                Inventec.Common.Logging.LogSystem.Error(ex);
                            }
                        }
                        else if (e.Column.FieldName == "IMP_TIME_STR")
                        {
                            try
                            {
                                string timeStr = Inventec.Common.DateTime.Convert.TimeNumberToTimeString((long)data.IMP_TIME);
                                string ngayThang = "";
                                if(!string.IsNullOrEmpty(timeStr))
                                {
                                     ngayThang = timeStr.Substring(0, 10);
                                }
                                e.Value = ngayThang;
                            }
                            catch (Exception ex)
                            {
                                Inventec.Common.Logging.LogSystem.Error(ex);
                            }
                        }
                        else if (e.Column.FieldName == "EXPIRED_DATE_STR")
                        {
                            try
                            {
                                string timeStr = Inventec.Common.DateTime.Convert.TimeNumberToTimeString((long)data.EXPIRED_DATE);
                                string ngayThang = "";
                                if (!string.IsNullOrEmpty(timeStr))
                                {
                                    ngayThang = timeStr.Substring(0, 10);
                                }
                                e.Value = ngayThang;
                            }
                            catch (Exception ex)
                            {
                                Inventec.Common.Logging.LogSystem.Error(ex);
                            }
                        }
                        else if (e.Column.FieldName == "MEDIMATERIAL_CODE")
                        {
                            try
                            {
                                if (!string.IsNullOrEmpty(data.MEDICINE_TYPE_CODE))
                                {
                                    e.Value = data.MEDICINE_TYPE_CODE;
                                }
                                else
                                {
                                    e.Value = data.MATERIAL_TYPE_CODE;
                                }
                            }
                            catch (Exception ex)
                            {
                                Inventec.Common.Logging.LogSystem.Error(ex);
                            }
                        }
                        else if (e.Column.FieldName == "MEDIMATERIAL_NAME")
                        {
                            try
                            {
                                if (!string.IsNullOrEmpty(data.MEDICINE_TYPE_NAME))
                                {
                                    e.Value = data.MEDICINE_TYPE_NAME;
                                }
                                else
                                {
                                    e.Value = data.MATERIAL_TYPE_NAME;
                                }
                            }
                            catch (Exception ex)
                            {
                                Inventec.Common.Logging.LogSystem.Error(ex);
                            }
                        }
                        else if (e.Column.FieldName == "MEDIMATE_ID")
                        {
                            try
                            {
                                if (data.MEDICINE_ID != null)
                                {
                                    e.Value = data.MEDICINE_ID;
                                }
                                else
                                {
                                    e.Value = data.MATERIAL_ID;
                                }
                            }
                            catch (Exception ex)
                            {
                                Inventec.Common.Logging.LogSystem.Error(ex);
                            }
                        }
                        else if (e.Column.FieldName == "AMOUNT_STR")
                        {
                            try
                            {
                                if (data.IN_AMOUNT != null && data.OUT_AMOUNT != null)
                                {
                                    decimal amount = (data.IN_AMOUNT - data.OUT_AMOUNT)??0;
                                    string convertAmount = amount.ToString("N0");
                                    e.Value = convertAmount;
                                }
                                else
                                {
                                    e.Value = 0;
                                }
                            }
                            catch (Exception ex)
                            {
                                Inventec.Common.Logging.LogSystem.Error(ex);
                            }
                        }
                        else if (e.Column.FieldName == "BEGIN_AMOUNT_STR")
                        {
                            try
                            {
                                    string convertAmount = (data.BEGIN_AMOUNT??0).ToString("N0");
                                    e.Value = convertAmount;
                            }
                            catch (Exception ex)
                            {
                                Inventec.Common.Logging.LogSystem.Error(ex);
                            }
                        }
                        else if (e.Column.FieldName == "IN_AMOUNT_STR")
                        {
                            try
                            {
                                string convertAmount = (data.IN_AMOUNT ?? 0).ToString("N0");
                                e.Value = convertAmount;
                            }
                            catch (Exception ex)
                            {
                                Inventec.Common.Logging.LogSystem.Error(ex);
                            }
                        }
                        else if (e.Column.FieldName == "OUT_AMOUNT_STR")
                        {
                            try
                            {
                                string convertAmount = (data.OUT_AMOUNT ?? 0).ToString("N0");
                                e.Value = convertAmount;
                            }
                            catch (Exception ex)
                            {
                                Inventec.Common.Logging.LogSystem.Error(ex);
                            }
                        }
                        else if (e.Column.FieldName == "BEAN_AMOUNT_STR")
                        {
                            try
                            {
                                string convertAmount = (data.BEAN_AMOUNT ?? 0).ToString("N0");
                                e.Value = convertAmount;
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

        private void gridView1_CustomRowCellEdit(object sender, DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs e)
        {
            try
            {
                DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                if (e.RowHandle >= 0)
                {
                    V_HIS_STOCK_DATA_ERROR data = (V_HIS_STOCK_DATA_ERROR)((IList)((BaseView)sender).DataSource)[e.RowHandle];
                    if (e.Column.FieldName == "IS_PROCESS")
                    {
                        e.RepositoryItem = (data.IS_PROCESSED == 1 ? repositoryNotProcess : repositoryProcess);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void repositoryProcess_Click(object sender, EventArgs e)
        {
            try
            {
                CommonParam param = new CommonParam();
                HIS_STOCK_DATA_ERROR success = new HIS_STOCK_DATA_ERROR();
                V_HIS_STOCK_DATA_ERROR data = (V_HIS_STOCK_DATA_ERROR)gridView1.GetFocusedRow();
                bool rs = false;
                WaitingManager.Show();
                if (data != null)
                {
                    success = new Inventec.Common.Adapter.BackendAdapter(param).Post<HIS_STOCK_DATA_ERROR>(RequestUriStore.RequestUriStore.HIS_STOCK_DATA_ERROR_ISPROCESS, ApiConsumers.MosConsumer, data.ID, param);
                }
                WaitingManager.Hide();
                if (success != null)
                {
                    BackendDataWorker.Reset<HIS_STOCK_DATA_ERROR>();
                    FillDataToGrid();
                    rs = true;
                }
                MessageManager.Show(param, rs);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void repositoryNotProcess_Click(object sender, EventArgs e)
        {
            try
            {
                try
                {
                    CommonParam param = new CommonParam();
                    HIS_STOCK_DATA_ERROR success = new HIS_STOCK_DATA_ERROR();
                    V_HIS_STOCK_DATA_ERROR data = (V_HIS_STOCK_DATA_ERROR)gridView1.GetFocusedRow();
                    bool rs = false;
                    WaitingManager.Show();
                    if (data != null)
                    {
                        success = new Inventec.Common.Adapter.BackendAdapter(param).Post<HIS_STOCK_DATA_ERROR>(RequestUriStore.RequestUriStore.HIS_STOCK_DATA_ERROR_ISUNPROCESS, ApiConsumers.MosConsumer, data.ID, param);
                    }
                    WaitingManager.Hide();
                    if (success != null)
                    {
                        BackendDataWorker.Reset<HIS_STOCK_DATA_ERROR>();
                        FillDataToGrid();
                        rs = true;
                    }
                    MessageManager.Show(param, rs);
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Error(ex);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnExport_Click(object sender, EventArgs e)
        {
            try
            {
                if (this.gridControlMediStockDataErr != null)
                {
                    SaveFileDialog saveFile = new SaveFileDialog();
                    saveFile.Filter = "Excel file|*.xlsx|All file|*.*";
                    if (saveFile.ShowDialog() == DialogResult.OK)
                    {
                        GridView gridView = gridControlMediStockDataErr.MainView as GridView;
                        if (gridView != null && gridView.Columns.Count > 0)
                        {
                            GridColumn column = gridView.Columns[0];
                            if (column != null)
                            {
                                column.Visible = false;
                                // Xuất dữ liệu vào file Excel
                                this.gridControlMediStockDataErr.ExportToXlsx(saveFile.FileName);
                                // Hiển thị lại cột 
                                column.Visible = true;
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

        #region TooltipGrid
        GridColumn lastColumn = null;
        ToolTipControlInfo lastInfo = null;
        int lastRowHandle = -1;
        private void toolTipControllerGrid_GetActiveObjectInfo(object sender, ToolTipControllerGetActiveObjectInfoEventArgs e)
        {
            try
            {
                if (e.Info == null && e.SelectedControl == gridControlMediStockDataErr)
                {
                    DevExpress.XtraGrid.Views.Grid.GridView view = gridControlMediStockDataErr.FocusedView as DevExpress.XtraGrid.Views.Grid.GridView;
                    GridHitInfo info = view.CalcHitInfo(e.ControlMousePosition);
                    if (info.InRowCell)
                    {
                        if (lastRowHandle != info.RowHandle || lastColumn != info.Column)
                        {
                            lastColumn = info.Column;
                            lastRowHandle = info.RowHandle;
                            string text = "";
                            if (info.Column.FieldName == "IS_PROCESS")
                            {
                                long dataERR = Inventec.Common.TypeConvert.Parse.ToInt64((view.GetRowCellValue(lastRowHandle, "IS_PROCESSED") ?? "").ToString());
                                if (dataERR == 1)
                                {
                                    text = "Đánh dấu 'Chưa xử lý'";
                                }
                                else
                                {
                                    text = "Đánh dấu 'Đã xử lý'";
                                }

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
        #endregion
    }
}
