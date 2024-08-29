using DevExpress.Utils;
using HIS.Desktop.LocalStorage.ConfigApplication;
using Inventec.Common.Logging;
using Inventec.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.UC.ListReportType.Design.Template1
{
    internal partial class Template1
    {
        private void btnSearch_Click(object sender, EventArgs e)
        {
            try
            {
                Data.SearchData data = new Data.SearchData();
                SetSearchData(data);
                FillDataToGridControl(data);
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
                SetDefaultControl();
                Data.SearchData data = new Data.SearchData();
                SetSearchData(data);
                FillDataToGridControl(data);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void FillDataToGridControl(Data.SearchData data)
        {
            try
            {
                FillDataToGridControl(new CommonParam(0, (int)this.pageSize));
                SetInitPaging();
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        private void btnCreateReport_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                var reportType = gridViewReportType.GetFocusedRow();
                if (_createReport != null) _createReport(reportType);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetSearchData(Data.SearchData data)
        {
            try
            {
                if (!String.IsNullOrEmpty(txtKeyWord.Text))
                {
                    data.KeyWord = txtKeyWord.Text;
                }

                if (this.ReportTypeGroupId>0)
                {
                    data.ReportTypeGroupId = this.ReportTypeGroupId;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
