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
using DevExpress.Data;
using DevExpress.XtraCharts;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.Plugins.SumaryTestResults.ADO;
using Inventec.Common.Adapter;
using Inventec.Core;
using Inventec.Desktop.Common.LanguageManager;
using Inventec.Desktop.Common.Message;
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
using DevExpress.XtraGrid;
using System.Globalization;

namespace HIS.Desktop.Plugins.SumaryTestResults
{
    public partial class frmSumaryTestResults : HIS.Desktop.Utility.FormBase
    {
        internal List<HIS_SERVICE_REQ> _ServiceReqs { get; set; }
        List<HIS_SERE_SERV> lstSereServ { get; set; }
        internal List<V_HIS_SERE_SERV_TEIN> _vSereServTeins { get; set; }
        internal List<V_HIS_SERE_SERV_TEIN> _vHisSereServTeinAlls { get; set; }
        // bool isCreateUnboundColumn = false;
        List<int> lstVisibleIndex = null;
        Inventec.Desktop.Common.Modules.Module currentModule;
        long treatmentId;

        public frmSumaryTestResults()
        {
            InitializeComponent();
        }

        public frmSumaryTestResults(Inventec.Desktop.Common.Modules.Module currentModule, long treatmentId)
            : base(currentModule)
        {
            InitializeComponent();
            try
            {
                SetIconFrm();
                this.currentModule = currentModule;
                this.treatmentId = treatmentId;
                if (this.currentModule != null)
                {
                    this.Text = this.currentModule.text;
                }
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

        private void frmSumaryTestResults_Load(object sender, EventArgs e)
        {
            try
            {
                SetCaptionByLanguageKey();
                LoadDataToGridSereServ();

                var row = (HIS_SERE_SERV)gridViewSereServ.GetFocusedRow();
                if (row != null && row.ID > 0)
                {
                    LoadDataSereServTein(row);
                }
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
                Resources.ResourceLanguageManager.LanguageResource = new ResourceManager("HIS.Desktop.Plugins.SumaryTestResults.Resources.Lang", typeof(HIS.Desktop.Plugins.SumaryTestResults.frmSumaryTestResults).Assembly);

                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.layoutControl1.Text = Inventec.Common.Resource.Get.Value("frmSumaryTestResults.layoutControl1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn1.Caption = Inventec.Common.Resource.Get.Value("frmSumaryTestResults.gridColumn1.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumnTestIndexCode.Caption = Inventec.Common.Resource.Get.Value("frmSumaryTestResults.gridColumn2.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn3.Caption = Inventec.Common.Resource.Get.Value("frmSumaryTestResults.gridColumn3.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn4.Caption = Inventec.Common.Resource.Get.Value("frmSumaryTestResults.gridColumn4.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColSerSevSTT.Caption = Inventec.Common.Resource.Get.Value("frmSumaryTestResults.gridColSerSevSTT.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColSerSevView.Caption = Inventec.Common.Resource.Get.Value("frmSumaryTestResults.gridColSerSevView.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColSerSevPrint.Caption = Inventec.Common.Resource.Get.Value("frmSumaryTestResults.gridColSerSevPrint.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColSerSevCode.Caption = Inventec.Common.Resource.Get.Value("frmSumaryTestResults.gridColSerSevCode.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColSerSevName.Caption = Inventec.Common.Resource.Get.Value("frmSumaryTestResults.gridColSerSevName.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColSerSevUnitName.Caption = Inventec.Common.Resource.Get.Value("frmSumaryTestResults.gridColSerSevUnitName.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColSerSevAmount.Caption = Inventec.Common.Resource.Get.Value("frmSumaryTestResults.gridColSerSevAmount.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColSerSevTypeName.Caption = Inventec.Common.Resource.Get.Value("frmSumaryTestResults.gridColSerSevTypeName.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.Text = Inventec.Common.Resource.Get.Value("frmSumaryTestResults.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadDataToGridSereServ()
        {
            try
            {
                CommonParam param = new CommonParam();
                //Load lấy các yêu cầu xét nghiệm của 1 BN
                MOS.Filter.HisServiceReqFilter serviceReqFilter1 = new HisServiceReqFilter();
                serviceReqFilter1.TREATMENT_ID = this.treatmentId;
                serviceReqFilter1.SERVICE_REQ_TYPE_ID = IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__XN;
                this._ServiceReqs = new List<HIS_SERVICE_REQ>();
                this._ServiceReqs = new BackendAdapter(param).Get<List<HIS_SERVICE_REQ>>("/api/HisServiceReq/Get", ApiConsumers.MosConsumer, serviceReqFilter1, param);
                if (this._ServiceReqs == null || this._ServiceReqs.Count == 0)
                {
                    DevExpress.XtraEditors.XtraMessageBox.Show("Bệnh nhân không có yêu cầu xét nghiệm", "Thông báo");
                    this.Close();
                    return;
                }
                WaitingManager.Show();
                var _serviceReqIds = this._ServiceReqs.Select(p => p.ID).ToList();

                lstSereServ = new List<HIS_SERE_SERV>();
                int index = 0;
                while (_serviceReqIds.Count - index > 0)
                {
                    var ssIds = _serviceReqIds.Skip(index).Take(100).ToList();

                    HisSereServViewFilter sereServFilter = new HisSereServViewFilter();
                    sereServFilter.SERVICE_REQ_IDs = ssIds;
                    sereServFilter.ORDER_FIELD = "SERVICE_NUM_ORDER";
                    sereServFilter.ORDER_DIRECTION = "DESC";
                    var lstSereServ = new BackendAdapter(param).Get<List<MOS.EFMODEL.DataModels.HIS_SERE_SERV>>("/api/HisSereServ/Get", ApiConsumers.MosConsumer, sereServFilter, param);
                    if (lstSereServ != null && lstSereServ.Count > 0)
                    {
                        this.lstSereServ.AddRange(lstSereServ);
                    }
                    index += 100;
                }

                var groupSS = lstSereServ.GroupBy(p => p.SERVICE_ID).Select(grc => grc.ToList()).ToList();
                List<HIS_SERE_SERV> lstSereServDistinct = new List<HIS_SERE_SERV>();
                foreach (var group in groupSS)
                {
                    HIS_SERE_SERV sereServ = new HIS_SERE_SERV();
                    sereServ = group.FirstOrDefault();
                    lstSereServDistinct.Add(sereServ);
                }
                gridControlSereServ.DataSource = lstSereServDistinct;
                LoadAllSSTeins();
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridControlSereServ_Click(object sender, EventArgs e)
        {
            try
            {
                var row = (HIS_SERE_SERV)gridViewSereServ.GetFocusedRow();
                if (row != null)
                {
                    LoadDataSereServTein(row);
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetDataGridSSTein()
        {
            try
            {

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadAllSSTeins()
        {
            try
            {
                CommonParam param = new CommonParam();
                int index = 0;
                this._vHisSereServTeinAlls = new List<V_HIS_SERE_SERV_TEIN>();
                while (lstSereServ.Count - index > 0)
                {
                    var ssIds = lstSereServ.Skip(index).Take(100).Select(o => o.ID).ToList();
                    List<long> sereServIds = ssIds;
                    HisSereServTeinViewFilter sereSerTeinFilter = new HisSereServTeinViewFilter();
                    sereSerTeinFilter.SERE_SERV_IDs = sereServIds;
                    sereSerTeinFilter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                    var vHisSereServTeinAlls = new BackendAdapter(param).Get<List<V_HIS_SERE_SERV_TEIN>>("api/HisSereServTein/GetView", ApiConsumers.MosConsumer, sereSerTeinFilter, param);
                    if(vHisSereServTeinAlls != null && vHisSereServTeinAlls.Count > 0)
                    {
                        _vHisSereServTeinAlls.AddRange(vHisSereServTeinAlls);
                    }
                    index += 100;
                }
                _vHisSereServTeinAlls = _vHisSereServTeinAlls.OrderByDescending(m => m.NUM_ORDER).ToList();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        List<HIS_SERE_SERV> _hisSereServCharts { get; set; }

        List<V_HIS_SERE_SERV_TEIN> _vSereServTeinCharts { get; set; }

        private void LoadDataSereServTein(HIS_SERE_SERV currentSereServ)
        {
            try
            {
                WaitingManager.Show();
                this._hisSereServCharts = new List<HIS_SERE_SERV>();
                this._vSereServTeinCharts = new List<V_HIS_SERE_SERV_TEIN>();
                this._vSereServTeins = new List<V_HIS_SERE_SERV_TEIN>();

                CommonParam param = new CommonParam();
                this._hisSereServCharts = lstSereServ.Where(p => p.SERVICE_ID == currentSereServ.SERVICE_ID).ToList();
                List<long> sereServIds = this._hisSereServCharts.Select(o => o.ID).ToList();
                //HisSereServTeinViewFilter sereSerTeinFilter = new HisSereServTeinViewFilter();
                //sereSerTeinFilter.SERE_SERV_IDs = sereServIds;
                //sereSerTeinFilter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                //_vSereServTeins = new BackendAdapter(param).Get<List<V_HIS_SERE_SERV_TEIN>>("api/HisSereServTein/GetView", ApiConsumers.MosConsumer, sereSerTeinFilter, param).OrderByDescending(m => m.NUM_ORDER).ToList();

                this._vSereServTeins = this._vHisSereServTeinAlls.Where(p => sereServIds.Contains(p.SERE_SERV_ID) && p.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList();

                index = 0;
                var groupSereServTeinADO = _vSereServTeins.GroupBy(p => p.TEST_INDEX_ID).Select(grc => grc.ToList()).ToList();
                List<HisSereServTeinADO> _HisSereServTeinADOs = new List<HisSereServTeinADO>();
                foreach (var itemGroup in groupSereServTeinADO)
                {
                    if (index < itemGroup.Count)
                    {
                        index = itemGroup.Count;
                    }
                    this._vSereServTeinCharts.Add(itemGroup.FirstOrDefault());
                    HisSereServTeinADO ado = new HisSereServTeinADO(itemGroup.FirstOrDefault());
                    _HisSereServTeinADOs.Add(ado);
                }
                //isCreateUnboundColumn = false;
                gridControlSereServTein.DataSource = _HisSereServTeinADOs;
                //if (isCreateUnboundColumn == false)
                if (index > 0)
                {
                    CreateUnboundColumn(index);
                }

                CreateControlChart();

                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        int index = 0;

        private void CreateUnboundColumn(int indexCreate)
        {
            try
            {
                if (lstVisibleIndex != null)
                {
                    for (int i = 0; i < lstVisibleIndex.Count; i++)
                    {
                        gridViewSereServTein.Columns.RemoveAt(lstVisibleIndex[i] - i);
                    }
                }
                lstVisibleIndex = new List<int>();
                GridColumn col = new GridColumn();
                var serviceReqGroups = this._hisSereServCharts.OrderBy(p => p.TDL_INTRUCTION_TIME).GroupBy(p => p.TDL_INTRUCTION_TIME).Select(p => p.ToList()).ToList();
                for (int i = 0; i < indexCreate; i++)
                {
                    long time = serviceReqGroups[i].FirstOrDefault().TDL_INTRUCTION_TIME;
                    col = gridViewSereServTein.Columns.AddVisible("cot" + i.ToString(), Inventec.Common.DateTime.Convert.TimeNumberToTimeString(time));
                    col.OptionsColumn.AllowEdit = false;
                    col.OptionsColumn.AllowSize = true;
                    col.AppearanceHeader.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
                    col.Width = 80;
                    col.UnboundType = DevExpress.Data.UnboundColumnType.Object;
                    lstVisibleIndex.Add(col.VisibleIndex);
                }

                //  isCreateUnboundColumn = true;
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
                    HIS_SERE_SERV data = (HIS_SERE_SERV)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];

                    DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                    if (e.Column.FieldName == "STT")
                    {
                        e.Value = e.ListSourceRowIndex + 1;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewSereServTein_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)
                {
                    V_HIS_SERE_SERV_TEIN data = (V_HIS_SERE_SERV_TEIN)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];

                    DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                    if (e.Column.FieldName == "STT")
                    {
                        e.Value = e.ListSourceRowIndex + 1;
                    }
                    else
                        for (int i = 0; i < index; i++)
                        {
                            if (e.Column.FieldName == "cot" + i)
                            {
                                e.Value = ProcessorGetValue(e.Column.GetCaption(), data.TEST_INDEX_ID ?? 0);
                                //var serviceReqGroups = this._hisSereServCharts.OrderBy(p => p.TDL_INTRUCTION_TIME).GroupBy(p => p.TDL_INTRUCTION_TIME).Select(p => p.ToList()).ToList();
                                //string caption = e.Column.GetCaption();
                                //string time = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(serviceReqGroups[i].FirstOrDefault().TDL_INTRUCTION_TIME);
                                //if (caption == time)
                                //{
                                //    List<long> _sereServIds = serviceReqGroups[i].Select(p => p.ID).ToList();
                                //    List<V_HIS_SERE_SERV_TEIN> rowCellStyles = (this._vSereServTeins != null && _vSereServTeins.Count > 0) ? (_vSereServTeins.Where(p => p.TEST_INDEX_ID == data.TEST_INDEX_ID && _sereServIds.Contains(p.SERE_SERV_ID)).ToList()) : null;
                                //    e.Value = rowCellStyles[i].VALUE;
                                //}
                            }
                        }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private string ProcessorGetValue(string caption, long _testIndexId)
        {
            string value = "";
            try
            {
                var sereServGroups = this._hisSereServCharts.OrderBy(p => p.TDL_INTRUCTION_TIME).GroupBy(p => p.TDL_INTRUCTION_TIME).Select(p => p.ToList()).ToList();
                foreach (var item in sereServGroups)
                {
                    string time = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(item.FirstOrDefault().TDL_INTRUCTION_TIME);
                    if (caption == time)
                    {
                        List<long> _sereServIds = item.Select(p => p.ID).ToList();
                        V_HIS_SERE_SERV_TEIN rowCellStyle = (this._vSereServTeins != null && _vSereServTeins.Count > 0) ? (_vSereServTeins.FirstOrDefault(p => p.TEST_INDEX_ID == _testIndexId && _sereServIds.Contains(p.SERE_SERV_ID))) : null;
                        value = rowCellStyle.VALUE;
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                value = "";
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return value;
        }

        private void gridViewSereServTein_RowCellStyle(object sender, DevExpress.XtraGrid.Views.Grid.RowCellStyleEventArgs e)
        {
            DevExpress.XtraGrid.Views.Grid.GridView vw = (sender as DevExpress.XtraGrid.Views.Grid.GridView);
            try
            {
                if (e.RowHandle >= 0)
                {
                    long testIndexID = (long)vw.GetRowCellValue(e.RowHandle, "TEST_INDEX_ID");
                    string result = ProcessorGetResult(e.Column.GetCaption(), testIndexID);
                    if (Inventec.Common.TypeConvert.Parse.ToInt64(result) == 1
                                    || Inventec.Common.TypeConvert.Parse.ToInt64(result) == 2)
                    {
                        e.Appearance.ForeColor = Color.Red;
                        e.Appearance.Font = new Font(e.Appearance.Font, FontStyle.Bold);
                    }
                }
                #region ----- old
                //long testIndexID = (long)vw.GetRowCellValue(e.RowHandle, "TEST_INDEX_ID");

                //var groupSereServTeinADO = _vSereServTeins.GroupBy(p => p.TEST_INDEX_ID).Select(grc => grc.ToList()).ToList();
                //foreach (var item in groupSereServTeinADO)
                //{
                //    if (item.FirstOrDefault().TEST_INDEX_ID == testIndexID)
                //    {
                //        for (int i = 0; i < item.Count; i++)
                //        {
                //            if (e.Column.FieldName == "cot" + i)
                //            {
                //                if (Inventec.Common.TypeConvert.Parse.ToInt64(item[i].RESULT_CODE) == 1
                //                    || Inventec.Common.TypeConvert.Parse.ToInt64(item[i].RESULT_CODE) == 2)
                //                {
                //                    e.Appearance.ForeColor = Color.Red;
                //                    e.Appearance.Font = new Font(e.Appearance.Font, FontStyle.Bold);
                //                }
                //            }
                //        }
                //    }
                //}
                #endregion
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private string ProcessorGetResult(string caption, long _testIndexId)
        {
            string value = "";
            try
            {
                var sereServGroups = this._hisSereServCharts.OrderBy(p => p.TDL_INTRUCTION_TIME).GroupBy(p => p.TDL_INTRUCTION_TIME).Select(p => p.ToList()).ToList();
                foreach (var item in sereServGroups)
                {
                    string time = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(item.FirstOrDefault().TDL_INTRUCTION_TIME);
                    if (caption == time)
                    {
                        List<long> _sereServIds = item.Select(p => p.ID).ToList();
                        V_HIS_SERE_SERV_TEIN rowCellStyle = (this._vSereServTeins != null && _vSereServTeins.Count > 0) ? (_vSereServTeins.FirstOrDefault(p => p.TEST_INDEX_ID == _testIndexId && _sereServIds.Contains(p.SERE_SERV_ID))) : null;
                        value = rowCellStyle.RESULT_CODE;
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                value = "";
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return value;
        }
        internal decimal ConvertToDecimal(string value)
        {
            decimal result = 0;
            try
            {
                if (string.IsNullOrEmpty(value))
                    return 0;
                CultureInfo culture = new CultureInfo("en-US");
                if (value.Contains(","))
                    culture = new CultureInfo("fr-FR");
                result = Convert.ToDecimal(value, culture);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }
        private void CreateControlChart()
        {
            try
            {
                this.panelControlChart.Controls.Clear();
                this.panelControlChart.Update();
                ChartControl chartControlSereServTein = new ChartControl();
                chartControlSereServTein.Series.Clear();
                chartControlSereServTein.Update();
                if (this._vSereServTeinCharts != null && this._vSereServTeinCharts.Count > 0)
                {
                    foreach (var item in this._vSereServTeinCharts)
                    {
                        Series _series = new Series(item.TEST_INDEX_NAME, ViewType.Line);
                        var dataGroups = this._hisSereServCharts.OrderBy(p => p.TDL_INTRUCTION_TIME).GroupBy(p => new { p.TDL_INTRUCTION_TIME }).Select(p => p.ToList()).ToList();
                        foreach (var itemGr in dataGroups)
                        {
                            foreach (var item2 in itemGr)
                            {
                                var rowCellStyle = (_vSereServTeins != null && _vSereServTeins.Count > 0) ? (_vSereServTeins.FirstOrDefault(p => p.TEST_INDEX_ID == item.TEST_INDEX_ID && p.SERE_SERV_ID == item2.ID)) : null;

                                string _TimeDate = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(item2.TDL_INTRUCTION_TIME);// ?? DateTime.Now;
                                //SeriesPoint ser = new SeriesPoint();
                                //ser.Argument = _TimeDate;
                                if (rowCellStyle != null && rowCellStyle.ID > 0 && !string.IsNullOrEmpty(rowCellStyle.VALUE))
                                {
                                    //double[] value = new double[1];
                                    //value[0] = Inventec.Common.TypeConvert.Parse.ToDouble(rowCellStyle.VALUE);
                                    //ser.Values = value;
                                    _series.Points.Add(new SeriesPoint(_TimeDate, ConvertToDecimal(rowCellStyle.VALUE)));
                                }
                                else
                                {
                                    _series.Points.Add(new SeriesPoint(_TimeDate));
                                }
                                // _series.Points.Add(ser);
                            }
                        }

                        _series.ArgumentScaleType = ScaleType.Qualitative;

                        //// Access the view-type-specific options of the series.
                        ((LineSeriesView)_series.View).MarkerVisibility = DevExpress.Utils.DefaultBoolean.True;
                        //((LineSeriesView)_series.View).LineMarkerOptions.Kind = MarkerKind.Triangle;
                        //((LineSeriesView)_series.View).LineStyle.DashStyle = DashStyle.Dash;


                        chartControlSereServTein.Series.Add(_series);
                    }
                    // Access the type-specific options of the diagram.
                    ((XYDiagram)chartControlSereServTein.Diagram).EnableAxisXZooming = true;

                    ((XYDiagram)chartControlSereServTein.Diagram).AxisX.Range.Auto = false;
                    ((XYDiagram)chartControlSereServTein.Diagram).AxisY.Range.Auto = false;

                    // Hide the legend (if necessary).
                    chartControlSereServTein.Legend.UseCheckBoxes = true;
                    chartControlSereServTein.Legend.Visibility = DevExpress.Utils.DefaultBoolean.True;
                }
                this.panelControlChart.Controls.Add(chartControlSereServTein);
                chartControlSereServTein.Dock = DockStyle.Fill;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void repositoryItemButton__Chart_Add_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                var data = (HisSereServTeinADO)gridViewSereServTein.GetFocusedRow();
                if (data != null && data.IsAdd)
                {
                    data.IsAdd = false;
                    this._vSereServTeinCharts.Add(data);
                    CreateControlChart();
                }
                gridViewSereServTein.RefreshRow(gridViewSereServTein.FocusedRowHandle);
                this.panelControlChart.Focus();
                // this.gridColumnChart.ColumnEdit = this.repositoryItemButton__Chart_Delete;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void repositoryItemButton__Chart_Delete_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                var data = (HisSereServTeinADO)gridViewSereServTein.GetFocusedRow();
                if (data != null && !data.IsAdd)
                {
                    data.IsAdd = true;
                    V_HIS_SERE_SERV_TEIN ado = new V_HIS_SERE_SERV_TEIN();
                    ado = this._vSereServTeinCharts.FirstOrDefault(p => p.TEST_INDEX_ID == data.TEST_INDEX_ID);
                    this._vSereServTeinCharts.Remove(ado);
                    CreateControlChart();
                }
                //else if (data != null && data.IsAdd)
                //{
                //    data.IsAdd = false;
                //    this._vSereServTeinCharts.Add(data);
                //    CreateControlChart();
                //    repositoryItemButton__Chart_Delete.Buttons[0].Image = HIS.Desktop.Plugins.SumaryTestResults.Properties.Resources.iconfinder_chart_line_delete_5151;
                //}
                gridViewSereServTein.RefreshRow(gridViewSereServTein.FocusedRowHandle);
                this.panelControlChart.Focus();

                // this.gridColumnChart.ColumnEdit = this.repositoryItemButton__Chart_Add;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewSereServTein_CustomRowCellEdit(object sender, DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs e)
        {
            try
            {
                if (e.RowHandle >= 0)
                {
                    bool isAdd = (bool)gridViewSereServTein.GetRowCellValue(e.RowHandle, "IsAdd");
                    if (e.Column.FieldName == "CHART")
                    {
                        if (isAdd)
                        {
                            e.RepositoryItem = repositoryItemButton__Chart_Add;
                        }
                        else
                        {
                            e.RepositoryItem = repositoryItemButton__Chart_Delete;
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
