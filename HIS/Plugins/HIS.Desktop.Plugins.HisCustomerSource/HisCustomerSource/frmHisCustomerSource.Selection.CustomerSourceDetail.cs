using DevExpress.Office.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Utilities.Extensions;
using Inventec.Common.Adapter;
using Inventec.Core;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HIS.Desktop.Plugins.HisCustomerSource.HisCustomerSource
{
    public partial class frmHisCustomerSource : HIS.Desktop.Utility.FormBase
    {
        private DevExpress.Utils.ToolTipController toolTipControllerRoomOptionItem;
        List<HIS_CUSTOMER_SOURCE_DT> SelectedOptions;
        List<HIS_CUSTOMER_SOURCE_DT> SourceDetailDataSource;
        private void InitComboDetail()
        {
            CommonParam commonParam = new CommonParam();
            HisCustomerSourceDtFilter filter = new HisCustomerSourceDtFilter();
            filter.IS_ACTIVE = 1;
            var data = new BackendAdapter(commonParam)
                .Get<List<HIS_CUSTOMER_SOURCE_DT>>(HisRequestUriStore.CustomerSourceDetail_GET, ApiConsumers.MosConsumer, filter, commonParam);
            this.SourceDetailDataSource = data;
            this.InitCombo(cboSourceDetail,
                SourceDetailDataSource,
                 nameof(HIS_CUSTOMER_SOURCE_DT.USERNAME),
                 nameof(HIS_CUSTOMER_SOURCE_DT.LOGINNAME),
                cboDepartment_MarksSelection,
                cboDepartment_CustomDisplayText,
                OnViewRowClick,
                OnViewRowStyle
                );
        }

        private void cboDepartment_MarksSelection(object sender, EventArgs e)
        {
            try
            {
                StringBuilder sb = new StringBuilder();
                GridCheckMarksSelection gridCheckMark = sender as GridCheckMarksSelection;
                if (gridCheckMark != null)
                {
                    List<HIS_CUSTOMER_SOURCE_DT> sgSelectedNews = new List<HIS_CUSTOMER_SOURCE_DT>();
                    foreach (HIS_CUSTOMER_SOURCE_DT rv in (gridCheckMark).Selection)
                    {
                        if (rv != null)
                        {
                            if (sb.ToString().Length > 0) { sb.Append(", "); }
                            sb.Append(rv.USERNAME.ToString());
                            sgSelectedNews.Add(rv);
                        }
                    }
                    this.SelectedOptions = new List<HIS_CUSTOMER_SOURCE_DT>();
                    this.SelectedOptions.AddRange(sgSelectedNews);
                    //this.cboDepartment.EditValue = sb.ToString();
                    this.cboSourceDetail.Text = sb.ToString();
                    //
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        protected void cboDepartment_CustomDisplayText(object sender, DevExpress.XtraEditors.Controls.CustomDisplayTextEventArgs e)
        {
            try
            {
                StringBuilder sb = new StringBuilder();
                GridCheckMarksSelection gridCheckMark = sender is GridLookUpEdit ? (sender as GridLookUpEdit).Properties.Tag as GridCheckMarksSelection : (sender as DevExpress.XtraEditors.Repository.RepositoryItemGridLookUpEdit).Tag as GridCheckMarksSelection;
                if (gridCheckMark == null || gridCheckMark.Selection == null || gridCheckMark.Selection.Count == 0)
                {
                    e.DisplayText = "";
                    return;
                }
                foreach (HIS_CUSTOMER_SOURCE_DT rv in gridCheckMark.Selection)
                {
                    if (sb.ToString().Length > 0) { sb.Append(", "); }

                    sb.Append(rv.USERNAME.ToString());
                }
                //if (SelectedOptions != null && SelectedOptions.Count == this.SourceDetailDataSource.Count)
                //{
                //    sb = new StringBuilder("Tất cả");
                //}
                e.DisplayText = sb.Length > 4000 ? sb.ToString().Substring(0, 4000) : sb.ToString();
                var g = sender as DevExpress.XtraEditors.GridLookUpEdit;
                g.Text = e.DisplayText;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void OnViewRowClick(object s, DevExpress.XtraGrid.Views.Grid.RowClickEventArgs e)
        {
            try
            {
                //var view = s as DevExpress.XtraGrid.Views.Grid.GridView;
                //if (view == null) return;
                //HIS_CUSTOMER_SOURCE_DT  row = view.GetRow(e.RowHandle) as HIS_CUSTOMER_SOURCE_DT;
                //if (row == null) return;
                //if (view.FocusedColumn.FieldName == "CheckMarkSelection")
                //{

                //}
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void OnViewRowStyle(object s, DevExpress.XtraGrid.Views.Grid.RowStyleEventArgs e)
        {
            try
            {
                var view = s as DevExpress.XtraGrid.Views.Grid.GridView;
                if (view == null) return;
                HIS_CUSTOMER_SOURCE_DT row = view.GetRow(e.RowHandle) as HIS_CUSTOMER_SOURCE_DT;
                if (row == null) return;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void ProcessSelectDepartment(string selectedItems)
        {
            try
            {
                GridCheckMarksSelection gridCheckMark = cboSourceDetail.Properties.Tag as GridCheckMarksSelection;
                if (gridCheckMark != null)
                {
                    cboClearSelection(cboSourceDetail);
                }
                if (!string.IsNullOrEmpty(selectedItems) && cboSourceDetail.Properties.Tag != null)
                {
                    List<HIS_CUSTOMER_SOURCE_DT> dataSourceItems = cboSourceDetail.Properties.DataSource as List<HIS_CUSTOMER_SOURCE_DT>;
                    List<HIS_CUSTOMER_SOURCE_DT> validSelectedItems = new List<HIS_CUSTOMER_SOURCE_DT>();
                    foreach (var item in selectedItems.Split(','))
                    {
                        var row = dataSourceItems != null ? dataSourceItems.FirstOrDefault(o => o.LOGINNAME == item) : null;
                        if (row != null)
                        {
                            validSelectedItems.Add(row);
                        }
                    }
                    gridCheckMark.SelectAll(validSelectedItems);
                    if (cboSourceDetail.Properties.Buttons.Count > 0)
                    {
                        foreach (EditorButton item in cboSourceDetail.Properties.Buttons)
                        {
                            if (item != null && item.Kind == ButtonPredefines.Delete)
                            {
                                item.Visible = true;
                            }
                        }
                    }
                }
                else
                {
                    cboSourceDetail.EditValue = null;
                    GridCheckMarksSelection gridCheckMarkBusinessCodes = cboSourceDetail.Properties.Tag as GridCheckMarksSelection;
                    if (gridCheckMarkBusinessCodes != null)
                    {
                        cboClearSelection(cboSourceDetail);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
