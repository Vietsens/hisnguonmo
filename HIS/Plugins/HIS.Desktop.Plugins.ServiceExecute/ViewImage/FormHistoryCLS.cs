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
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.Plugins.ServiceExecute.ADO;
using HIS.Desktop.Plugins.ServiceExecute.Resources;
using HIS.Desktop.Utility;
using Inventec.Common.Adapter;
using Inventec.Core;
using Inventec.Desktop.Common.LanguageManager;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.ServiceExecute.ViewImage
{
    public partial class FormHistoryCLS : FormBase
    {
        HIS_SERVICE_REQ ServiceReq;
        Inventec.Desktop.Common.Modules.Module ModuleData;

        public FormHistoryCLS()
        {
            InitializeComponent();
        }

        public FormHistoryCLS(Inventec.Desktop.Common.Modules.Module moduleData, HIS_SERVICE_REQ serviceReq)
            : base(moduleData)
        {
            InitializeComponent();
            try
            {
                this.ServiceReq = serviceReq;
                this.ModuleData = moduleData;
                var type = BackendDataWorker.Get<HIS_SERVICE_REQ_TYPE>().FirstOrDefault(o => o.ID == serviceReq.SERVICE_REQ_TYPE_ID);
                if (type != null)
                {
                    this.Text = string.Format("{0} {1}", ResourceLanguageManager.GetValue("HIS.Desktop.Plugins.ServiceExecute.FormHistoryCLS.Text"), type.SERVICE_REQ_TYPE_NAME);
                }
                else
                {
                    this.Text = ResourceLanguageManager.GetValue("HIS.Desktop.Plugins.ServiceExecute.FormHistoryCLS.Text");
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void FormHistoryCLS_Load(object sender, EventArgs e)
        {
            try
            {
                this.SetCaptionByLanguageKey();
                ProcessDataHistory();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ProcessDataHistory()
        {
            try
            {
                CommonParam param = new CommonParam();
                List<SereServHistoryADO> listSereServ = new List<SereServHistoryADO>();

                HisSereServView1Filter ssFilter = new HisSereServView1Filter();
                ssFilter.TDL_PATIENT_ID = ServiceReq.TDL_PATIENT_ID;
                ssFilter.HAS_EXECUTE = true;
                ssFilter.SERVICE_REQ_STT_IDs = new List<long>() { IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_STT.ID__HT };
                ssFilter.SERVICE_TYPE_ID = GetServiceTypeByReqType(ServiceReq.SERVICE_REQ_TYPE_ID);

                var sereServ = new BackendAdapter(param).Get<List<V_HIS_SERE_SERV_1>>("/api/HisSereServ/GetView1", ApiConsumer.ApiConsumers.MosConsumer, ssFilter, param);
                if (sereServ != null && sereServ.Count > 0)
                {
                    listSereServ.AddRange((from s in sereServ select new SereServHistoryADO(s)).ToList());
                    //int skip = 0;
                    //while (sereServ.Count - skip > 0)
                    //{
                    //    var listid = sereServ.Skip(skip).Take(100).ToList();
                    //    skip += 100;

                    //    HisSereServExtFilter extFilter = new HisSereServExtFilter();
                    //    extFilter.SERE_SERV_IDs = listid.Select(s => s.ID).ToList();
                    //    var sereServExt = new BackendAdapter(param).Get<List<HIS_SERE_SERV_EXT>>("/api/HisSereServExt/Get", ApiConsumer.ApiConsumers.MosConsumer, extFilter, param);
                    //    if (sereServExt != null && sereServExt.Count > 0)
                    //    {
                    //        foreach (var item in sereServExt)
                    //        {
                    //            var ss = listid.FirstOrDefault(o => o.ID == item.SERE_SERV_ID);
                    //            listSereServ.Add(new SereServHistoryADO(ss, item));
                    //        }
                    //    }
                    //}
                }

                listSereServ = listSereServ.OrderByDescending(o => o.TIME_END).ThenBy(o => o.TDL_SERVICE_CODE).ToList();

                gridControlSereServHistory.BeginUpdate();
                gridControlSereServHistory.DataSource = null;
                gridControlSereServHistory.DataSource = listSereServ;
                gridControlSereServHistory.EndUpdate();
                gridViewSereServHistory.ExpandAllGroups();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private long? GetServiceTypeByReqType(long service_req_type_id)
        {
            long? result = null;
            try
            {
                switch (service_req_type_id)
                {
                    case IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__AN:
                        result = IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__AN;
                        break;
                    case IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__CDHA:
                        result = IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__CDHA;
                        break;
                    case IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__GPBL:
                        result = IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__GPBL;
                        break;
                    case IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__KH:
                        result = IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__KH;
                        break;
                    case IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__KHAC:
                        result = IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__KHAC;
                        break;
                    case IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__NS:
                        result = IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__NS;
                        break;
                    case IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__PHCN:
                        result = IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__PHCN;
                        break;
                    case IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__PT:
                        result = IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__PT;
                        break;
                    case IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__SA:
                        result = IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__SA;
                        break;
                    case IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__TDCN:
                        result = IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__TDCN;
                        break;
                    case IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__TT:
                        result = IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__TT;
                        break;
                    case IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__XN:
                        result = IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__XN;
                        break;
                    default:
                        result = -1;
                        break;
                }
            }
            catch (Exception ex)
            {
                result = null;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        private void gridViewSereServHistory_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != DevExpress.Data.UnboundColumnType.Bound)
                {
                    var data = (SereServHistoryADO)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    if (data != null)
                    {
                        if (e.Column.FieldName == "END_TIME_STR")
                        {
                            try
                            {
                                if (data.HisSereServExt != null && data.HisSereServExt.END_TIME.HasValue)
                                {
                                    e.Value = Inventec.Common.DateTime.Convert.TimeNumberToDateString(data.HisSereServExt.END_TIME.Value);
                                }
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

        private void gridViewSereServHistory_RowClick(object sender, DevExpress.XtraGrid.Views.Grid.RowClickEventArgs e)
        {

        }

        private void repositoryItemButtonView_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                var sereServRow = (SereServHistoryADO)gridViewSereServHistory.GetFocusedRow();
                CallModulView(sereServRow);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void CallModulView(SereServHistoryADO data)
        {
            try
            {
                if (data != null)
                {
                    List<object> sendObj = new List<object>();
                    sendObj.Add(data.ID);
                    HIS.Desktop.ModuleExt.PluginInstanceBehavior.ShowModule("HIS.Desktop.Plugins.ServiceReqResultView", ModuleData.RoomId, ModuleData.RoomTypeId, sendObj);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewSereServHistory_CustomDrawGroupRow(object sender, RowObjectCustomDrawEventArgs e)
        {
            try
            {
                GridView view = sender as GridView;
                GridGroupRowInfo info = e.Info as GridGroupRowInfo;
                if (info.Column.FieldName == "TIME_END")
                {
                    long time = Convert.ToInt64(view.GetGroupRowValue(e.RowHandle, info.Column));
                    info.GroupText = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(time);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>
        ///Hàm xét ngôn ngữ cho giao diện FormHistoryCLS
        /// </summary>
        private void SetCaptionByLanguageKey()
        {
            try
            {
                ////Khoi tao doi tuong resource
                Resources.ResourceLanguageManager.LanguageResourceFormHistoryCLS = new ResourceManager("HIS.Desktop.Plugins.ServiceExecute.Resources.Lang", typeof(FormHistoryCLS).Assembly);

                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.layoutControl1.Text = Inventec.Common.Resource.Get.Value("FormHistoryCLS.layoutControl1.Text", Resources.ResourceLanguageManager.LanguageResourceFormHistoryCLS, LanguageManager.GetCulture());
                this.Gc_Vỉew.Caption = Inventec.Common.Resource.Get.Value("FormHistoryCLS.Gc_Vỉew.Caption", Resources.ResourceLanguageManager.LanguageResourceFormHistoryCLS, LanguageManager.GetCulture());
                this.Gc_ServiceName.Caption = Inventec.Common.Resource.Get.Value("FormHistoryCLS.Gc_ServiceName.Caption", Resources.ResourceLanguageManager.LanguageResourceFormHistoryCLS, LanguageManager.GetCulture());
                this.Gc_IntructionTime.Caption = Inventec.Common.Resource.Get.Value("FormHistoryCLS.Gc_IntructionTime.Caption", Resources.ResourceLanguageManager.LanguageResourceFormHistoryCLS, LanguageManager.GetCulture());
                this.Gc_EndTime.Caption = Inventec.Common.Resource.Get.Value("FormHistoryCLS.Gc_EndTime.Caption", Resources.ResourceLanguageManager.LanguageResourceFormHistoryCLS, LanguageManager.GetCulture());
                this.Text = Inventec.Common.Resource.Get.Value("FormHistoryCLS.Text", Resources.ResourceLanguageManager.LanguageResourceFormHistoryCLS, LanguageManager.GetCulture());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public override void ProcessDisposeModuleDataAfterClose()
        {
            try
            {
                ModuleData = null;
                ServiceReq = null;
                this.gridViewSereServHistory.CustomDrawGroupRow -= new DevExpress.XtraGrid.Views.Base.RowObjectCustomDrawEventHandler(this.gridViewSereServHistory_CustomDrawGroupRow);
                this.gridViewSereServHistory.CustomUnboundColumnData -= new DevExpress.XtraGrid.Views.Base.CustomColumnDataEventHandler(this.gridViewSereServHistory_CustomUnboundColumnData);
                this.repositoryItemButtonView.ButtonClick -= new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.repositoryItemButtonView_ButtonClick);
                this.Load -= new System.EventHandler(this.FormHistoryCLS_Load);
                gridViewSereServHistory.GridControl.DataSource = null;
                gridControlSereServHistory.DataSource = null;
                Gc_EndTime = null;
                repositoryItemButtonView = null;
                layoutControlItem1 = null;
                Gc_IntructionTime = null;
                Gc_ServiceName = null;
                gridViewSereServHistory = null;
                gridControlSereServHistory = null;
                layoutControlGroup1 = null;
                layoutControl1 = null;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
