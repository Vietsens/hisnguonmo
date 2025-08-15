using DevExpress.XtraGrid.Views.Base;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Utility;
using Inventec.Common.Adapter;
using Inventec.Common.Logging;
using Inventec.Core;
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
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace HIS.Desktop.Plugins.HisReceivingReportList
{
    public partial class frmReceivingDetail : FormBase
    {
        #region Declare
        private HIS_RECEIVING_REPORT report;
        private List<HIS_RECEIVING_DETAIL> details;
        #endregion
        #region Constructor
        public frmReceivingDetail(HIS_RECEIVING_REPORT report) : base()
        {
            InitializeComponent();
            this.report = report;
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
        }
        #endregion

        private void frmReceivingDetail_Load(object sender, EventArgs e)
        {
            try
            {
                Show();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void Show()
        {
            LoadData();
            FillDataToControl();
        }
        #region Data Processing
        private void LoadData()
        {
            try
            {
                CommonParam param = new CommonParam();
                HisReceivingDetailFilter filter = new HisReceivingDetailFilter();
                filter.RECEIVING_REPORT_ID = report.ID;

                details = new BackendAdapter(param).Get<List<HIS_RECEIVING_DETAIL>>(
                    "/api/HisReceivingDetail/Get", ApiConsumers.MosConsumer, filter, param);

                if (details == null)
                {
                    details = new List<HIS_RECEIVING_DETAIL>();
                }
            }
            catch (Exception ex)
            {
                LogSystem.Error("Lỗi khi tải dữ liệu chi tiết: ", ex); 
            }
        }

        private void FillDataToControl()
        {
            try
            {
                gridControl1.BeginUpdate();
                gridControl1.DataSource = details;
                gridControl1.EndUpdate();
            }
            catch (Exception ex)
            {
                LogSystem.Error("Lỗi khi hiển thị dữ liệu lên grid: ", ex);
            }
        }
        #endregion

        private void grvReceivingDetail_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != DevExpress.Data.UnboundColumnType.Bound)
                {
                    HIS_RECEIVING_DETAIL data = (HIS_RECEIVING_DETAIL)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    short status = Inventec.Common.TypeConvert.Parse.ToInt16((data.IS_ACTIVE ?? -1).ToString());
                    if (e.Column.FieldName == "STT")
                    {
                        e.Value = e.ListSourceRowIndex + 1;
                    }
                    else if (e.Column.FieldName == "TRANS_DATETIME_STR")
                    {
                        e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(data.TRANS_DATETIME ?? 0);
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
                }
            }
            catch (Exception ex)
            {

                throw;
            }
        }
    }
}
