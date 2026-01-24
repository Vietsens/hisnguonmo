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
        private void LoadSereServGrid()
        {
            try
            {
                CommonParam paramCommon = new CommonParam();
                MOS.Filter.HisSereServFilter filter = new MOS.Filter.HisSereServFilter();
                //filter.TDL_SERVICE_TYPE_ID = IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__KH;
                filter.TDL_PATIENT_ID = currentSR.TDL_PATIENT_ID;
                filter.IS_ACTIVE = 1;
                var apiResult = new BackendAdapter(paramCommon).Get<List<SereServOptometristADO>>
                    (ApiConsumer.HisRequestUriStore.HIS_SERE_SERV_GET, ApiConsumer.ApiConsumers.MosConsumer, filter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, paramCommon);
                if (apiResult != null && apiResult.Count > 0)
                {
                    var sereServIds = apiResult.Select(o => o.ID).Distinct().ToList();

                    var paramViex = new CommonParam();
                    var filterViex = new MOS.Filter.HisSereServViexFilter();
                    filterViex.SERE_SERV_IDs = sereServIds;
                    filterViex.IS_ACTIVE = 1;

                    var allViex = new BackendAdapter(paramViex).Get<List<HIS_SERE_SERV_VIEX>>
                        (ApiConsumer.HisRequestUriStore.HIS_SERE_SERV_VIEX_GET, ApiConsumer.ApiConsumers.MosConsumer, filterViex, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, paramViex)
                        ?? new List<HIS_SERE_SERV_VIEX>();

                    var joined = apiResult
                        .GroupJoin(
                            allViex,
                            ss => ss.ID,
                            vx => vx.SERE_SERV_ID,
                            (ss, vxGroup) => new
                            {
                                SereServ = ss,
                                ViexList = vxGroup.OrderByDescending(o => o.ID).ToList(),
                                LatestViex = vxGroup.OrderByDescending(o => o.ID).FirstOrDefault()
                            })
                        .Select(x =>
                        {
                            x.SereServ.HIS_SERE_SERV_VIEX = x.ViexList;
                            if (x.LatestViex != null)
                            {
                                x.SereServ.VISION_TEST_TIME = x.LatestViex.VISION_TEST_TIME;
                                x.SereServ.VISION_TEST_ROOM_NAME = x.LatestViex.VISION_TEST_ROOM_NAME;
                                x.SereServ.VISION_TEST_NUM = x.LatestViex.VISION_TEST_NUM;
                            }
                            return x.SereServ;
                        })
                        .ToList();

                    apiResult = joined
                        .Where(w => (w.HIS_SERE_SERV_VIEX != null && w.HIS_SERE_SERV_VIEX.Count > 0)
                            || w.SERVICE_REQ_ID == currentSR.ID)
                        .OrderBy(o => o.SERVICE_REQ_ID == currentSR.ID && (o.HIS_SERE_SERV_VIEX == null || o.HIS_SERE_SERV_VIEX.Count == 0) ? 1 : 99)
                        .ThenByDescending(o => o.VISION_TEST_TIME)
                        .ToList();
                    gridControlSereServ.DataSource = null;
                    gridControlSereServ.DataSource = apiResult;
                    gridControlSereServ.RefreshDataSource();

                    int defaultIndex = apiResult.FindIndex(o => o.SERVICE_REQ_ID == currentSR.ID);
                    if (defaultIndex < 0) defaultIndex = 0;
                    int handle = gridViewSereServ.GetRowHandle(defaultIndex);
                    if (handle > 0)
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

        //private List<HIS_SERE_SERV_VIEX> FetchSereServViex(HIS_SERE_SERV sereServ)
        //{
        //    try
        //    {
        //        if (sereServ == null) return null;

        //        // Nếu item đã có list thì dùng luôn (chuẩn hóa về List)
        //        if (sereServ.HIS_SERE_SERV_VIEX != null && sereServ.HIS_SERE_SERV_VIEX.Count > 0)
        //        {
        //            return sereServ.HIS_SERE_SERV_VIEX
        //                .OrderByDescending(o => o.ID)
        //                .ToList();
        //        }

        //        CommonParam param = new CommonParam();
        //        MOS.Filter.HisSereServViexFilter filter = new MOS.Filter.HisSereServViexFilter();
        //        filter.SERE_SERV_IDs = new List<long> { sereServ.ID };
        //        filter.IS_ACTIVE = 1;
        //        var apiResult = new BackendAdapter(param).Get<List<HIS_SERE_SERV_VIEX>>
        //            (ApiConsumer.HisRequestUriStore.HIS_SERE_SERV_VIEX_GET, ApiConsumer.ApiConsumers.MosConsumer, filter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, param);

        //        return apiResult;
        //    }
        //    catch (Exception ex)
        //    {
        //        Inventec.Common.Logging.LogSystem.Error(ex);
        //    }
        //    return null;
        //}
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