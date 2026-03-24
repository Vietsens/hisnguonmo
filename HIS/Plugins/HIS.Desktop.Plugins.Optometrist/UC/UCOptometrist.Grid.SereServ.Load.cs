using DevExpress.Data;
using DevExpress.XtraGrid.Views.Base;
using HIS.Desktop.Plugins.Optometrist.ADO;
using HIS.Desktop.Utility;
using Inventec.Common.Adapter;
using Inventec.Core;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace HIS.Desktop.Plugins.Optometrist.UC
{
    public partial class UCOptometrist : UserControlBase
    {
        private void LoadSereServViexGrid()
        {
            try
            {
                // Load V_HIS_SERE_SERV_VIEX
                var paramViex = new CommonParam();
                var filterViexView = new MOS.Filter.HisSereServViexViewFilter();
                filterViexView.TDL_PATIENT_ID = currentSR.TDL_PATIENT_ID;
                filterViexView.IS_ACTIVE = 1;
                var allViex = new BackendAdapter(paramViex).Get<List<V_HIS_SERE_SERV_VIEX>>
                    (ApiConsumer.HisRequestUriStore.HIS_SERE_SERV_VIEX_GETVIEW, ApiConsumer.ApiConsumers.MosConsumer, filterViexView, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, paramViex)
                    ?? new List<V_HIS_SERE_SERV_VIEX>();
                //Bổ sung dòng mới 
                if (!allViex.Any(a => a.SERVICE_REQ_ID == currentSR.ID))
                {
                    MOS.Filter.HisSereServFilter filter = new MOS.Filter.HisSereServFilter();
                    filter.SERVICE_REQ_ID = currentSR.ID;
                    filter.TDL_SERVICE_TYPE_ID = currentSR.SERVICE_REQ_TYPE_ID;
                    filter.IS_ACTIVE = 1;
                    var apiResult = new BackendAdapter(paramViex).Get<List<HIS_SERE_SERV>>
                        (ApiConsumer.HisRequestUriStore.HIS_SERE_SERV_GET, ApiConsumer.ApiConsumers.MosConsumer, filter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, paramViex);
                    var viex = new V_HIS_SERE_SERV_VIEX
                    {
                        SERVICE_REQ_ID = currentSR.ID,
                        TDL_SERVICE_NAME = apiResult[0].TDL_SERVICE_NAME,
                        SERE_SERV_ID = apiResult[0].ID,
                    };
                    allViex.Add(viex);
                }
                // Sắp xếp
                allViex = allViex
                    .OrderBy(o => o.SERVICE_REQ_ID == currentSR.ID ? 1 : 99)
                    .ThenByDescending(o => o.VISION_TEST_TIME)
                    .ToList();

                // Gán
                gridControlSereServ.DataSource = null;
                gridControlSereServ.DataSource = allViex;
                gridControlSereServ.RefreshDataSource();
                // Chọn dòng đang thao tác
                int defaultIndex = allViex.FindIndex(o => o.SERVICE_REQ_ID == currentSR.ID);
                if (defaultIndex < 0) defaultIndex = 0;
                int handle = gridViewSereServ.GetRowHandle(defaultIndex);
                if (handle > 0)
                {
                    gridViewSereServ.FocusedRowHandle = handle;
                    BindSelectedSereServ(allViex[defaultIndex]);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewSereServ_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)
                {
                    V_HIS_SERE_SERV_VIEX dataRow = (V_HIS_SERE_SERV_VIEX)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    if (dataRow == null) return;

                    if (e.Column.FieldName == "VISION_TEST_TIME_STR" && dataRow.VISION_TEST_TIME.HasValue)
                    {
                        e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(dataRow.VISION_TEST_TIME.Value);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewSereServ_RowStyle(object sender, DevExpress.XtraGrid.Views.Grid.RowStyleEventArgs e)
        {
            try
            {
                //if (e.RowHandle >= 0)
                //{
                //    SereServOptometristADO sereServ = (SereServOptometristADO)gridViewSereServ.GetRow(e.RowHandle);
                //    if (sereServ != null)
                //    {
                //        if (sereServ.IS_NO_EXECUTE == 1)
                //        {
                //            e.Appearance.Font = new System.Drawing.Font(e.Appearance.Font, System.Drawing.FontStyle.Strikeout);
                //        }
                //    }
                //}
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

    }
}