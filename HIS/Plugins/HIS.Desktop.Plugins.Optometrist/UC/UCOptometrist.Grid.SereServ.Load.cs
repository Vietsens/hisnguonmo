using DevExpress.Data;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraGrid.Views.Base;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Plugins.Library.EmrGenerate;
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
        private HIS_SERVICE_REQ GetServiceReq()
        {
            try
            {
                var currentsereServ = GetSelectedSereServ();
                if (currentsereServ != null)
                {
                    CommonParam param = new CommonParam();
                    MOS.Filter.HisServiceReqFilter filter = new MOS.Filter.HisServiceReqFilter();
                    filter.ID = currentsereServ.SERVICE_REQ_ID;
                    filter.IS_ACTIVE = 1;
                    var apiResult = new BackendAdapter(param).Get<List<HIS_SERVICE_REQ>>
                        (ApiConsumer.HisRequestUriStore.HIS_SERVICE_REQ_GET_, ApiConsumer.ApiConsumers.MosConsumer, filter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, param);
                    if (apiResult != null && apiResult.Count > 0)
                    {
                        return apiResult.First();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return null;
        }
        private void LoadSereServGrid()
        {
            try
            {
                CommonParam paramCommon = new CommonParam();
                MOS.Filter.HisSereServFilter filter = new MOS.Filter.HisSereServFilter();
                filter.TDL_SERVICE_REQ_TYPE_ID = currentsereServ.TDL_SERVICE_REQ_TYPE_ID;
                filter.TDL_PATIENT_ID = currentsereServ.TDL_PATIENT_ID;
                filter.IS_ACTIVE = 1;
                var apiResult = new BackendAdapter(paramCommon).Get<List<SereServOptometristADO>>
                    (ApiConsumer.HisRequestUriStore.HIS_SERE_SERV_GET, ApiConsumer.ApiConsumers.MosConsumer, filter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, paramCommon);
                if (apiResult != null && apiResult.Count > 0)
                {
                    foreach (var item in apiResult)
                    {
                        if (item.HIS_SERE_SERV_VIEX == null || item.HIS_SERE_SERV_VIEX.Count == 0)
                        {
                            item.HIS_SERE_SERV_VIEX = FetchSereServViex(item);
                        }

                        if (item.HIS_SERE_SERV_VIEX != null && item.HIS_SERE_SERV_VIEX.Count > 0)
                        {
                            var viex = item.HIS_SERE_SERV_VIEX
                                .OrderByDescending(o => o.ID)
                                .FirstOrDefault();

                            if (viex != null)
                            {
                                item.VISION_TEST_TIME = viex.VISION_TEST_TIME;
                                item.VISION_TEST_ROOM_NAME = viex.VISION_TEST_ROOM_NAME;
                                item.VISION_TEST_NUM = viex.VISION_TEST_NUM;
                            }
                        }
                    }
                    apiResult = apiResult.OrderByDescending(o => o.VISION_TEST_TIME).ToList();
                    gridControlSereServ.DataSource = apiResult;
                    gridControlSereServ.RefreshDataSource();

                    int defaultIndex = apiResult.FindIndex(o => o.ID == currentsereServ.ID);
                    if (defaultIndex < 0) defaultIndex = 0;
                    int handle = gridViewSereServ.GetRowHandle(defaultIndex);
                    if (handle >= 0)
                    {
                        gridViewSereServ.FocusedRowHandle = handle;
                        BindSelectedSereServ(apiResult[defaultIndex]);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private List<HIS_SERE_SERV_VIEX> FetchSereServViex(HIS_SERE_SERV sereServ)
        {
            try
            {
                if (sereServ == null) return null;

                // Nếu item đã có list thì dùng luôn (chuẩn hóa về List)
                if (sereServ.HIS_SERE_SERV_VIEX != null && sereServ.HIS_SERE_SERV_VIEX.Count > 0)
                {
                    return sereServ.HIS_SERE_SERV_VIEX
                        .OrderByDescending(o => o.ID)
                        .ToList();
                }

                CommonParam param = new CommonParam();
                MOS.Filter.HisSereServViexFilter filter = new MOS.Filter.HisSereServViexFilter();
                filter.TDL_TREATMENT_ID = sereServ.TDL_TREATMENT_ID;
                filter.SERE_SERV_ID = sereServ.ID;
                filter.IS_ACTIVE = 1;

                var apiResult = new BackendAdapter(param).Get<List<HIS_SERE_SERV_VIEX>>
                    (ApiConsumer.HisRequestUriStore.HIS_SERE_SERV_VIEX_GET, ApiConsumer.ApiConsumers.MosConsumer, filter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, param);

                return apiResult?
                    .OrderByDescending(o => o.ID)
                    .ToList();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return null;
        }
        private void gridViewSereServ_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)
                {
                    SereServOptometristADO dataRow = (SereServOptometristADO)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    if (dataRow == null) return;

                    if (e.Column.FieldName == "VISION_TEST_TIME_STR" && dataRow.VISION_TEST_TIME.HasValue)
                    {
                        e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(dataRow.CREATE_TIME ?? 0);
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
                if (e.RowHandle >= 0)
                {
                    SereServOptometristADO sereServ = (SereServOptometristADO)gridViewSereServ.GetRow(e.RowHandle);
                    if (sereServ != null)
                    {
                        if (sereServ.IS_NO_EXECUTE == 1)
                        {
                            e.Appearance.Font = new System.Drawing.Font(e.Appearance.Font, System.Drawing.FontStyle.Strikeout);
                        }
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