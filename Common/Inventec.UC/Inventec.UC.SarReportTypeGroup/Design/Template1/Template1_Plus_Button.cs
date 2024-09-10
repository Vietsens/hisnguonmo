using DevExpress.Utils;
using DevExpress.XtraGrid.Views.Base;
using HIS.Desktop.LocalStorage.ConfigApplication;
using Inventec.Common.Logging;
using Inventec.Core;
using SAR.EFMODEL.DataModels;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.UC.ListReportTypeGroup.Design.Template1
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
                //SetInitPaging();
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        private void gridViewReportTypeGroup_RowCellClick(object sender, DevExpress.XtraGrid.Views.Grid.RowCellClickEventArgs e)
        {
            try
            {
                DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                if (e.RowHandle >= 0)
                {

                    SAR_REPORT_TYPE_GROUP data = (SAR_REPORT_TYPE_GROUP)((IList)((BaseView)sender).DataSource)[e.RowHandle];
                    chkAllReportType.Checked = false;
                    if (_rowCellClick != null) _rowCellClick(data);

                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void chkAllReportType_Click(object sender, EventArgs e)
        {
            try
            {
                if (!chkAllReportType.Checked)
                {
                    if (_rowCellClick != null) _rowCellClick(new SAR_REPORT_TYPE_GROUP());
                }
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
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
