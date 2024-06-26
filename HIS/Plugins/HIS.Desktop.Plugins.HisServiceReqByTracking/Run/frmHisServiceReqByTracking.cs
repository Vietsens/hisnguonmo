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
using HIS.Desktop.ApiConsumer;
using Inventec.Common.Adapter;
using Inventec.Core;
using Inventec.Desktop.Common.LanguageManager;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
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

namespace HIS.Desktop.Plugins.HisServiceReqByTracking.Run
{
    public partial class frmHisServiceReqByTracking : HIS.Desktop.Utility.FormBase
    {
        internal Inventec.Desktop.Common.Modules.Module currentModule;
        long treamentId = 0;
        HIS_TRACKING currentTracking { get; set; }
        List<long> serviceReqIds = null;
        internal List<V_HIS_SERVICE_REQ_2> lstServiceReqByTracking { get; set; }
        long trackingId = 0;

        public frmHisServiceReqByTracking()
        {
            InitializeComponent();
        }

        public frmHisServiceReqByTracking(Inventec.Desktop.Common.Modules.Module currentModule, long trackingId)
		:base(currentModule)
        {
            InitializeComponent();
            try
            {
                this.currentModule = currentModule;
                this.trackingId = trackingId;
                LoadHisTrackingByTrackingId();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void frmHisServiceReqByTracking_Load(object sender, EventArgs e)
        {
            try
            {
                SetIconFrm();
                SetCaptionByLanguageKey();
                if (this.currentModule != null)
                {
                    this.Text = this.currentModule.text;
                }
                LoadDataServiceReq();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        void SetIconFrm()
        {
            try
            {
                string iconPath = System.IO.Path.Combine(HIS.Desktop.LocalStorage.Location.ApplicationStoreLocation.ApplicationStartupPath, System.Configuration.ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]);
                this.Icon = Icon.ExtractAssociatedIcon(iconPath);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetCaptionByLanguageKey()
        {
            try
            {
                ////Khoi tao doi tuong resource
                Resources.ResourceLanguageManager.LanguageResource = new ResourceManager("HIS.Desktop.Plugins.HisServiceReqByTracking.Resources.Lang", typeof(HIS.Desktop.Plugins.HisServiceReqByTracking.Run.frmHisServiceReqByTracking).Assembly);

                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.layoutControl1.Text = Inventec.Common.Resource.Get.Value("frmHisServiceReqByTracking.layoutControl1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnSave.Text = Inventec.Common.Resource.Get.Value("frmHisServiceReqByTracking.btnSave.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn1.Caption = Inventec.Common.Resource.Get.Value("frmHisServiceReqByTracking.gridColumn1.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn2.Caption = Inventec.Common.Resource.Get.Value("frmHisServiceReqByTracking.gridColumn2.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn3.Caption = Inventec.Common.Resource.Get.Value("frmHisServiceReqByTracking.gridColumn3.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn4.Caption = Inventec.Common.Resource.Get.Value("frmHisServiceReqByTracking.gridColumn4.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn5.Caption = Inventec.Common.Resource.Get.Value("frmHisServiceReqByTracking.gridColumn5.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn6.Caption = Inventec.Common.Resource.Get.Value("frmHisServiceReqByTracking.gridColumn6.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn7.Caption = Inventec.Common.Resource.Get.Value("frmHisServiceReqByTracking.gridColumn7.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn8.Caption = Inventec.Common.Resource.Get.Value("frmHisServiceReqByTracking.gridColumn8.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn9.Caption = Inventec.Common.Resource.Get.Value("frmHisServiceReqByTracking.gridColumn9.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn10.Caption = Inventec.Common.Resource.Get.Value("frmHisServiceReqByTracking.gridColumn10.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn11.Caption = Inventec.Common.Resource.Get.Value("frmHisServiceReqByTracking.gridColumn11.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bar1.Text = Inventec.Common.Resource.Get.Value("frmHisServiceReqByTracking.bar1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.barButton__Save.Caption = Inventec.Common.Resource.Get.Value("frmHisServiceReqByTracking.barButton__Save.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.Text = Inventec.Common.Resource.Get.Value("frmHisServiceReqByTracking.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }


        private void LoadHisTrackingByTrackingId()
        {
            try
            {
                if (this.trackingId > 0)
                {
                    CommonParam param = new CommonParam();
                    MOS.Filter.HisTrackingFilter trackingFilter = new MOS.Filter.HisTrackingFilter();
                    trackingFilter.ID = this.trackingId;
                    currentTracking = new HIS_TRACKING();
                    currentTracking = new BackendAdapter(param).Get<List<HIS_TRACKING>>(HisRequestUriStore.HIS_TRACKING_GET
                        , ApiConsumers.MosConsumer, trackingFilter, param).FirstOrDefault();
                    if (currentTracking != null)
                    {
                        this.treamentId = currentTracking.TREATMENT_ID;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadDataServiceReq()
        {
            try
            {
                CommonParam param = new CommonParam();
                WaitingManager.Show();

                MOS.Filter.HisServiceReqView2Filter serviceReqFilter = new MOS.Filter.HisServiceReqView2Filter();
                serviceReqFilter.TREATMENT_ID = this.treamentId;

                //DateTime toDay = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(this.currentTracking.TRACKING_TIME) ?? DateTime.Now;
                //DateTime newDay = toDay.AddDays(1);
                //long nextTime = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(newDay) ?? 0;

                var currentServiceReq = new BackendAdapter(param).Get<List<V_HIS_SERVICE_REQ_2>>(HisRequestUriStore.HIS_SERVICE_REQ_GETVIEW_2, ApiConsumers.MosConsumer, serviceReqFilter, param).Where(p =>
                    p.TRACKING_ID == this.trackingId
                    || (p.TRACKING_ID == null
                    && p.SERVICE_REQ_TYPE_ID != IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__KH)).OrderByDescending(p => p.TRACKING_ID).ThenByDescending(p => p.INTRUCTION_TIME).ToList();

                lstServiceReqByTracking = new List<V_HIS_SERVICE_REQ_2>();//các serviceReq of tracking
                lstServiceReqByTracking = currentServiceReq.Where(p => p.TRACKING_ID == this.trackingId).ToList();

                gridControlServiceReq.DataSource = null;
                gridControlServiceReq.DataSource = currentServiceReq;
                gridViewServiceReq.BestFitColumns();

                for (int i = 0; i < currentServiceReq.Count; i++)
                {
                    foreach (var item in lstServiceReqByTracking)
                    {
                        if (currentServiceReq[i].ID == item.ID)
                        {
                            gridViewServiceReq.SelectRow(i);
                        }
                    }
                }
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewServiceReq_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != DevExpress.Data.UnboundColumnType.Bound)
                {
                    var data = (V_HIS_SERVICE_REQ_2)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    if (data != null)
                    {
                        if (e.Column.FieldName == "STT")
                        {
                            try
                            {
                                e.Value = e.ListSourceRowIndex + 1;
                            }
                            catch (Exception ex)
                            {
                                Inventec.Common.Logging.LogSystem.Error(ex);
                            }
                        }
                        else if (e.Column.FieldName == "REQUEST_USERNAME_DISPLAY")
                        {
                            try
                            {
                                e.Value = data.REQUEST_LOGINNAME + (String.IsNullOrEmpty(data.REQUEST_USERNAME) ? "" : " - " + data.REQUEST_USERNAME);
                            }
                            catch (Exception ex)
                            {
                                Inventec.Common.Logging.LogSystem.Error(ex);
                            }
                        }
                        else if (e.Column.FieldName == "INTRUCTION_TIME_STR")
                        {
                            try
                            {
                                e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(data.INTRUCTION_TIME);
                            }
                            catch (Exception ex)
                            {
                                Inventec.Common.Logging.LogSystem.Error(ex);
                            }
                        }
                        else if (e.Column.FieldName == "CREATE_TIME_STR")
                        {
                            try
                            {
                                e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(data.CREATE_TIME ?? 0);
                            }
                            catch (Exception ex)
                            {
                                Inventec.Common.Logging.LogSystem.Error(ex);
                            }
                        }
                        else if (e.Column.FieldName == "MODIFY_TIME_STR")
                        {
                            try
                            {
                                e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(data.MODIFY_TIME ?? 0);
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

        private void gridViewServiceReq_SelectionChanged(object sender, DevExpress.Data.SelectionChangedEventArgs e)
        {
            //if (gridViewServiceReq.RowCount > 0)
            //{
            //    serviceReqIds = new List<long>();
            //    for (int i = 0; i < gridViewServiceReq.SelectedRowsCount; i++)
            //    {
            //        if (gridViewServiceReq.GetSelectedRows()[i] >= 0)
            //        {
            //            serviceReqIds.Add(Inventec.Common.TypeConvert.Parse.ToInt64((gridViewServiceReq.GetRowCellValue(i, "ID") ?? "").ToString()));
            //        }
            //    }
            //}
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                CommonParam param = new CommonParam();
                bool success = false;

                if (gridViewServiceReq.RowCount > 0)
                {
                    serviceReqIds = new List<long>();
                    for (int i = 0; i < gridViewServiceReq.SelectedRowsCount; i++)
                    {
                        if (gridViewServiceReq.GetSelectedRows()[i] >= 0)
                        {
                            serviceReqIds.Add(Inventec.Common.TypeConvert.Parse.ToInt64((gridViewServiceReq.GetRowCellValue(gridViewServiceReq.GetSelectedRows()[i], "ID") ?? "").ToString()));
                        }
                    }
                }

                MOS.SDO.HisTrackingSDO adoUpdate = new MOS.SDO.HisTrackingSDO();
                adoUpdate.ServiceReqIds = serviceReqIds;

                adoUpdate.Tracking = this.currentTracking;

                MOS.Filter.HisDhstFilter dhstFilter = new MOS.Filter.HisDhstFilter();
                dhstFilter.TRACKING_ID = this.currentTracking.ID;

                adoUpdate.Dhst = new BackendAdapter(param).Get<List<HIS_DHST>>(HisRequestUriStore.HIS_DHST_GET, ApiConsumers.MosConsumer, dhstFilter, param).FirstOrDefault();

                var rs = new BackendAdapter(param).Post<HIS_TRACKING>(HisRequestUriStore.HIS_TRACKING_UPDATE, ApiConsumers.MosConsumer, adoUpdate, param);
                if (rs != null)
                {
                    success = true;
                    LoadDataServiceReq();
                }
                MessageManager.Show(this.ParentForm, param, success);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewServiceReq_RowStyle(object sender, DevExpress.XtraGrid.Views.Grid.RowStyleEventArgs e)
        {
            try
            {
                if (e.RowHandle >= 0)
                {
                    if (lstServiceReqByTracking != null && lstServiceReqByTracking.Count > 0)
                    {
                        var data = (V_HIS_SERVICE_REQ_2)gridViewServiceReq.GetRow(e.RowHandle);
                        if (data == null)
                            return;
                        foreach (var item in lstServiceReqByTracking)
                        {
                            if (item.ID == data.ID)
                            {
                                e.Appearance.ForeColor = Color.Blue;
                                e.HighPriority = true;
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

        private void barButton__Save_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                btnSave_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
