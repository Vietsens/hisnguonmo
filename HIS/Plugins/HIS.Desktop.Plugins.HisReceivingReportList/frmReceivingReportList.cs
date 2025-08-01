using DevExpress.XtraExport;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Views.Base;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Controls.Session;
using HIS.Desktop.LocalStorage.ConfigApplication;
using Inventec.Common.Adapter;
using Inventec.Common.Logging;
using Inventec.Core;
using Inventec.Desktop.Common.LanguageManager;
using Inventec.Desktop.Common.Message;
using Inventec.UC.Paging;
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

namespace HIS.Desktop.Plugins.HisReceivingReportList
{
    public partial class frmReceivingReportList : HIS.Desktop.Utility.FormBase
    {
        #region Declare
        int rowCount = 0;
        int dataTotal = 0;
        int startPage = 0;
        PagingGrid pagingGrid;
        int positionHandle = -1;
        List<HIS_RECEIVING_REPORT> receivingReports;
        HIS_RECEIVING_REPORT currentModule;
        Inventec.Desktop.Common.Modules.Module moduleData;
        #endregion

        #region Constructor
        public frmReceivingReportList(Inventec.Desktop.Common.Modules.Module moduleData)
            : base(moduleData)
        {
            try
            {
                InitializeComponent();
                pagingGrid = new PagingGrid();
                this.moduleData = moduleData;
                this.KeyPreview = true;
                try
                {
                    string iconPath = System.IO.Path.Combine(
                        HIS.Desktop.LocalStorage.Location.ApplicationStoreLocation.ApplicationStartupPath,
                        System.Configuration.ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]);
                    this.Icon = Icon.ExtractAssociatedIcon(iconPath);
                }
                catch (Exception ex)
                {
                    LogSystem.Warn(ex);
                }
                // Thêm sự kiện cho nút xem chi tiết
                this.grvReportList.CustomRowCellEditForEditing += gridView1_CustomRowCellEditForEditing;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void frmReceivingReportList_Load(object sender, EventArgs e)
        {
            try
            {
                Show();
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }
        #endregion

        private void Show()
        {
            SetDefaultValue();
            SetDefaultFocus();
            SetResourceByLanguageKey();
            dtCreateTimeFrom.EditValue = DateTime.Now.Date;
            dtCreateTimeTo.EditValue = DateTime.Now.Date.AddHours(23).AddMinutes(59).AddSeconds(59);
            FillDataToControl();
        }

        private void SetDefaultValue()
        {
            try
            {
                txtSearch.Text = "";
                dtCreateTimeFrom.EditValue = DateTime.Now.Date;
                dtCreateTimeTo.EditValue = DateTime.Now.Date.AddHours(23).AddMinutes(59).AddSeconds(59);
            }
            catch (Exception ex)
            {
                LogSystem.Warn("Lỗi khi đặt giá trị mặc định: ", ex);
            }
        }
        private void SetDefaultFocus()
        {
            try
            {
                txtSearch.Focus();
                txtSearch.SelectAll();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Debug(ex);
            }
        }
        private void SetResourceByLanguageKey()
        {
            try
            {
                Resources.ResourceLanguageManager.LanguageResource = new System.Resources.ResourceManager(
                    "HIS.Desktop.Plugins.HisReceivingReportList.Resources.Lang",
                    typeof(HIS.Desktop.Plugins.HisReceivingReportList.frmReceivingReportList).Assembly);

                this.layoutControl1.Text = Inventec.Common.Resource.Get.Value("frmReceivingReportList.layoutControl1.Text",
                    Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());

                this.btnRefresh.Text = Inventec.Common.Resource.Get.Value("frmReceivingReportList.btnRefresh.Text",
                    Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnSearch.Text = Inventec.Common.Resource.Get.Value("frmReceivingReportList.btnSearch.Text",
                    Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());

                this.dtCreateTimeFrom.Text = Inventec.Common.Resource.Get.Value("frmReceivingReportList.dtCreateTimeFrom.Text",
                    Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.dtCreateTimeTo.Text = Inventec.Common.Resource.Get.Value("frmReceivingReportList.dtCreateTimeTo.Text",
                    Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.labelControl1.Text = Inventec.Common.Resource.Get.Value("frmReceivingReportList.labelControl1.Text",
                    Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.labelControl2.Text = Inventec.Common.Resource.Get.Value("frmReceivingReportList.labelControl2.Text",
                    Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());

                this.colSTT.Caption = Inventec.Common.Resource.Get.Value("frmReceivingReportList.colSTT.Caption",
                    Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.colReportID.Caption = Inventec.Common.Resource.Get.Value("frmReceivingReportList.colReportID.Caption",
                    Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.colBeginDate.Caption = Inventec.Common.Resource.Get.Value("frmReceivingReportList.colBeginDate.Caption",
                    Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.colEndDate.Caption = Inventec.Common.Resource.Get.Value("frmReceivingReportList.colEndDate.Caption",
                    Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.colReceivingReport.Caption = Inventec.Common.Resource.Get.Value("frmReceivingReportList.colReceivingReport.Caption",
                    Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.colCreationTime.Caption = Inventec.Common.Resource.Get.Value("frmReceivingReportList.colCreationTime.Caption",
                    Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.colPageNumber.Caption = Inventec.Common.Resource.Get.Value("frmReceivingReportList.colPageNumber.Caption",
                    Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.colTotalPage.Caption = Inventec.Common.Resource.Get.Value("frmReceivingReportList.colTotalPage.Caption",
                    Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.colCreator.Caption = Inventec.Common.Resource.Get.Value("frmReceivingReportList.colCreator.Caption",
                    Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.colModifier.Caption = Inventec.Common.Resource.Get.Value("frmReceivingReportList.colModifier.Caption",
                    Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.colActive.Caption = Inventec.Common.Resource.Get.Value("frmReceivingReportList.colActive.Caption",
                    Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.colCreateTime.Caption = Inventec.Common.Resource.Get.Value("frmReceivingReportList.colCreateTime.Caption",
                    Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.colModifyTime.Caption = Inventec.Common.Resource.Get.Value("frmReceivingReportList.colModifyTime.Caption",
                    Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());

                this.txtSearch.Properties.NullValuePrompt = Inventec.Common.Resource.Get.Value("frmReceivingReportList.txtSearch.Properties.NullValuePrompt",
                    Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());

                if (this.moduleData != null && !String.IsNullOrEmpty(this.moduleData.text))
                {
                    this.Text = this.moduleData.text;
                }
                else
                {
                    this.Text = Inventec.Common.Resource.Get.Value("frmHisHolidayPolicies.Text",
                        Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn("Lỗi khi thiết lập ngôn ngữ: ", ex);
            }
        }
        private void FillDataToControl()
        {
            try
            {
                WaitingManager.Show();
                int pageSize = ucPaging1.pagingGrid != null ? ucPaging1.pagingGrid.PageSize : ConfigApplicationWorker.Get<int>("CONFIG_KEY__NUM_PAGESIZE");
                LoadPaging(new CommonParam(0, pageSize));
                CommonParam param = new CommonParam();
                param.Limit = rowCount;
                param.Count = dataTotal;
                ucPaging1.Init(LoadPaging, param, pageSize, this.gridControl1);
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
                WaitingManager.Hide();
            }
        }

        private void LoadPaging(object param)
        {
            try
            {
                startPage = ((CommonParam)param).Start ?? 0;
                int limit = ((CommonParam)param).Limit ?? 0;
                CommonParam paramCommon = new CommonParam(startPage, limit);
                Inventec.Core.ApiResultObject<List<HIS_RECEIVING_REPORT>> apiResult = null;
                HisReceivingReportFilter filter = new HisReceivingReportFilter();
                filter.KEY_WORD = txtSearch.Text.Trim();
                filter.CREATE_TIME_FROM = Convert.ToInt64(dtCreateTimeFrom.DateTime.ToString("yyyyMMdd") + "000000");
                filter.CREATE_TIME_TO = Convert.ToInt64(dtCreateTimeTo.DateTime.ToString("yyyyMMdd") + "235959");
                filter.ORDER_DIRECTION = "DESC";
                filter.ORDER_FIELD = "CREATE_TIME";
                gridControl1.BeginUpdate();
                apiResult = new BackendAdapter(paramCommon).GetRO<List<HIS_RECEIVING_REPORT>>(
                    "/api/HisReceivingReport/Get", ApiConsumers.MosConsumer, filter, paramCommon);
                if (apiResult != null)
                {
                    receivingReports = (List<HIS_RECEIVING_REPORT>)apiResult.Data;
                    if (receivingReports != null)
                    {
                        gridControl1.DataSource = receivingReports;
                        rowCount = receivingReports.Count;
                        dataTotal = (apiResult.Param == null ? 0 : apiResult.Param.Count ?? 0);
                    }
                }
                gridControl1.EndUpdate();
                SessionManager.ProcessTokenLost(paramCommon);
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void txtSearch_EditValueChanged(object sender, EventArgs e)
        {

        }



        private void btnRefresh_Click(object sender, EventArgs e)
        {
            try
            {
                SetDefaultValue();
                FillDataToControl();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridView1_CustomRowCellEditForEditing(object sender, DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs e)
        {
            try
            {
                if (e.Column.FieldName == "btnViewDetail") // Check if click on view detail button
                {
                    int rowHandle = e.RowHandle;
                    if (rowHandle >= 0 && rowHandle < receivingReports?.Count)
                    {
                        var report = receivingReports[rowHandle];
                        if (report != null)
                        {
                            // Show detail popup form
                            using (var detailForm = new frmReceivingDetail(report))
                            {
                                detailForm.StartPosition = FormStartPosition.CenterParent;
                                detailForm.ShowDialog(this);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
                MessageBox.Show("Có lỗi xảy ra khi xem chi tiết báo cáo");
            }
        }

        private void gridView1_CustomUnboundColumnData(object sender, CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != DevExpress.Data.UnboundColumnType.Bound)
                {
                    HIS_RECEIVING_REPORT data = (HIS_RECEIVING_REPORT)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    short status = Inventec.Common.TypeConvert.Parse.ToInt16((data.IS_ACTIVE ?? -1).ToString());
                    if (e.Column.FieldName == "STT")
                    {
                        e.Value = e. ListSourceRowIndex + 1 + startPage;
                    }
                    else if (e.Column.FieldName == "BEGIN_DATE_STR")
                    {
                        e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(data.BEGIN_DATE ?? 0);
                    }
                    else if (e.Column.FieldName == "END_DATE_STR")
                    {
                        e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(data.END_DATE ?? 0);
                    }
                    else if (e.Column.FieldName == "CREATION_DATETIME_STR")
                    {
                        e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(data.CREATION_DATETIME ?? 0);
                    }
                    else if (e.Column.FieldName == "CREATE_TIME_STR")
                    {
                        e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(data.CREATE_TIME ?? 0);
                    }
                    else if (e.Column.FieldName == "MODIFY_TIME_STR")
                    {
                        e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(data.MODIFY_TIME ?? 0);
                    }
                    else if (e.Column.FieldName == "IS_ACTIVE_STR")
                    {
                        try
                        {
                            if (status == 1)
                                e.Value = "Hoạt động";
                            else
                                e.Value = "Tạm khóa";
                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Error(ex);
                        }
                    }
                    else if (e.Column.FieldName == "btnViewDetail")
                    {
                        e.Value = "Xem chi tiết";
                    }
                }    
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        private void gridView1_RowCellStyle(object sender, DevExpress.XtraGrid.Views.Grid.RowCellStyleEventArgs e)
        {
            DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
            if (e.RowHandle >= 0)
            {
                HIS_RECEIVING_REPORT data = (HIS_RECEIVING_REPORT)((IList)((BaseView)sender).DataSource)[e.RowHandle];
                if (e.Column.FieldName == "IS_ACTIVE_STR")
                {
                    if (data.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__FALSE)
                        e.Appearance.ForeColor = Color.Red;
                    else
                        e.Appearance.ForeColor = Color.Green;
                }
            }
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            try
            {
                FillDataToControl();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
