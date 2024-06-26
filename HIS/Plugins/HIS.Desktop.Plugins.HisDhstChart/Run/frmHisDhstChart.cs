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
using DevExpress.XtraCharts;
using HIS.Desktop.ApiConsumer;
using Inventec.Common.Adapter;
using Inventec.Core;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.HisDhstChart.Run
{
    public partial class frmHisDhstChart : HIS.Desktop.Utility.FormBase
    {
        Inventec.Desktop.Common.Modules.Module currentModule;
        long treatmentId;

        DataTable dataTable { get; set; }

        internal List<HIS_DHST> _HisDhsts { get; set; }

        public frmHisDhstChart()
        {
            InitializeComponent();
        }

        public frmHisDhstChart(Inventec.Desktop.Common.Modules.Module currentModule, long treatmentId)
            : base(currentModule)
        {
            InitializeComponent();
            try
            {
                this.currentModule = currentModule;
                this.treatmentId = treatmentId;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public frmHisDhstChart(Inventec.Desktop.Common.Modules.Module currentModule, List<HIS_DHST> _hisDhsts)
            : base(currentModule)
        {
            InitializeComponent();
            try
            {
                this.currentModule = currentModule;
                this._HisDhsts = _hisDhsts;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void frmHisDhstChart_Load(object sender, EventArgs e)
        {
            try
            {
                SetIconFrm();
                if (this.currentModule != null)
                {
                    this.Text = this.currentModule.text;
                }
                if (this.treatmentId > 0)
                {
                    //TODO
                    LoadDHST();
                }
                CreateChartControl();
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

        private void CreateChartControl()
        {
            try
            {
                dataTable = new DataTable("Table");
                dataTable.Columns.Add("ky", typeof(string));
                dataTable.Columns.Add("PULSE", typeof(decimal));
                dataTable.Columns.Add("TEMPERATURE", typeof(decimal));
                dataTable.Columns.Add("BLOOD_PRESSURE_MIN", typeof(decimal));
                dataTable.Columns.Add("BLOOD_PRESSURE_MAX", typeof(decimal));
                dataTable.Columns.Add("WEIGHT", typeof(decimal));
                dataTable.Columns.Add("BREATH_RATE", typeof(decimal));
                dataTable.Columns.Add("URINE", typeof(decimal));
                dataTable.Columns.Add("CAPILLARY_BLOOD_GLUCOSE", typeof(decimal));
                if (_HisDhsts == null)
                {
                    return;
                }
                else
                {
                    //HIS_DHST ado = new HIS_DHST();//Goc
                    //ado.EXECUTE_TIME = 0;
                    //ado.PULSE = 0;
                    //ado.TEMPERATURE = 0;
                    //ado.BLOOD_PRESSURE_MAX = 0;
                    //ado.WEIGHT = 0;
                    //ado.BREATH_RATE = 0;
                    //_HisDhsts.Add(ado);
                    _HisDhsts = _HisDhsts.OrderBy(p => p.EXECUTE_TIME).ToList();
                }
                for (int i = 0; i < _HisDhsts.Count; i++)
                {
                    if (_HisDhsts[i].EXECUTE_TIME == null)
                        continue;
                    DataRow row = dataTable.NewRow();

                    row["ky"] = _HisDhsts[i].EXECUTE_TIME == 0 ? "0" : Inventec.Common.DateTime.Convert.TimeNumberToTimeString(_HisDhsts[i].EXECUTE_TIME ?? 0);
                    row["PULSE"] = _HisDhsts[i].PULSE ?? 0;
                    row["TEMPERATURE"] = _HisDhsts[i].TEMPERATURE ?? 0;
                    row["BLOOD_PRESSURE_MIN"] = _HisDhsts[i].BLOOD_PRESSURE_MIN ?? 0;
                    row["BLOOD_PRESSURE_MAX"] = _HisDhsts[i].BLOOD_PRESSURE_MAX ?? 0;
                    row["WEIGHT"] = _HisDhsts[i].WEIGHT ?? 0;
                    row["BREATH_RATE"] = _HisDhsts[i].BREATH_RATE ?? 0;
                    row["URINE"] = _HisDhsts[i].URINE ?? 0;
                    row["CAPILLARY_BLOOD_GLUCOSE"] = _HisDhsts[i].CAPILLARY_BLOOD_GLUCOSE ?? 0;
                    dataTable.Rows.Add(row);
                }
                chartControlDHST.Series[0].DataSource = dataTable;
                //chartControl1.Series[0].ArgumentScaleType = ScaleType.Numerical;
                // Access the view-type-specific options of the series.
                ((LineSeriesView)chartControlDHST.Series[0].View).MarkerVisibility = DevExpress.Utils.DefaultBoolean.True;
                ((LineSeriesView)chartControlDHST.Series[0].View).LineMarkerOptions.Kind = MarkerKind.Triangle;
                ((LineSeriesView)chartControlDHST.Series[0].View).LineStyle.DashStyle = DashStyle.Dash;

                chartControlDHST.Series[1].DataSource = dataTable;
                // chartControl1.Series[1].ArgumentScaleType = ScaleType.Numerical;
                // Access the view-type-specific options of the series.
                ((LineSeriesView)chartControlDHST.Series[1].View).MarkerVisibility = DevExpress.Utils.DefaultBoolean.True;
                ((LineSeriesView)chartControlDHST.Series[1].View).LineMarkerOptions.Kind = MarkerKind.Circle;
                //((LineSeriesView)chartControl1.Series[1].View).LineStyle.DashStyle = DashStyle.DashDot;

                chartControlDHST.Series[2].DataSource = dataTable;
                //chartControl1.Series[2].ArgumentScaleType = ScaleType.Numerical;
                // Access the view-type-specific options of the series.
                ((LineSeriesView)chartControlDHST.Series[2].View).MarkerVisibility = DevExpress.Utils.DefaultBoolean.True;
                ((LineSeriesView)chartControlDHST.Series[2].View).LineMarkerOptions.Kind = MarkerKind.Cross;
                ((LineSeriesView)chartControlDHST.Series[2].View).LineStyle.DashStyle = DashStyle.DashDotDot;

                chartControlDHST.Series[3].DataSource = dataTable;
                //chartControl1.Series[3].ArgumentScaleType = ScaleType.Numerical;
                // Access the view-type-specific options of the series.
                ((LineSeriesView)chartControlDHST.Series[3].View).MarkerVisibility = DevExpress.Utils.DefaultBoolean.True;
                ((LineSeriesView)chartControlDHST.Series[3].View).LineMarkerOptions.Kind = MarkerKind.Diamond;
                // ((LineSeriesView)chartControl1.Series[3].View).LineStyle.DashStyle = DashStyle.Dot;

                chartControlDHST.Series[4].DataSource = dataTable;
                // chartControl1.Series[4].ArgumentScaleType = ScaleType.Numerical;
                // Access the view-type-specific options of the series.
                ((LineSeriesView)chartControlDHST.Series[4].View).MarkerVisibility = DevExpress.Utils.DefaultBoolean.True;
                ((LineSeriesView)chartControlDHST.Series[4].View).LineMarkerOptions.Kind = MarkerKind.Hexagon;
                ((LineSeriesView)chartControlDHST.Series[4].View).LineStyle.DashStyle = DashStyle.Solid;

                chartControlDHST.Series[5].DataSource = dataTable;
                // chartControl1.Series[5].ArgumentScaleType = ScaleType.Numerical;
                // Access the view-type-specific options of the series.
                ((LineSeriesView)chartControlDHST.Series[5].View).MarkerVisibility = DevExpress.Utils.DefaultBoolean.True;
                ((LineSeriesView)chartControlDHST.Series[5].View).LineMarkerOptions.Kind = MarkerKind.Hexagon;
                ((LineSeriesView)chartControlDHST.Series[5].View).LineStyle.DashStyle = DashStyle.Solid;

                chartControlDHST.Series[6].DataSource = dataTable;
                // chartControl1.Series[6].ArgumentScaleType = ScaleType.Numerical;
                // Access the view-type-specific options of the series.
                ((LineSeriesView)chartControlDHST.Series[6].View).MarkerVisibility = DevExpress.Utils.DefaultBoolean.True;
                ((LineSeriesView)chartControlDHST.Series[6].View).LineMarkerOptions.Kind = MarkerKind.Pentagon;
                ((LineSeriesView)chartControlDHST.Series[6].View).LineStyle.DashStyle = DashStyle.Solid;

                chartControlDHST.Series[7].DataSource = dataTable;
                // chartControl1.Series[7].ArgumentScaleType = ScaleType.Numerical;
                // Access the view-type-specific options of the series.
                ((LineSeriesView)chartControlDHST.Series[7].View).MarkerVisibility = DevExpress.Utils.DefaultBoolean.True;
                ((LineSeriesView)chartControlDHST.Series[7].View).LineMarkerOptions.Kind = MarkerKind.InvertedTriangle;
                ((LineSeriesView)chartControlDHST.Series[7].View).LineStyle.DashStyle = DashStyle.Solid;
                // Access the type-specific options of the diagram.
                ((XYDiagram)chartControlDHST.Diagram).EnableAxisXZooming = true;

                ((XYDiagram)chartControlDHST.Diagram).AxisX.Range.Auto = false;
                ((XYDiagram)chartControlDHST.Diagram).AxisY.Range.Auto = false;



                // Hide the legend (if necessary).
                // chartControl1.Legend.Visibility = DevExpress.Utils.DefaultBoolean.False;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadDHST()
        {
            try
            {
                MOS.Filter.HisDhstFilter filter = new MOS.Filter.HisDhstFilter();
                filter.TREATMENT_ID = this.treatmentId;
                this._HisDhsts = new List<HIS_DHST>();
                this._HisDhsts = new BackendAdapter(new CommonParam()).Get<List<HIS_DHST>>("api/HisDhst/Get", ApiConsumers.MosConsumer, filter, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void CreateChart2()
        {
            try
            {
                ChartControl lineChart = new ChartControl();

                // Create a line series.
                Series series1 = new Series("Series 1", ViewType.Line);

                // Add points to it.
                series1.Points.Add(new SeriesPoint(1, 2));
                series1.Points.Add(new SeriesPoint(2, 12));
                series1.Points.Add(new SeriesPoint(3, 14));
                series1.Points.Add(new SeriesPoint(4, 17));

                // Add the series to the chart.
                lineChart.Series.Add(series1);

                // Set the numerical argument scale types for the series,
                // as it is qualitative, by default.
                series1.ArgumentScaleType = ScaleType.Numerical;

                // Access the view-type-specific options of the series.
                ((LineSeriesView)series1.View).MarkerVisibility = DevExpress.Utils.DefaultBoolean.True;
                ((LineSeriesView)series1.View).LineMarkerOptions.Kind = MarkerKind.Triangle;
                ((LineSeriesView)series1.View).LineStyle.DashStyle = DashStyle.Dash;

                // Access the type-specific options of the diagram.
                ((XYDiagram)lineChart.Diagram).EnableAxisXZooming = true;

                // Hide the legend (if necessary).
                lineChart.Legend.Visibility = DevExpress.Utils.DefaultBoolean.False;

                // Add a title to the chart (if necessary).
                lineChart.Titles.Add(new ChartTitle());
                lineChart.Titles[0].Text = "A Line Chart";

                // Add the chart to the form.
                lineChart.Dock = DockStyle.Fill;





                ChartControl LineChart3D = new ChartControl();

                // Add a line series to it.
                Series series2 = new Series("Series 1", ViewType.Line3D);

                // Add points to the series.
                series2.Points.Add(new SeriesPoint("A", 12));
                series2.Points.Add(new SeriesPoint("B", 4));
                series2.Points.Add(new SeriesPoint("C", 17));
                series2.Points.Add(new SeriesPoint("D", 7));
                series2.Points.Add(new SeriesPoint("E", 12));
                series2.Points.Add(new SeriesPoint("F", 4));
                series2.Points.Add(new SeriesPoint("G", 17));
                series2.Points.Add(new SeriesPoint("H", 7));

                // Add both series to the chart.
                LineChart3D.Series.Add(series2);

                // Customize the view-type-specific properties of the series.
                Line3DSeriesView myView = (Line3DSeriesView)series2.View;
                myView.LineWidth = 5;
                myView.LineThickness = 4;

                // Access the diagram's options.
                ((XYDiagram3D)LineChart3D.Diagram).ZoomPercent = 110;
                ((XYDiagram3D)LineChart3D.Diagram).VerticalScrollPercent = 10;

                // Add a title to the chart and hide the legend.
                ChartTitle chartTitle1 = new ChartTitle();
                chartTitle1.Text = "3D Line Chart";
                LineChart3D.Titles.Add(chartTitle1);
                //LineChart3D.Legend.Visible = false;

                // Add the chart to the form.
                LineChart3D.Dock = DockStyle.Fill;
                // this.panelControl1.Controls.Add(LineChart3D);

                // this.panelControl1.Controls.Add(lineChart);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void chartControlDHST_MouseClick(object sender, MouseEventArgs e)
        {
            try
            {
                ChartHitInfo hitInfo = chartControlDHST.CalcHitInfo(e.Location);

                if (hitInfo.InSeries && hitInfo.InLegend)
                {
                    if (hitInfo.Series == chartControlDHST.Series[0])
                    {
                        if (chartControlDHST.Series[0].DataSource != null)
                            chartControlDHST.Series[0].DataSource = null;
                        else
                            chartControlDHST.Series[0].DataSource = dataTable;
                    }
                    else if (hitInfo.Series == chartControlDHST.Series[1])
                    {
                        if (chartControlDHST.Series[1].DataSource != null)
                            chartControlDHST.Series[1].DataSource = null;
                        else
                            chartControlDHST.Series[1].DataSource = dataTable;
                    }
                    else if (hitInfo.Series == chartControlDHST.Series[2])
                    {
                        if (chartControlDHST.Series[2].DataSource != null)
                            chartControlDHST.Series[2].DataSource = null;
                        else
                            chartControlDHST.Series[2].DataSource = dataTable;
                    }
                    else if (hitInfo.Series == chartControlDHST.Series[3])
                    {
                        if (chartControlDHST.Series[3].DataSource != null)
                            chartControlDHST.Series[3].DataSource = null;
                        else
                            chartControlDHST.Series[3].DataSource = dataTable;
                    }
                    else if (hitInfo.Series == chartControlDHST.Series[4])
                    {
                        if (chartControlDHST.Series[4].DataSource != null)
                            chartControlDHST.Series[4].DataSource = null;
                        else
                            chartControlDHST.Series[4].DataSource = dataTable;
                    }
                    else if (hitInfo.Series == chartControlDHST.Series[5])
                    {
                        if (chartControlDHST.Series[5].DataSource != null)
                            chartControlDHST.Series[5].DataSource = null;
                        else
                            chartControlDHST.Series[5].DataSource = dataTable;
                    }
                    else if (hitInfo.Series == chartControlDHST.Series[6])
                    {
                        if (chartControlDHST.Series[6].DataSource != null)
                            chartControlDHST.Series[6].DataSource = null;
                        else
                            chartControlDHST.Series[6].DataSource = dataTable;
                    }
                    else if (hitInfo.Series == chartControlDHST.Series[7])
                    {
                        if (chartControlDHST.Series[7].DataSource != null)
                            chartControlDHST.Series[7].DataSource = null;
                        else
                            chartControlDHST.Series[7].DataSource = dataTable;
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
