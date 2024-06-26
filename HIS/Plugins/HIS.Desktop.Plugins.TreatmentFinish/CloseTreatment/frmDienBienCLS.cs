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
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using HIS.Desktop.Common;
using Inventec.Common.Adapter;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using System.Collections;
using DevExpress.XtraGrid.Views.Base;
using System.Resources;
using Inventec.Desktop.Common.LanguageManager;

namespace HIS.Desktop.Plugins.TreatmentFinish.CloseTreatment
{
    public partial class frmDienBienCLS : Form
    {
        #region Delare
        DelegateSelectData delegateData;
        HIS_TREATMENT currentTreatment;
        Inventec.Desktop.Common.Modules.Module module;
        List<HIS_EXECUTE_ROOM> lstHisExecuteRoom;
        List<HIS_DEPARTMENT> lstHisDepartment;
        #endregion
        #region ConStructor Load
        public frmDienBienCLS(Inventec.Desktop.Common.Modules.Module _Module, DelegateSelectData ds, HIS_TREATMENT hisTreatment)
        {
            InitializeComponent();
            try
            {
                this.module = _Module;
                this.delegateData = ds;
                this.currentTreatment = hisTreatment;
                SetIcon();

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SetIcon()
        {
            try
            {
                this.Icon = Icon.ExtractAssociatedIcon(System.IO.Path.Combine(LocalStorage.Location.ApplicationStoreLocation.ApplicationDirectory, System.Configuration.ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void frmDienBienCLS_Load(object sender, EventArgs e)
        {
            try
            {
                SetCaptionByLanguageKey();
                FillDataToTabCongKham();
                FillDataToTabToDieuTri();

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void SetCaptionByLanguageKey()
        {
            try
            {
                ////Khoi tao doi tuong resource
                Resources.ResourceLanguageManager.LanguageResource = new ResourceManager("HIS.Desktop.Plugins.TreatmentFinish.Resources.Lang", typeof(frmDienBienCLS).Assembly);

                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.layoutControl1.Text = Inventec.Common.Resource.Get.Value("frmDienBienCLS.layoutControl1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl2.Text = Inventec.Common.Resource.Get.Value("frmDienBienCLS.layoutControl2.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnSave.Text = Inventec.Common.Resource.Get.Value("frmDienBienCLS.btnSave.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.xtraTabPage1.Text = Inventec.Common.Resource.Get.Value("frmDienBienCLS.xtraTabPage1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl3.Text = Inventec.Common.Resource.Get.Value("frmDienBienCLS.layoutControl3.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn1.Caption = Inventec.Common.Resource.Get.Value("frmDienBienCLS.gridColumn1.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn2.Caption = Inventec.Common.Resource.Get.Value("frmDienBienCLS.gridColumn2.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn3.Caption = Inventec.Common.Resource.Get.Value("frmDienBienCLS.gridColumn3.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bar1.Text = Inventec.Common.Resource.Get.Value("frmDienBienCLS.bar1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.barButtonItem1.Caption = Inventec.Common.Resource.Get.Value("frmDienBienCLS.barButtonItem1.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.xtraTabPage2.Text = Inventec.Common.Resource.Get.Value("frmDienBienCLS.xtraTabPage2.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl4.Text = Inventec.Common.Resource.Get.Value("frmDienBienCLS.layoutControl4.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn4.Caption = Inventec.Common.Resource.Get.Value("frmDienBienCLS.gridColumn4.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn5.Caption = Inventec.Common.Resource.Get.Value("frmDienBienCLS.gridColumn5.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.Text = Inventec.Common.Resource.Get.Value("frmDienBienCLS.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        #endregion
        #region FilDataGridTab
        private void FillDataToTabCongKham()
        {
            try
            {
                CommonParam param = new CommonParam();
                HisServiceReqFilter filter = new HisServiceReqFilter();
                filter.SERVICE_REQ_TYPE_ID = IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__KH;
                filter.TREATMENT_ID = currentTreatment.ID;
                var data = new BackendAdapter(param).Get<List<HIS_SERVICE_REQ>>("api/HisServiceReq/Get", ApiConsumer.ApiConsumers.MosConsumer, filter, param);
                gridControl1.BeginUpdate();
                if (data != null)
                {

                    gridControl1.DataSource = data.OrderByDescending(o=>o.INTRUCTION_TIME);
                    HisExecuteRoomFilter fil = new HisExecuteRoomFilter();
                    fil.ROOM_IDs = data.Select(o => o.EXECUTE_ROOM_ID).ToList();
                    lstHisExecuteRoom = new BackendAdapter(param).Get<List<HIS_EXECUTE_ROOM>>("api/HisExecuteRoom/Get", ApiConsumer.ApiConsumers.MosConsumer, fil, param);
                }
                else
                {
                    gridControl1.DataSource = null;
                }
                gridControl1.EndUpdate();

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void FillDataToTabToDieuTri()
        {
            try
            {
                CommonParam param = new CommonParam();
                HisTrackingFilter filter = new HisTrackingFilter();
                 filter.TREATMENT_ID = currentTreatment.ID;
                var data = new BackendAdapter(param).Get<List<HIS_TRACKING>>("api/HisTracking/Get", ApiConsumer.ApiConsumers.MosConsumer, filter, param);
                gridControl2.BeginUpdate();
                if (data != null)
                {
                    gridControl2.DataSource = data.OrderByDescending(o => o.TRACKING_TIME);
                    if (data.Count > 0)
                    {
                        xtraTabControl1.SelectedTabPage = xtraTabPage2;
                        gridView2.SelectRow(0);
                    }              
                    HisDepartmentFilter fil = new HisDepartmentFilter();
                    fil.IDs = data.Select(o => (long) o.DEPARTMENT_ID).ToList();
                    lstHisDepartment = new BackendAdapter(param).Get<List<HIS_DEPARTMENT>>("api/HisDepartment/Get", ApiConsumer.ApiConsumers.MosConsumer, fil, param);      
                }
                else
                {
                    gridControl2.DataSource = null;
                }
                gridControl2.EndUpdate();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridView1_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != DevExpress.Data.UnboundColumnType.Bound)
                {

                    var data = (HIS_SERVICE_REQ)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    if (data != null)
                    {
                        if (e.Column.FieldName == "INTRUCTION_TIME_STR")
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(data.INTRUCTION_TIME);
                        }
                        else if (e.Column.FieldName == "EXECUTE_ROOM_NAME_STR")
                        {
                            if (data.EXECUTE_ROOM_ID != null)
                            {
                                if (lstHisExecuteRoom != null && lstHisExecuteRoom.Count > 0)
                                {
                                    e.Value = lstHisExecuteRoom.FirstOrDefault(o => o.ROOM_ID == data.EXECUTE_ROOM_ID).EXECUTE_ROOM_NAME;
                                }
                                else
                                {
                                    e.Value = "";
                                }
                            }
                        }
                        else if (e.Column.FieldName == "SERVICE_REQ_CODE_STR")
                        {
                            if (data.SERVICE_REQ_CODE != null)
                                e.Value = data.SERVICE_REQ_CODE;
                            else
                                e.Value = "";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridView2_CustomUnboundColumnData(object sender, CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != DevExpress.Data.UnboundColumnType.Bound)
                {

                    var data = (HIS_TRACKING)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    if (data != null)
                    {
                        if (e.Column.FieldName == "TRACKING_TIME_STR")
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(data.TRACKING_TIME);
                        }
                        else if (e.Column.FieldName == "DEPARTMENT_NAME_STR")
                        {
                            if (data.DEPARTMENT_ID != null)
                            {
                                if (lstHisDepartment != null && lstHisDepartment.Count > 0)
                                {
                                    e.Value = lstHisDepartment.FirstOrDefault(o => o.ID == data.DEPARTMENT_ID).DEPARTMENT_NAME;
                                }
                                else
                                {
                                    e.Value = "";
                                }
                            }
                        }
                      
                    }
                }
            }
            catch (Exception ex)
            {
               Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        #endregion

        private void barButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                btnSave_Click(null,null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                List<string> lst = new List<string>();
                var rowSelectCongKham = gridView1.GetSelectedRows();
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => rowSelectCongKham), rowSelectCongKham));
      
                if (rowSelectCongKham != null && rowSelectCongKham.Count() > 0)
                {
                    foreach (var i in rowSelectCongKham)
                    {
                        var row = (HIS_SERVICE_REQ)gridView1.GetRow(i);
                        if (row != null)
                        {
                            lst.Add(row.PATHOLOGICAL_PROCESS);
                            Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => lst), lst));
  
                        }
                    }
                }
                var rowSelectToDieuTri = gridView2.GetSelectedRows();
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => rowSelectToDieuTri), rowSelectToDieuTri));
                if (rowSelectToDieuTri != null && rowSelectToDieuTri.Count() > 0)
                {
                    foreach (var i in rowSelectToDieuTri)
                    {
                        var row = (HIS_TRACKING)gridView2.GetRow(i);
                        if (row != null)
                        {
                            lst.Add(row.CONTENT);
                            Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => lst), lst));
                        }
                    }
                }
                Inventec.Common.Logging.LogSystem.Warn(String.Join("; ", lst));   
                this.delegateData(String.Join("; ",lst));
                this.Close();

            }
            catch (Exception ex)
            {
               Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }



    }
}
