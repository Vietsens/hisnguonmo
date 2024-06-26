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
using System.Drawing;
using System.Data;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using DevExpress.XtraCharts;
using Inventec.Core;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using Inventec.Common.Adapter;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.LocalStorage.BackendData;
using MOS.SDO;

namespace HIS.Desktop.Plugins.Dashboard
{
    public partial class UCDashBoardViewer : UserControl
    {
        public UCDashBoardViewer()
        {
            InitializeComponent();

            MeShow();
        }

        private DataTable CreateChartData(int rowCount)
        {
            // Create an empty table.
            DataTable table = new DataTable("Table1");

            // Add two columns to the table.
            table.Columns.Add("Argument", typeof(Int32));
            table.Columns.Add("Value", typeof(Int32));

            // Add data rows to the table.
            Random rnd = new Random();
            DataRow row = null;
            for (int i = 0; i < rowCount; i++)
            {
                row = table.NewRow();
                row["Argument"] = i;
                row["Value"] = rnd.Next(100);
                table.Rows.Add(row);
            }

            return table;
        }

        public void MeShow()
        {
            try
            {
                //Create servicereq room counter
                CreateServiceReqRoomCounterChart();

                //create patient bedroom counter
                CreatePatientCounterChart();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        void CreatePatientCounterChart()
        {
            try
            {
                if (chartPatientCounterByBedRoom.Series != null && chartPatientCounterByBedRoom.Series.Count > 0)
                    chartPatientCounterByBedRoom.Series.Clear();

                // Create a pie series.
                Series series1 = new Series("Số lượng bệnh nhân đang điều trị theo buồng", ViewType.Pie);

                HisTreatmentBedRoomViewFilter roomCounterViewFilter = new MOS.Filter.HisTreatmentBedRoomViewFilter();
                roomCounterViewFilter.IS_IN_ROOM = true;
                var bedroomCounters = new BackendAdapter(new CommonParam()).Get<List<V_HIS_TREATMENT_BED_ROOM>>("api/HisTreatmentBedRoom/GetView", ApiConsumers.MosConsumer, roomCounterViewFilter, null);
                if (bedroomCounters != null && bedroomCounters.Count > 0 && BackendDataWorker.Get<V_HIS_BED_ROOM>().Count > 0)
                {
                    decimal totalTreatmentBedRoomPercen = 0;
                    foreach (var item_bedRoom in BackendDataWorker.Get<V_HIS_BED_ROOM>())
                    {
                        decimal totalTreatmentBedRoom = bedroomCounters.Where(o => o.BED_ROOM_ID == item_bedRoom.ID).Count();
                        totalTreatmentBedRoomPercen = ((decimal)totalTreatmentBedRoom / (decimal)(bedroomCounters.Count)) * (decimal)100;
                        series1.Points.Add(new SeriesPoint(item_bedRoom.BED_ROOM_NAME, totalTreatmentBedRoom, totalTreatmentBedRoom));
                    }
                }

                // Add the series to the chart.
                chartPatientCounterByBedRoom.Series.Add(series1);

                // Format the the series labels.
                series1.Label.TextPattern = "{A}: {V} bệnh nhân ({VP:p1})";

                // Adjust the position of series labels. 
                ((PieSeriesLabel)series1.Label).Position = PieSeriesLabelPosition.TwoColumns;

                // Detect overlapping of series labels.
                ((PieSeriesLabel)series1.Label).ResolveOverlappingMode = ResolveOverlappingMode.Default;

                // Access the view-type-specific options of the series.
                PieSeriesView myView = (PieSeriesView)series1.View;

                // Show a title for the series.
                //myView.Titles.Add(new SeriesTitle());
                //myView.Titles[0].Text = series1.Name;

                // Specify a data filter to explode points.
                //myView.ExplodedPointsFilters.Add(new SeriesPointFilter(SeriesPointKey.Value_1,
                //    DataFilterCondition.GreaterThanOrEqual, 9));
                //myView.ExplodedPointsFilters.Add(new SeriesPointFilter(SeriesPointKey.Argument,
                //    DataFilterCondition.NotEqual, "Khác"));
                myView.ExplodeMode = PieExplodeMode.UseFilters;
                myView.ExplodedDistancePercentage = 30;
                myView.RuntimeExploding = true;
                myView.HeightToWidthRatio = 0.75;

                // Hide the legend (if necessary).
                chartPatientCounterByBedRoom.Legend.Visibility = DevExpress.Utils.DefaultBoolean.True;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }


        XYDiagram Diagram { get { return chartVienPhi.Diagram as XYDiagram; } }
        AxisBase AxisX { get { return Diagram != null ? Diagram.AxisX : null; } }

        enum TypeChartColumn
        {
            Total,
            BaoHiem,
            VienPhi,
            TamUng
        }

        //public override ChartControl ChartControl { get { return chartServiceReqCounterByRoom; } }

        void LoadSeries(Series series, List<MOS.EFMODEL.DataModels.V_HIS_ROOM_COUNTER> roomCounters, string name, string shortName, TypeChartColumn typeChartColumn)
        {
            try
            {
                LoadPoints(series, roomCounters, typeChartColumn);
                series.CrosshairLabelPattern = shortName + " : {V}";
                series.Name = name;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        void LoadPoints(Series series, List<MOS.EFMODEL.DataModels.V_HIS_ROOM_COUNTER> roomCounters, TypeChartColumn typeChartColumn)
        {
            try
            {
                if (series != null && roomCounters != null)
                {
                    series.Points.BeginUpdate();
                    series.Points.Clear();
                    foreach (var roomCounter in roomCounters)
                    {
                        double rate = 0;
                        DateTime date = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(roomCounter.MODIFY_TIME ?? 0) ?? DateTime.Now;
                        switch (typeChartColumn)
                        {
                            case TypeChartColumn.Total:
                                rate = (double)(roomCounter.TOTAL_TODAY_SERVICE_REQ ?? 0);
                                break;
                            case TypeChartColumn.BaoHiem:
                                rate = (double)(roomCounter.TOTAL_OPEN_SERVICE_REQ ?? 0);
                                break;
                            case TypeChartColumn.VienPhi:
                                rate = (double)((roomCounter.TOTAL_TODAY_SERVICE_REQ ?? 0) - (roomCounter.TOTAL_OPEN_SERVICE_REQ ?? 0));
                                break;
                            case TypeChartColumn.TamUng:
                                rate = (double)((roomCounter.TOTAL_TODAY_SERVICE_REQ ?? 0) - (roomCounter.TOTAL_OPEN_SERVICE_REQ ?? 0));
                                break;
                            default:
                                break;
                        }

                        //series.Points.Add(new SeriesPoint(date, rate));
                        series.Points.Add(new SeriesPoint(roomCounter.ROOM_NAME, rate));
                    }
                    series.Points.EndUpdate();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        void CreateServiceReqRoomCounterChart()
        {
            try
            {
                //if (chartServiceReqCounterByRoom.Series != null && chartServiceReqCounterByRoom.Series.Count > 0)
                //    chartServiceReqCounterByRoom.Series.Clear();

                chartVienPhi.CrosshairOptions.CrosshairLabelMode = CrosshairLabelMode.ShowForNearestSeries;

                MOS.SDO.HipoFinanceReportSDO dataADO = new MOS.SDO.HipoFinanceReportSDO();

                long date = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(DateTime.Now) ?? 0;
                List<string> _DateNow = new List<string>();
                _DateNow.Add(date.ToString().Substring(0, 8) + "000000");
                _DateNow.Add(date.ToString().Substring(0, 8) + "235959");
                List<List<string>> _filter = new List<List<string>>();
                _filter.Add(_DateNow);
                CommonParam param = new CommonParam();
                var dataRow = new BackendAdapter(param).Get<List<HipoFinanceReportSDO>>("api/statistic/FinanceReport", ApiConsumers.MosConsumer, _filter, param);
                if (dataRow != null)
                {
                    chartVienPhi.DataSource = dataRow;
                    //if (currentLstRoomCouter != null)
                    //{
                    //    decimal maxValue = currentLstRoomCouter.Max(o => (o.TOTAL_TODAY_SERVICE_REQ ?? 0));

                    //    Diagram.AxisY.VisualRange.MinValue = 0;
                    //    Diagram.AxisY.VisualRange.MaxValue = maxValue;
                    //    Diagram.AxisY.WholeRange.MinValue = 0;
                    //    Diagram.AxisY.WholeRange.MaxValue = maxValue;
                    //    Diagram.AxisY.NumericScaleOptions.GridSpacing = ((int)(maxValue / 100) <= 0 ? 1 : (int)(maxValue / 100));

                    //    LoadSeries(chartVienPhi.Series[4], currentLstRoomCouter, "Tổng cộng", "Tổng cộng", TypeChartColumn.Total);
                    //    LoadSeries(chartVienPhi.Series[1], currentLstRoomCouter, "Bảo hiểm", "Bảo hiểm", TypeChartColumn.BaoHiem);
                    //    LoadSeries(chartVienPhi.Series[2], currentLstRoomCouter, "Viện phí", "Viện phí", TypeChartColumn.VienPhi);
                    //    LoadSeries(chartVienPhi.Series[2], currentLstRoomCouter, "Tạm ứng", "Tạm ứng", TypeChartColumn.TamUng);

                    //}
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
